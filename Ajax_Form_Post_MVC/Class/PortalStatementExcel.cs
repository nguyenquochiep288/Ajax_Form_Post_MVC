using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace MVC_QuanLyTHP.Class
{
    // Minimal OpenXML workbook: no macros, external links or user-controlled formulas.
    public static class PortalStatementExcel
    {
        private const string Ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        public static byte[] Create(JObject report)
        {
            using (var output = new MemoryStream())
            {
                using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true))
                {
                    Write(zip, "[Content_Types].xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/><Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/></Types>");
                    Write(zip, "_rels/.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
                    Write(zip, "xl/workbook.xml", "<workbook xmlns=\"" + Ns + "\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Đối chiếu công nợ\" sheetId=\"1\" r:id=\"rId1\"/></sheets><definedNames><definedName name=\"_xlnm.Print_Titles\" localSheetId=\"0\">'Đối chiếu công nợ'!$1:$7</definedName></definedNames><calcPr calcId=\"191029\" fullCalcOnLoad=\"1\"/></workbook>");
                    Write(zip, "xl/_rels/workbook.xml.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/><Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/></Relationships>");
                    Write(zip, "xl/styles.xml", "<styleSheet xmlns=\"" + Ns + "\"><numFmts count=\"1\"><numFmt numFmtId=\"164\" formatCode=\"#,##0.##;[Red](#,##0.##)\"/></numFmts><fonts count=\"3\"><font><sz val=\"10\"/><name val=\"Arial\"/></font><font><b/><sz val=\"15\"/><color rgb=\"FF008D81\"/><name val=\"Arial\"/></font><font><b/><sz val=\"10\"/><color rgb=\"FFFFFFFF\"/><name val=\"Arial\"/></font></fonts><fills count=\"3\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill><fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF008D81\"/><bgColor indexed=\"64\"/></patternFill></fill></fills><borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders><cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs><cellXfs count=\"4\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"><alignment vertical=\"top\" wrapText=\"1\"/></xf><xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/><xf numFmtId=\"0\" fontId=\"2\" fillId=\"2\" borderId=\"0\" xfId=\"0\" applyFill=\"1\" applyFont=\"1\"><alignment wrapText=\"1\"/></xf><xf numFmtId=\"164\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"><alignment horizontal=\"right\"/></xf></cellXfs><cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles></styleSheet>");
                    using (var stream = zip.CreateEntry("xl/worksheets/sheet1.xml").Open())
                    using (var xml = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false) }))
                    {
                        xml.WriteStartElement("worksheet", Ns);
                        xml.WriteStartElement("sheetViews"); xml.WriteStartElement("sheetView"); xml.WriteAttributeString("workbookViewId", "0");
                        xml.WriteStartElement("pane"); xml.WriteAttributeString("ySplit", "7"); xml.WriteAttributeString("topLeftCell", "A8"); xml.WriteAttributeString("activePane", "bottomLeft"); xml.WriteAttributeString("state", "frozen"); xml.WriteEndElement(); xml.WriteEndElement(); xml.WriteEndElement();
                        xml.WriteStartElement("cols");
                        var widths = new[] { 7, 14, 23, 24, 21, 21, 22 };
                        for (int i = 0; i < widths.Length; i++) { xml.WriteStartElement("col"); xml.WriteAttributeString("min", (i + 1).ToString()); xml.WriteAttributeString("max", (i + 1).ToString()); xml.WriteAttributeString("width", widths[i].ToString()); xml.WriteAttributeString("customWidth", "1"); xml.WriteEndElement(); }
                        xml.WriteEndElement(); xml.WriteStartElement("sheetData");
                        var customer = report["customer"];
                        TextRow(xml, 1, "TRANG HIỆP PHÁT", 1);
                        TextRow(xml, 2, "BẢNG ĐỐI CHIẾU CÔNG NỢ", 1);
                        TextRow(xml, 3, "Khách hàng: " + (string)customer["name"] + " · Mã: " + (string)customer["code"]);
                        TextRow(xml, 4, "Địa chỉ: " + (string)customer["address"]);
                        TextRow(xml, 5, "Kỳ đối chiếu: " + Date(report["from"]) + " – " + Date(report["to"]));
                        TextRow(xml, 6, "Đơn vị: VNĐ · Toàn bộ phát sinh trong kỳ, không áp dụng tìm kiếm mã chứng từ.");
                        xml.WriteStartElement("row"); xml.WriteAttributeString("r", "7");
                        var headers = new[] { "STT", "Ngày", "Mã chứng từ", "Loại chứng từ", "Tăng nợ", "Giảm nợ", "Dư nợ" };
                        for (int i = 0; i < headers.Length; i++) Cell(xml, ((char)('A' + i)) + "7", headers[i], 2);
                        xml.WriteEndElement();
                        SummaryRow(xml, 8, "Dư đầu kỳ", report["summary"]["opening"].Value<decimal>());
                        int row = 9, index = 1;
                        foreach (var item in (JArray)report["rows"])
                        {
                            xml.WriteStartElement("row"); xml.WriteAttributeString("r", row.ToString());
                            Numeric(xml, "A" + row, index++); Cell(xml, "B" + row, Date(item["date"])); Cell(xml, "C" + row, (string)item["code"]); Cell(xml, "D" + row, Kind((string)item["kind"]));
                            Numeric(xml, "E" + row, item["increase"].Value<decimal>()); Numeric(xml, "F" + row, item["decrease"].Value<decimal>());
                            Numeric(xml, "G" + row, item["balance"].Value<decimal>(), "G" + (row - 1) + "+E" + row + "-F" + row);
                            xml.WriteEndElement(); row++;
                        }
                        int totalRow = row;
                        xml.WriteStartElement("row"); xml.WriteAttributeString("r", row.ToString()); Cell(xml, "D" + row, "Tổng phát sinh / Dư tính toán");
                        Numeric(xml, "E" + row, report["increase"].Value<decimal>(), row > 9 ? "SUM(E9:E" + (row - 1) + ")" : "0");
                        Numeric(xml, "F" + row, report["decrease"].Value<decimal>(), row > 9 ? "SUM(F9:F" + (row - 1) + ")" : "0");
                        Numeric(xml, "G" + row, report["calculatedClosing"].Value<decimal>(), "G" + (row - 1)); xml.WriteEndElement(); row++;
                        SummaryRow(xml, row++, "Dư cuối kỳ theo hệ thống", report["summary"]["closing"].Value<decimal>());
                        xml.WriteStartElement("row"); xml.WriteAttributeString("r", row.ToString()); Cell(xml, "D" + row, "Chênh lệch cần đối chiếu"); Numeric(xml, "G" + row, report["difference"].Value<decimal>(), "G" + (row - 1) + "-G" + totalRow); xml.WriteEndElement(); row++;
                        TextRow(xml, row++, "Số dư âm thể hiện khách hàng trả trước / dư có. Hai bên xác nhận sau khi đối chiếu chứng từ.");
                        TextRow(xml, row + 1, "ĐẠI DIỆN KHÁCH HÀNG                                       ĐẠI DIỆN TRANG HIỆP PHÁT");
                        xml.WriteEndElement();
                        xml.WriteStartElement("mergeCells"); xml.WriteAttributeString("count", "8");
                        foreach (int number in new[] { 1, 2, 3, 4, 5, 6, row - 1, row + 1 }) { xml.WriteStartElement("mergeCell"); xml.WriteAttributeString("ref", "A" + number + ":G" + number); xml.WriteEndElement(); }
                        xml.WriteEndElement();
                        xml.WriteStartElement("pageMargins"); foreach (var side in new[] { "left", "right", "top", "bottom", "header", "footer" }) xml.WriteAttributeString(side, side == "header" || side == "footer" ? "0.2" : "0.35"); xml.WriteEndElement();
                        xml.WriteStartElement("pageSetup"); xml.WriteAttributeString("paperSize", "9"); xml.WriteAttributeString("orientation", "landscape"); xml.WriteAttributeString("fitToWidth", "1"); xml.WriteAttributeString("fitToHeight", "0"); xml.WriteEndElement();
                        xml.WriteStartElement("headerFooter"); xml.WriteElementString("oddFooter", "&LĐối chiếu công nợ&RTrang &P / &N"); xml.WriteEndElement();
                        xml.WriteEndElement();
                    }
                }
                return output.ToArray();
            }
        }
        private static void Write(ZipArchive zip, string path, string content) { using (var writer = new StreamWriter(zip.CreateEntry(path).Open(), new UTF8Encoding(false))) writer.Write(content); }
        private static void Cell(XmlWriter xml, string address, string text, int style = 0) { xml.WriteStartElement("c"); xml.WriteAttributeString("r", address); xml.WriteAttributeString("s", style.ToString()); xml.WriteAttributeString("t", "inlineStr"); xml.WriteStartElement("is"); xml.WriteElementString("t", text ?? ""); xml.WriteEndElement(); xml.WriteEndElement(); }
        private static void Numeric(XmlWriter xml, string address, decimal value, string formula = null) { xml.WriteStartElement("c"); xml.WriteAttributeString("r", address); xml.WriteAttributeString("s", "3"); if (formula != null) xml.WriteElementString("f", formula); xml.WriteElementString("v", value.ToString(CultureInfo.InvariantCulture)); xml.WriteEndElement(); }
        private static void TextRow(XmlWriter xml, int row, string text, int style = 0) { xml.WriteStartElement("row"); xml.WriteAttributeString("r", row.ToString()); if (row <= 2) { xml.WriteAttributeString("ht", "26"); xml.WriteAttributeString("customHeight", "1"); } Cell(xml, "A" + row, text, style); xml.WriteEndElement(); }
        private static void SummaryRow(XmlWriter xml, int row, string label, decimal value) { xml.WriteStartElement("row"); xml.WriteAttributeString("r", row.ToString()); Cell(xml, "D" + row, label); Numeric(xml, "G" + row, value); xml.WriteEndElement(); }
        private static string Date(JToken token) { return token.Value<DateTime>().ToString("dd/MM/yyyy"); }
        private static string Kind(string kind) { switch (kind) { case "outputs": return "Phiếu xuất"; case "returns": return "Hàng trả lại"; case "receipts": return "Phiếu thu"; case "payments": return "Phiếu chi"; default: return kind; } }
    }
}
