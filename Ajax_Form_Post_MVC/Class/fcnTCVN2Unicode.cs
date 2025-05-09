using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;
using System.Web.UI.WebControls;
using System.Web.Services.Description;
using System.Reflection;
using DatabaseTHP;
using ZXing;
using Syncfusion.EJ2.Spreadsheet;
using System.Runtime.Remoting.Messaging;
namespace MVC_QuanLyTHP.Class
{
    public class fcnTCVN2Unicode
    {
        private static char[] tcvnchars = {
            'µ', '¸', '¶', '·', '¹',
            '¨', '»', '¾', '¼', '½', 'Æ',
            '©', 'Ç', 'Ê', 'È', 'É', 'Ë',
            '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ',
            'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö',
            '×', 'Ý', 'Ø', 'Ü', 'Þ',
            'ß', 'ã', 'á', 'â', 'ä',
            '«', 'å', 'è', 'æ', 'ç', 'é',
            '¬', 'ê', 'í', 'ë', 'ì', 'î',
            'ï', 'ó', 'ñ', 'ò', 'ô',
            '­', 'õ', 'ø', 'ö', '÷', 'ù',
            'ú', 'ý', 'û', 'ü', 'þ',
            '¡', '¢', '§', '£', '¤', '¥', '¦'};
        private static char[] unichars = {
            'à', 'á', 'ả', 'ã', 'ạ',
            'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ',
            'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ',
            'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ',
            'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ',
            'ì', 'í', 'ỉ', 'ĩ', 'ị',
            'ò', 'ó', 'ỏ', 'õ', 'ọ',
            'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ',
            'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ',
            'ù', 'ú', 'ủ', 'ũ', 'ụ',
            'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự',
            'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ',
            'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'};
        private static char[] convertTable;
        static fcnTCVN2Unicode()
        {
            convertTable = new char[256];
            for (int i = 0; i < 256; i++)
                convertTable[i] = (char)i;
            for (int i = 0; i < tcvnchars.Length; i++)
                convertTable[tcvnchars[i]] = unichars[i];
        }
        public static string TCVN3ToUnicode(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                char[] chars = value.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                    if (chars[i] < (char)256)
                        chars[i] = convertTable[chars[i]];
                return new string(chars);
            }
            catch
            {
                return value;
            }
        }

        public static string UnicodeToTCVN3(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            try
            {
                for (int i = 0; i < unichars.Length; i++)
                {
                    value = value.Replace(unichars[i], tcvnchars[i]);
                }
                return value;
            }
            catch
            {
                return value;
            }
        }

        public static T ConvertObjectTCVN3ToUnicode<T>(T Object)
        {
            try
            {
                var types = Object.GetType();
                var properties = types.GetProperties();
                T obj = Activator.CreateInstance<T>();

                foreach (PropertyInfo itmPropertyInfo in properties)
                {

                    string value = itmPropertyInfo.PropertyType.GenericTypeArguments.Count() > 0 ? itmPropertyInfo.PropertyType.GenericTypeArguments[0].Name.ToUpper() : itmPropertyInfo.PropertyType.Name.ToUpper();
                    switch (value)
                    {
                        case "STRING":
                            object val = itmPropertyInfo.GetValue(Object);
                            if (val != null)
                            {
                                string strvalue = TCVN3ToUnicode(val.ToString());
                                itmPropertyInfo.SetValue(obj, strvalue);
                            }
                            break;
                        default:
                            break;
                    }
                }
                return obj;
            }
           catch
            {
                return Object;
            }
           
        }

        public static T ConvertObjectUnicodeToTCVN3<T>(T Object)
        {
            try
            {
                var types = Object.GetType();
                var properties = types.GetProperties();
                T obj = Activator.CreateInstance<T>();

                foreach (PropertyInfo itmPropertyInfo in properties)
                {

                    string value = itmPropertyInfo.PropertyType.GenericTypeArguments.Count() > 0 ? itmPropertyInfo.PropertyType.GenericTypeArguments[0].Name.ToUpper() : itmPropertyInfo.PropertyType.Name.ToUpper();
                    switch (value)
                    {
                        case "STRING":
                            object val = itmPropertyInfo.GetValue(Object);
                            if (val != null)
                            {
                                string strvalue = UnicodeToTCVN3(val.ToString());
                                itmPropertyInfo.SetValue(obj, strvalue);
                            }
                            break;
                        default:
                            break;
                    }
                }
                return obj;
            }
            catch
            {
                return Object;
            }

        }
    }
}