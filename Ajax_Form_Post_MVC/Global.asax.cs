using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_QuanLyTHP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ConcurrentDictionary<string, ActiveAccessSession> ActiveSessions = new ConcurrentDictionary<string, ActiveAccessSession>();
        private static readonly ConcurrentDictionary<string, DateTime> RevokedSessions = new ConcurrentDictionary<string, DateTime>();
        private static readonly object ActiveSessionsSync = new object();
        private static readonly TimeSpan ActiveSessionLifetime = TimeSpan.FromMinutes(35);
        private static DateTime LastSessionCleanup = DateTime.MinValue;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (context == null || context.Session == null)
            {
                return;
            }

            CleanupExpiredSessions();

            string sessionId = context.Session.SessionID;
            string userId = context.Session[Sessions.idUser] as string;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionId))
            {
                return;
            }

            Login_Model login = context.Session[Sessions.Login_Model] as Login_Model;
            string userName = context.Session[Sessions.User] as string;
            string fullName = login == null ? userName : login.fullname;
            string ipAddress = context.Request.UserHostAddress;
            string accessTokenId = GetTokenFingerprint(Convert.ToString(context.Session[Sessions.Token]), "A");
            string refreshTokenId = GetTokenFingerprint(Convert.ToString(context.Session["RefreshToken"]), "R");
            DateTime? accessTokenExpires = GetTokenExpiration(context.Session[Sessions.Expires]);

            ActiveAccessSession previous;
            bool resolveComputerName;
            lock (ActiveSessionsSync)
            {
                if (IsSessionRevoked(sessionId))
                {
                    context.Session.Clear();
                    Utility.ExpirePersistedTokenCookie();
                    context.Response.Redirect(VirtualPathUtility.ToAbsolute("~/Admin/Index"), false);
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                resolveComputerName = !ActiveSessions.TryGetValue(sessionId, out previous)
                    || !string.Equals(previous.IpAddress, ipAddress, StringComparison.OrdinalIgnoreCase);
                string computerName = resolveComputerName ? "Không xác định" : previous.ComputerName;
                var current = new ActiveAccessSession(sessionId, userId, userName, fullName, ipAddress, computerName,
                    accessTokenId, refreshTokenId, Convert.ToString(context.Session["RefreshToken"]), accessTokenExpires, DateTime.Now);
                ActiveSessions.AddOrUpdate(sessionId, current, (key, existing) => current);
            }

            if (resolveComputerName && !string.IsNullOrWhiteSpace(ipAddress))
            {
                ThreadPool.QueueUserWorkItem(state => ResolveComputerName(sessionId, ipAddress));
            }
        }

        protected void Session_End()
        {
            if (Session != null)
            {
                RemoveActiveSession(Session.SessionID);
            }
        }

        public static IReadOnlyCollection<ActiveAccessSession> GetActiveAccessSessions()
        {
            CleanupExpiredSessions(force: true);
            return ActiveSessions.Values.OrderByDescending(session => session.LastSeen).ToList().AsReadOnly();
        }

        private static void CleanupExpiredSessions(bool force = false)
        {
            lock (ActiveSessionsSync)
            {
                DateTime now = DateTime.Now;
                if (!force && now - LastSessionCleanup < TimeSpan.FromMinutes(1))
                {
                    return;
                }

                LastSessionCleanup = now;
                DateTime cutoff = now.Subtract(ActiveSessionLifetime);
                foreach (KeyValuePair<string, ActiveAccessSession> session in ActiveSessions)
                {
                    if (session.Value.LastSeen < cutoff)
                    {
                        ActiveAccessSession ignored;
                        ActiveSessions.TryRemove(session.Key, out ignored);
                    }
                }

                foreach (KeyValuePair<string, DateTime> session in RevokedSessions)
                {
                    if (session.Value <= now)
                    {
                        DateTime ignored;
                        RevokedSessions.TryRemove(session.Key, out ignored);
                    }
                }
            }
        }

        public static void RemoveActiveSession(string sessionId)
        {
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                ActiveAccessSession ignored;
                ActiveSessions.TryRemove(sessionId, out ignored);
            }
        }

        public static bool TryDisconnectActiveSession(string sessionId, out ActiveAccessSession disconnectedSession)
        {
            disconnectedSession = null;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }

            lock (ActiveSessionsSync)
            {
                if (!ActiveSessions.TryRemove(sessionId, out disconnectedSession))
                {
                    return false;
                }

                RevokedSessions[sessionId] = DateTime.Now.Add(ActiveSessionLifetime);
                return true;
            }
        }

        public static void ActivateSession(string sessionId)
        {
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                DateTime ignored;
                RevokedSessions.TryRemove(sessionId, out ignored);
            }
        }

        public static void UpdateActiveSessionTokens(string sessionId, string accessToken, string refreshToken, object expires)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return;
            }

            lock (ActiveSessionsSync)
            {
                ActiveAccessSession current;
                if (!ActiveSessions.TryGetValue(sessionId, out current) || IsSessionRevoked(sessionId))
                {
                    return;
                }

                var updated = new ActiveAccessSession(current.SessionId, current.UserId, current.UserName,
                    current.FullName, current.IpAddress, current.ComputerName,
                    GetTokenFingerprint(accessToken, "A"), GetTokenFingerprint(refreshToken, "R"),
                    refreshToken, GetTokenExpiration(expires), current.LastSeen);
                ActiveSessions.TryUpdate(sessionId, updated, current);
            }
        }

        private static bool IsSessionRevoked(string sessionId)
        {
            DateTime expiresAt;
            if (!RevokedSessions.TryGetValue(sessionId, out expiresAt))
            {
                return false;
            }

            if (expiresAt > DateTime.Now)
            {
                return true;
            }

            DateTime ignored;
            RevokedSessions.TryRemove(sessionId, out ignored);
            return false;
        }

        private static void ResolveComputerName(string sessionId, string ipAddress)
        {
            try
            {
                string computerName = Dns.GetHostEntry(ipAddress).HostName;
                ActiveAccessSession current;
                while (!string.IsNullOrWhiteSpace(computerName)
                    && ActiveSessions.TryGetValue(sessionId, out current)
                    && string.Equals(current.IpAddress, ipAddress, StringComparison.OrdinalIgnoreCase))
                {
                    var updated = new ActiveAccessSession(current.SessionId, current.UserId, current.UserName,
                        current.FullName, current.IpAddress, computerName, current.AccessTokenId,
                        current.RefreshTokenId, current.RefreshToken, current.AccessTokenExpires, current.LastSeen);
                    if (ActiveSessions.TryUpdate(sessionId, updated, current))
                    {
                        break;
                    }
                }
            }
            catch
            {
                // Reverse DNS is optional; many client networks do not publish host records.
            }
        }

        private static string GetTokenFingerprint(string token, string prefix)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return "Không có";
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
                string fingerprint = Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
                return prefix + "-" + fingerprint.Substring(0, 10);
            }
        }

        private static DateTime? GetTokenExpiration(object value)
        {
            if (value == null)
            {
                return null;
            }

            DateTime expiration;
            if (!DateTime.TryParse(Convert.ToString(value), out expiration))
            {
                return null;
            }

            return expiration.Kind == DateTimeKind.Utc ? expiration.ToLocalTime() : expiration;
        }
    }

    public sealed class ActiveAccessSession
    {
        public ActiveAccessSession(string sessionId, string userId, string userName, string fullName,
            string ipAddress, string computerName, string accessTokenId, string refreshTokenId, string refreshToken,
            DateTime? accessTokenExpires, DateTime lastSeen)
        {
            SessionId = sessionId;
            UserId = userId;
            UserName = userName;
            FullName = fullName;
            IpAddress = ipAddress;
            ComputerName = computerName;
            AccessTokenId = accessTokenId;
            RefreshTokenId = refreshTokenId;
            RefreshToken = refreshToken;
            AccessTokenExpires = accessTokenExpires;
            LastSeen = lastSeen;
        }

        public string SessionId { get; private set; }
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string FullName { get; private set; }
        public string IpAddress { get; private set; }
        public string ComputerName { get; private set; }
        public string AccessTokenId { get; private set; }
        public string RefreshTokenId { get; private set; }
        internal string RefreshToken { get; private set; }
        public DateTime? AccessTokenExpires { get; private set; }
        public DateTime LastSeen { get; private set; }
    }
}
