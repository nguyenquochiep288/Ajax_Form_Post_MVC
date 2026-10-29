using System;
using System.Collections.Generic;
using System.Web.Hosting;
using CrystalDecisions.CrystalReports.Engine;
using Ras = CrystalDecisions.ReportAppServer.ReportDefModel;

namespace MVC_QuanLyTHP.Class
{
    internal static class ReportLogo
    {
        public static void Apply(ReportDocument report)
        {
            var path = HostingEnvironment.MapPath("~/Images_Upload/Logo/trang-hiep-phat-20261028.jpg");
            ApplyToDocument(report, path);
        }

        private static void ApplyToDocument(ReportDocument report, string path)
        {
            var definition = report.ReportClientDocument.ReportDefController;
            var controller = definition.ReportObjectController;
            var objects = controller.GetAllReportObjects();
            var logos = new List<Ras.PictureObject>();
            for (var i = 0; i < objects.Count; i++)
            {
                var picture = objects[i] as Ras.PictureObject;
                if (picture != null && picture.GraphicLocationFormula != null &&
                    string.Equals(picture.GraphicLocationFormula.Text.Trim(), "{@ICON}", StringComparison.OrdinalIgnoreCase))
                    logos.Add(picture);
            }

            foreach (var logo in logos)
            {
                var section = definition.ReportDefinition.FindSectionByName(logo.SectionName);
                var height = report.ReportDefinition.Sections[section.Name].Height;
                // Import the actual image so Crystal supplies its native dimensions.
                // A dynamic ICON image otherwise inherits the old image's scale and can overflow.
                var imported = controller.ImportPicture(path, section, 0, 0);
                var replaced = false;
                try
                {
                    var replacement = (Ras.PictureObject)imported.Clone(true);
                    replacement.Format = logo.Format;
                    replacement.Border = logo.Border;
                    replacement.GraphicLocationFormula.Text = string.Empty;
                    replacement.GraphicLocation = string.Empty;
                    var scale = Math.Min((double)logo.Width / imported.OriginalWidth,
                        (double)logo.Height / imported.OriginalHeight);
                    replacement.Width = (int)Math.Round(imported.OriginalWidth * scale);
                    replacement.Height = (int)Math.Round(imported.OriginalHeight * scale);
                    replacement.Left = logo.Left + (logo.Width - replacement.Width) / 2;
                    replacement.Top = logo.Top + (logo.Height - replacement.Height) / 2;
                    controller.Modify(imported, replacement);
                    controller.Remove(logo);
                    replaced = true;
                }
                finally
                {
                    if (!replaced)
                        controller.Remove(imported);
                    report.ReportDefinition.Sections[section.Name].Height = height;
                }
            }
        }
    }
}
