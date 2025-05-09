using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using System.Web.Razor.Parser.SyntaxTree;

namespace MVC_QuanLyTHP.Controllers
{
    public class LoadMenuController : Controller
    {
        //
        // GET: /Partial_admin/
        [ChildActionOnly]
        public ActionResult Index()
        {
            if (Utility.KiemTra())
            {
                ViewBag.ViewMenu = "";
                return PartialView();
            }
            string menu = Utility.Menu;
            if (string.IsNullOrEmpty(menu))
            {
                menu = " <li><a href=\"/Admin/Home\" id=\"li\"><i class=\"fa fa-home\"></i> <span>Trang chủ</span></a></li>";
                var lstMenu = Utility.GetMenu();
                foreach (v_web_Menu itm in lstMenu.Where(s => string.IsNullOrEmpty(s.ID_QUYENCHA) && s.ISACTIVE).OrderBy(s => s.STT))
                {
                    var lstchildren = lstMenu.Where(s => s.ID_QUYENCHA == itm.ID && s.ISACTIVE).OrderBy(s => s.STT);
                    if (lstchildren != null && lstchildren.Count() > 0)
                    {
                        string menu1 = "";
                        string menu2 = "";
                        menu1 += "<li class=\"nav-parent\">" +
                         "<a href=\"#\"><i class=\"" + itm.ICON + "\"></i> <span>" + itm.NAME + "</span></a>" +
                         "<ul class=\"children\">";

                        foreach (v_web_Menu children in lstchildren)
                        {
                            var lstchildren1 = lstMenu.Where(s => s.ID_QUYENCHA == children.ID && s.ISACTIVE).OrderBy(s => s.STT);
                            if (lstchildren1 != null && lstchildren1.Count() > 0 &&  children.CONTROLLERNAME != API.BaoCao)
                            {
                               
                                string menu3 = "";
                                string menu4 = "";
                                menu3 += "<li>" +
                                       "<a><i class=\"" + children.ICON + "\"></i> <span>" + children.NAME + "</span></a>" +
                                       "<ul class=\"children\" style=\"display: block;\">";

                                foreach (v_web_Menu children1 in lstchildren1)
                                {
                                    if (Utility.KiemTraQuyen(children.CONTROLLERNAME, API.Xem, children1))
                                        menu4 += "<li><a href=\"/" + children1.CONTROLLERNAME + "/" + children1.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + children1.ICON + "\"></i> " + children1.NAME + "</a></li>";
                                }


                                if (!string.IsNullOrEmpty(menu4.Trim()))
                                {
                                    menu3 += menu4;
                                    menu3 += "</ul>";
                                    menu3 += "</li>";
                                    menu2 += menu3;
                                }
                            }
                            else
                            {
                                if (children.CONTROLLERNAME == API.BaoCao)
                                {
                                    foreach (v_web_Menu children1 in lstchildren1)
                                    {
                                        if (Utility.KiemTraQuyen(children.CONTROLLERNAME, API.Xem, children1))
                                        {
                                            menu2 += "<li><a href=\"/" + children.CONTROLLERNAME + "/" + children.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + children.ICON + "\"></i> " + children.NAME + "</a></li>";
                                            break;
                                        }    
                                           
                                    }
                                }   
                                else
                                {
                                    if (Utility.KiemTraQuyen(children.CONTROLLERNAME, API.Xem, children))
                                        menu2 += "<li><a href=\"/" + children.CONTROLLERNAME + "/" + children.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + children.ICON + "\"></i> " + children.NAME + "</a></li>";

                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(menu2.Trim()))
                        {
                            menu1 += menu2;
                            menu1 += "</ul>";
                            menu1 += "</li>";
                            menu += menu1;
                        }
                    }
                    else
                        menu += "<li><a href=\"/" + itm.CONTROLLERNAME + "/" + itm.ACTIONNAME + "\" id=\"li\"><i class=\"" + itm.ICON + "\"></i> " + itm.NAME + "</a></li>";
                }
                Session[Sessions.Menu] = menu;
            }
            string url = Request.Url.AbsolutePath;
            var lstString = Request.Url.AbsolutePath.Split('/');
            if (menu.Contains(Request.Url.AbsolutePath) || (lstString.Length > 0 && lstString.Length > 1 && menu.Contains(lstString[1])))
            {
                string text = "<li><a href=\"" + Request.Url.AbsolutePath + "\" id=\"li\">";


                if (menu.Contains(text))
                {
                    menu = menu.Replace(text, "<li class=\"active\"><a href=\"" + Request.Url.AbsolutePath + "\"" + " id=\"li\">");
                }
                else
                {
                    if (lstString.Length > 0 && lstString.Length == 2)
                    {
                        var lstMenu = Utility.GetMenu();
                        var Menu = lstMenu.Where(s => s.CONTROLLERNAME != null && s.CONTROLLERNAME.ToLower() == lstString[1].ToLower() && s.ISACTIVE).FirstOrDefault();
                        if (Menu != null)
                        {
                            url = "/" + Menu.CONTROLLERNAME + "/" + Menu.ACTIONNAME;
                            text = "<li><a href=\"" + url + "\" id=\"li\">";
                            if (menu.Contains(text))
                            {
                                menu = menu.Replace(text, "<li class=\"active\"><a href=\"" + url + "\"" + " id=\"li\">");
                            }
                        }
                    }
                    else if (lstString.Length > 0 && lstString.Length > 2)
                    {
                        var lstMenu = Utility.GetMenu();
                        var Menu = lstMenu.Where(s => s.CONTROLLERNAME != null && s.CONTROLLERNAME.ToLower() == lstString[1].ToLower() && s.ACTIONNAME.ToLower() == lstString[2].ToLower() && s.ISACTIVE).FirstOrDefault();
                        if (Menu != null)
                        {
                            url = "/" + Menu.CONTROLLERNAME + "/" + Menu.ACTIONNAME;
                            text = "<li><a href=\"" + url + "\" id=\"li\">";
                            if (menu.Contains(text))
                            {
                                menu = menu.Replace(text, "<li class=\"active\"><a href=\"" + url + "\"" + " id=\"li\">");
                            }
                        }
                    }
                }
                text = text.Replace("id=\"li\"", "id=\"ulli\"");
                //text = "<li><a href=\"" + Request.Url.AbsolutePath + "\" id=\"ulli\">";
                if (menu.Contains(text))
                {
                    int vitri = menu.LastIndexOf(text);
                    if (vitri > 0)
                    {
                        menu = menu.Replace(text, "<li class=\"active\"><a href=\"" + url + "\"" + " id=\"ulli\">");
                        text = menu.Substring(0, vitri);
                        vitri = text.LastIndexOf("<ul class=\"children\">");
                        if (vitri > 0)
                        {
                            menu = menu.Replace(menu.Substring(0, vitri) + "<ul class=\"children\">", menu.Substring(0, vitri) + "<ul class=\"children\" style=\"display: block;\">");
                            text = text.Substring(0, vitri);
                            vitri = text.LastIndexOf("<li class=\"nav-parent\">");
                            if (vitri > 0)
                            {
                                menu = menu.Replace(menu.Substring(0, vitri) + "<li class=\"nav-parent\">", menu.Substring(0, vitri) + "<li class=\"nav-parent active nav-active\">");
                            }
                        }
                    }

                }
            }

            ViewBag.ViewMenu = menu;
            return PartialView();
        }
    }
}
