using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;

namespace MVC_QuanLyTHP.Controllers
{

	public class LoadMenuController : Controller
	{
		[ChildActionOnly]
		public ActionResult Index()
		{
			if (!Utility.KiemTra())
			{
				string text = Utility.Menu;
				if (string.IsNullOrEmpty(text))
				{
					text = " <li><a href=\"/Admin/Home\" id=\"li\"><i class=\"fa fa-home\"></i> <span>Trang chủ</span></a></li>";
					List<v_web_Menu> menu = Utility.GetMenu();
					foreach (v_web_Menu itm in from s in menu
											   where string.IsNullOrEmpty(s.ID_QUYENCHA) && s.ISACTIVE
											   orderby s.STT
											   select s)
					{
						IOrderedEnumerable<v_web_Menu> orderedEnumerable = from s in menu
																		   where s.ID_QUYENCHA == itm.ID && s.ISACTIVE
																		   orderby s.STT
																		   select s;
						if (orderedEnumerable != null && orderedEnumerable.Count() > 0)
						{
							string text2 = "";
							string text3 = "";
							text2 = text2 + "<li class=\"nav-parent\"><a href=\"#\"><i class=\"" + itm.ICON + "\"></i> <span>" + itm.NAME + "</span></a><ul class=\"children\">";
							foreach (v_web_Menu children in orderedEnumerable)
							{
								IOrderedEnumerable<v_web_Menu> orderedEnumerable2 = from s in menu
																					where s.ID_QUYENCHA == children.ID && s.ISACTIVE
																					orderby s.STT
																					select s;
								if (orderedEnumerable2 != null && orderedEnumerable2.Count() > 0 && children.CONTROLLERNAME != "ViewReport")
								{
									string text4 = "";
									string text5 = "";
									text4 = text4 + "<li><a><i class=\"" + children.ICON + "\"></i> <span>" + children.NAME + "</span></a><ul class=\"children\" style=\"display: block;\">";
									foreach (v_web_Menu item in orderedEnumerable2)
									{
										if (Utility.KiemTraQuyen(children.CONTROLLERNAME, "View", item))
										{
											text5 = text5 + "<li><a href=\"/" + item.CONTROLLERNAME + "/" + item.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + item.ICON + "\"></i> " + item.NAME + "</a></li>";
										}
									}
									if (!string.IsNullOrEmpty(text5.Trim()))
									{
										text4 += text5;
										text4 += "</ul>";
										text4 += "</li>";
										text3 += text4;
									}
								}
								else if (children.CONTROLLERNAME == "ViewReport")
								{
									foreach (v_web_Menu item2 in orderedEnumerable2)
									{
										if (Utility.KiemTraQuyen(children.CONTROLLERNAME, "View", item2))
										{
											text3 = text3 + "<li><a href=\"/" + children.CONTROLLERNAME + "/" + children.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + children.ICON + "\"></i> " + children.NAME + "</a></li>";
											break;
										}
									}
								}
								else if (Utility.KiemTraQuyen(children.CONTROLLERNAME, "View", children))
								{
									text3 = text3 + "<li><a href=\"/" + children.CONTROLLERNAME + "/" + children.ACTIONNAME + "\" id=\"ulli\"><i class=\"" + children.ICON + "\"></i> " + children.NAME + "</a></li>";
								}
							}
							if (!string.IsNullOrEmpty(text3.Trim()))
							{
								text2 += text3;
								text2 += "</ul>";
								text2 += "</li>";
								text += text2;
							}
						}
						else
						{
							text = text + "<li><a href=\"/" + itm.CONTROLLERNAME + "/" + itm.ACTIONNAME + "\" id=\"li\"><i class=\"" + itm.ICON + "\"></i> " + itm.NAME + "</a></li>";
						}
					}
					base.Session["Menu"] = text;
				}
				string text6 = base.Request.Url.AbsolutePath;
				string[] lstString = base.Request.Url.AbsolutePath.Split('/');
				if (text.Contains(base.Request.Url.AbsolutePath) || (lstString.Length != 0 && lstString.Length > 1 && text.Contains(lstString[1])))
				{
					string text7 = "<li><a href=\"" + base.Request.Url.AbsolutePath + "\" id=\"li\">";
					string text8 = "<li><a href=\"" + base.Request.Url.AbsolutePath + base.Request.Url.Query + "\" id=\"li\">";
					if (text.Contains(text7) || text.Contains(text8))
					{
						text = text.Replace(text.Contains(text7) ? text7 : text8, "<li class=\"active\"><a href=\"" + base.Request.Url.AbsolutePath + "\" id=\"li\">");
					}
					else if (lstString.Length != 0 && lstString.Length == 2)
					{
						List<v_web_Menu> menu2 = Utility.GetMenu();
						v_web_Menu v_web_Menu2 = menu2.Where((v_web_Menu s) => s.CONTROLLERNAME != null && s.CONTROLLERNAME.ToLower() == lstString[1].ToLower() && s.ISACTIVE).FirstOrDefault();
						if (v_web_Menu2 != null)
						{
							text6 = "/" + v_web_Menu2.CONTROLLERNAME + "/" + v_web_Menu2.ACTIONNAME;
							text7 = "<li><a href=\"" + text6 + "\" id=\"li\">";
							if (text.Contains(text7))
							{
								text = text.Replace(text7, "<li class=\"active\"><a href=\"" + text6 + "\" id=\"li\">");
							}
						}
					}
					else if (lstString.Length != 0 && lstString.Length > 2)
					{
						List<v_web_Menu> menu3 = Utility.GetMenu();
						v_web_Menu v_web_Menu3 = menu3.Where((v_web_Menu s) => s.CONTROLLERNAME != null && s.CONTROLLERNAME.ToLower() == lstString[1].ToLower() && s.ACTIONNAME.ToLower() == lstString[2].ToLower() && s.ISACTIVE).FirstOrDefault();
						if (v_web_Menu3 != null)
						{
							text6 = "/" + v_web_Menu3.CONTROLLERNAME + "/" + v_web_Menu3.ACTIONNAME;
							text7 = "<li><a href=\"" + text6 + "\" id=\"li\">";
							if (text.Contains(text7))
							{
								text = text.Replace(text7, "<li class=\"active\"><a href=\"" + text6 + "\" id=\"li\">");
							}
						}
					}
					text7 = text7.Replace("id=\"li\"", "id=\"ulli\"");
					text8 = text8.Replace("id=\"li\"", "id=\"ulli\"");
					if (text.Contains(text7) || text.Contains(text8))
					{
						bool flag = false;
						int num = text.LastIndexOf(text7);
						if (num < 0)
						{
							num = text.LastIndexOf(text8);
							flag = true;
						}
						if (num > 0)
						{
							text = text.Replace(flag ? text8 : text7, "<li class=\"active\"><a href=\"" + text6 + "\" id=\"ulli\">");
							text7 = text.Substring(0, num);
							num = text7.LastIndexOf("<ul class=\"children\">");
							if (num > 0)
							{
								text = text.Replace(text.Substring(0, num) + "<ul class=\"children\">", text.Substring(0, num) + "<ul class=\"children\" style=\"display: block;\">");
								text7 = text7.Substring(0, num);
								num = text7.LastIndexOf("<li class=\"nav-parent\">");
								if (num > 0)
								{
									text = text.Replace(text.Substring(0, num) + "<li class=\"nav-parent\">", text.Substring(0, num) + "<li class=\"nav-parent active nav-active\">");
								}
							}
						}
					}
				}
				base.ViewBag.ViewMenu = text;
				return PartialView();
			}
			base.ViewBag.ViewMenu = "";
			return PartialView();
		}
	}
}
