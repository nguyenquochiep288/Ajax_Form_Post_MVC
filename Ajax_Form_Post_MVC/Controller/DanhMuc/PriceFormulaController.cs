using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC_QuanLyTHP.Controllers
{
    public class PriceFormulaController : Controller
    {
        public ActionResult Index(string Search = "")
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "View")) return RedirectToAction("Index", "Notfound");
            ApiResponse result = Utility.GetListData<PriceFormulaDto>("PriceFormula", "", "", Utility.LOC_ID);
            var rows = result.Success ? result.Data as List<PriceFormulaDto> : new List<PriceFormulaDto>();
            rows = rows ?? new List<PriceFormulaDto>();
            if (!String.IsNullOrWhiteSpace(Search)) rows = rows.Where(x => (x.NAME ?? "").IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 || (x.MA ?? "").IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 || (x.TEN_NHOMHANGHOAS ?? x.TEN_NHOMHANGHOA ?? "").IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            ViewBag.LoadError = result.Success ? null : "Không tải được danh sách công thức. Hãy chạy script SQL PriceFormula và kiểm tra kết nối API.";
            ViewBag.PermissionCreate = Utility.KiemTraQuyen("PriceFormula", "Create");
            ViewBag.PermissionEdit = Utility.KiemTraQuyen("PriceFormula", "Edit");
            ViewBag.PermissionDelete = Utility.KiemTraQuyen("PriceFormula", "Delete");
            return View(new PriceFormulaPageModel { Formulas = rows, Search = Search });
        }

        public ActionResult History()
        {
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "View")) return RedirectToAction("Index", "Notfound");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> HistoryData(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "View"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền xem báo cáo không hợp lệ." });
            try
            {
                var request = JObject.Parse(payload ?? "{}");
                request["LOC_ID"] = Utility.LOC_ID;
                return await ForwardBatchRequest("/PriceFormula/HistoryReport", request);
            }
            catch { return JsonContent(new { Success = false, Message = "Không tải được báo cáo. Vui lòng kiểm tra kết nối API và thử lại." }); }
        }

        public ActionResult CalculatePrices()
        {
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "View")) return RedirectToAction("Index", "Notfound");
            ApiResponse groupsResponse = Utility.GetListData<PriceFormulaGroupDto>("GroupProduct", "", "", Utility.LOC_ID);
            var groups = groupsResponse.Success ? groupsResponse.Data as List<PriceFormulaGroupDto> : new List<PriceFormulaGroupDto>();
            return View("CalculatePrices", new PriceCalculationRunModel
            {
                LOC_ID = Utility.LOC_ID,
                Groups = groups ?? new List<PriceFormulaGroupDto>(),
                CanApply = Utility.KiemTraQuyen("PriceFormula", "Edit")
            });
        }

        public ActionResult ComparePrices()
        {
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "View")) return RedirectToAction("Index", "Notfound");
            ApiResponse groupsResponse = Utility.GetListData<PriceFormulaGroupDto>("GroupProduct", "", "", Utility.LOC_ID);
            var groups = groupsResponse.Success ? groupsResponse.Data as List<PriceFormulaGroupDto> : new List<PriceFormulaGroupDto>();
            return View(new PriceCalculationRunModel { LOC_ID = Utility.LOC_ID, Groups = groups ?? new List<PriceFormulaGroupDto>() });
        }

        [HttpPost]
        public async Task<ActionResult> ComparePricesData(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "View"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền truy cập không hợp lệ." });
            try
            {
                var request = JsonConvert.DeserializeObject<PriceComparisonRequest>(payload ?? "{}");
                if (request == null) return JsonContent(new { Success = false, Message = "Điều kiện đối chiếu không hợp lệ." });
                request.LOC_ID = Utility.LOC_ID;
                request.ITEMS = new List<PriceCalculationBatchItem>();
                request.DRAFT_FORMULA = null;
                return await ForwardBatchRequest("/PriceFormula/ComparePrices", request);
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        [HttpPost]
        public async Task<ActionResult> PreviewBatch(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "View"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền truy cập không hợp lệ." });
            try
            {
                var request = JsonConvert.DeserializeObject<PriceCalculationBatchRequest>(payload ?? "{}");
                if (request == null) return JsonContent(new { Success = false, Message = "Dữ liệu xem trước không hợp lệ." });
                request.LOC_ID = Utility.LOC_ID;
                if (request.DRAFT_FORMULA != null) request.DRAFT_FORMULA.LOC_ID = Utility.LOC_ID;
                return await ForwardBatchRequest("/PriceFormula/PreviewBatch", request);
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        [HttpPost]
        public async Task<ActionResult> CalculationProducts(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "View"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền truy cập không hợp lệ." });
            try
            {
                var request = JsonConvert.DeserializeObject<PriceCalculationBatchRequest>(payload ?? "{}");
                if (request == null) return JsonContent(new { Success = false, Message = "Phạm vi không hợp lệ." });
                request.LOC_ID = Utility.LOC_ID;
                request.DRAFT_FORMULA = null;
                return await ForwardBatchRequest("/PriceFormula/CalculationProducts", request);
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ApplyBatch(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "Edit"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền cập nhật không hợp lệ." });
            try
            {
                var request = JsonConvert.DeserializeObject<PriceCalculationApplyRequest>(payload ?? "{}");
                if (request == null) return JsonContent(new { Success = false, Message = "Dữ liệu áp dụng giá không hợp lệ." });
                request.LOC_ID = Utility.LOC_ID;
                return await ForwardBatchRequest("/PriceFormula/ApplyBatch", request);
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        [HttpGet]
        public async Task<ActionResult> LatestAppliedBatch()
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "Edit"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền cập nhật không hợp lệ." });
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Utility.Token);
                    var response = await client.GetAsync(Utility.URL.TrimEnd('/') + "/PriceFormula/LatestAppliedBatch/" + Uri.EscapeDataString(Utility.LOC_ID));
                    var body = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        return JsonContent(new { Success = false, Message = "API trả về lỗi HTTP " + (int)response.StatusCode + "." });
                    return Content(body, "application/json", Encoding.UTF8);
                }
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> RestoreBatch(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "Edit"))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền cập nhật không hợp lệ." });
            try
            {
                var request = JsonConvert.DeserializeObject<PriceCalculationRestoreRequest>(payload ?? "{}");
                if (request == null) return JsonContent(new { Success = false, Message = "Dữ liệu khôi phục giá không hợp lệ." });
                request.LOC_ID = Utility.LOC_ID;
                return await ForwardBatchRequest("/PriceFormula/RestoreBatch", request);
            }
            catch (Exception ex) { return JsonContent(new { Success = false, Message = ex.Message }); }
        }

        private async Task<ActionResult> ForwardBatchRequest(string endpoint, object request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Utility.Token);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(Utility.URL.TrimEnd('/') + endpoint, content);
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return JsonContent(new { Success = false, Message = "API trả về lỗi HTTP " + (int)response.StatusCode + "." });
                return Content(body, "application/json", Encoding.UTF8);
            }
        }

        private ActionResult JsonContent(object value)
        {
            return Content(JsonConvert.SerializeObject(value), "application/json", Encoding.UTF8);
        }

        public ActionResult Create()
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "Create")) return RedirectToAction("Index", "Notfound");
            var model = new PriceFormulaEditModel { LOC_ID = Utility.LOC_ID, MA = "CTG-" + DateTime.Now.ToString("yyMMddHHmmss"), TUNGAY = DateTime.Today, LOAIGIA = "GIA01", CONGTHUC = "" };
            LoadGroups(model);
            return View("Edit", model);
        }

        [HttpGet]
        public ActionResult Copy(string id)
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "Create") || !Utility.KiemTraQuyen("PriceFormula", "View")) return RedirectToAction("Index", "Notfound");
            ApiResponse response = Utility.GetDetail<PriceFormulaDto>(Utility.LOC_ID + "/" + id, "PriceFormula");
            var item = response.Success ? response.Data as PriceFormulaDto : null;
            if (item == null) { TempData["TitleError"] = response.Message ?? "Không tìm thấy công thức giá."; return RedirectToAction("Index"); }
            var groups = new List<string>(item.ID_NHOMHANGHOAS ?? new List<string>());
            if (groups.Count == 0 && !String.IsNullOrWhiteSpace(item.ID_NHOMHANGHOA)) groups.Add(item.ID_NHOMHANGHOA);
            var model = new PriceFormulaEditModel {
                LOC_ID = Utility.LOC_ID, MA = "CTG-" + DateTime.Now.ToString("yyMMddHHmmssfff"),
                NAME = ((item.NAME ?? "").Length > 190 ? item.NAME.Substring(0, 190) : item.NAME) + " (bản sao)",
                ID_NHOMHANGHOA = item.ID_NHOMHANGHOA, ID_NHOMHANGHOAS = groups,
                LOAIGIA = item.LOAIGIA, CONGTHUC = item.CONGTHUC, TINHTHUE = item.TINHTHUE,
                TUNGAY = item.TUNGAY, DENNGAY = item.DENNGAY, ISACTIVE = false, IsEdit = false,
                Components = (item.Components ?? new List<PriceFormulaComponentDto>()).Select(x => new PriceFormulaComponentDto {
                    MA = x.MA, TEN = x.TEN, KIEUGIATRI = x.KIEUGIATRI, GIATRI_MACDINH = x.GIATRI_MACDINH, STT = x.STT
                }).ToList()
            };
            ViewBag.CopyNotice = "Đây là bản sao chưa lưu, có mã mới. Kiểm tra nhóm hàng và thời hạn trước khi bật Đang áp dụng để tránh trùng hiệu lực với công thức gốc.";
            LoadGroups(model);
            return View("Edit", model);
        }

        public ActionResult Edit(string id)
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "Edit")) return RedirectToAction("Index", "Notfound");
            ApiResponse response = Utility.GetDetail<PriceFormulaDto>(Utility.LOC_ID + "/" + id, "PriceFormula");
            var item = response.Success ? response.Data as PriceFormulaDto : null;
            if (item == null) { TempData["TitleError"] = response.Message; return RedirectToAction("Index"); }
            var selectedGroups = item.ID_NHOMHANGHOAS ?? new List<string>();
            if (selectedGroups.Count == 0 && !String.IsNullOrWhiteSpace(item.ID_NHOMHANGHOA)) selectedGroups.Add(item.ID_NHOMHANGHOA);
            var model = new PriceFormulaEditModel { ID = item.ID, LOC_ID = item.LOC_ID, MA = item.MA, NAME = item.NAME, ID_NHOMHANGHOA = item.ID_NHOMHANGHOA, ID_NHOMHANGHOAS = selectedGroups, LOAIGIA = item.LOAIGIA, CONGTHUC = item.CONGTHUC, TINHTHUE = item.TINHTHUE, TUNGAY = item.TUNGAY, DENNGAY = item.DENNGAY, ISACTIVE = item.ISACTIVE, Components = item.Components ?? new List<PriceFormulaComponentDto>(), IsEdit = true };
            LoadGroups(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(PriceFormulaEditModel model, string draftPayload = null)
        {
            if (Request.IsAjaxRequest() && draftPayload != null)
            {
                try { model = JsonConvert.DeserializeObject<PriceFormulaEditModel>(draftPayload); }
                catch (JsonException) { return JsonContent(new { Success = false, Message = "Giá trị công thức không hợp lệ." }); }
                if (model == null) return JsonContent(new { Success = false, Message = "Thiếu thông tin công thức." });
            }
            bool editing = !String.IsNullOrEmpty(model.ID);
            string permission = editing ? "Edit" : "Create";
            if (Request.IsAjaxRequest() && (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", permission)))
                return JsonContent(new { Success = false, Message = "Phiên đăng nhập hoặc quyền lưu công thức không hợp lệ." });
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", permission)) return RedirectToAction("Index", "Notfound");
            model.LOC_ID = Utility.LOC_ID;
            model.Components = model.Components ?? new List<PriceFormulaComponentDto>();
            ApiResponse response = editing
                ? Utility.Edit(Utility.LOC_ID + "/" + model.ID, ToRequest(model), "PriceFormula")
                : Utility.Create(ToRequest(model), "PriceFormula");
            if (Request.IsAjaxRequest()) return JsonContent(response);
            if (response.Success) return RedirectToAction("Index");
            ModelState.AddModelError("", response.Message);
            model.IsEdit = editing;
            LoadGroups(model);
            return View("Edit", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "Delete")) return RedirectToAction("Index", "Notfound");
            ApiResponse response = Utility.Delete<PriceFormulaDto>(Utility.LOC_ID + "/" + id, "PriceFormula");
            TempData[response.Success ? "SuccessMessage" : "TitleError"] = response.Success ? "Đã ngưng áp dụng công thức giá." : response.Message;
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Apply(string id)
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("PriceFormula", "Edit")) return RedirectToAction("Index", "Notfound");

            ApiResponse response = Utility.GetDetail<PriceFormulaDto>(Utility.LOC_ID + "/" + id, "PriceFormula");
            var item = response.Success ? response.Data as PriceFormulaDto : null;
            if (item == null)
            {
                TempData["TitleError"] = response.Message ?? "Không tìm thấy công thức giá.";
                return RedirectToAction("Index");
            }

            var selectedGroups = item.ID_NHOMHANGHOAS ?? new List<string>();
            if (selectedGroups.Count == 0 && !String.IsNullOrWhiteSpace(item.ID_NHOMHANGHOA)) selectedGroups.Add(item.ID_NHOMHANGHOA);
            var model = new PriceFormulaEditModel
            {
                ID = item.ID,
                LOC_ID = Utility.LOC_ID,
                MA = item.MA,
                NAME = item.NAME,
                ID_NHOMHANGHOA = item.ID_NHOMHANGHOA,
                ID_NHOMHANGHOAS = selectedGroups,
                LOAIGIA = item.LOAIGIA,
                CONGTHUC = item.CONGTHUC,
                TINHTHUE = item.TINHTHUE,
                TUNGAY = item.TUNGAY,
                DENNGAY = item.DENNGAY,
                ISACTIVE = true,
                Components = item.Components ?? new List<PriceFormulaComponentDto>()
            };

            ApiResponse update = Utility.Edit(Utility.LOC_ID + "/" + item.ID, ToRequest(model), "PriceFormula");
            TempData[update.Success ? "SuccessMessage" : "TitleError"] = update.Success ? "Đã áp dụng công thức giá." : update.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> Preview(PriceFormulaPreviewModel model)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("PriceFormula", "View"))
                return Json(new { success = false, message = "Phiên đăng nhập hoặc quyền truy cập không hợp lệ." });
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Utility.Token);
                    var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(Utility.URL.TrimEnd('/') + "/PriceFormula/Calculate", content);
                    var body = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var success = (bool?)(body["Success"] ?? body["success"]) == true;
                    var message = (string)(body["Message"] ?? body["message"]);
                    var data = body["Data"] ?? body["data"];
                    if (!response.IsSuccessStatusCode && String.IsNullOrWhiteSpace(message))
                        message = "API trả về lỗi HTTP " + (int)response.StatusCode + ".";
                    var beforeTax = FindDecimalValue(data ?? body, "GIA_TRUOC_THUE");
                    var afterTax = FindDecimalValue(data ?? body, "GIA_SAU_THUE");
                    if (success && (!beforeTax.HasValue || !afterTax.HasValue))
                        return Json(new { success = false, message = "API tính công thức thành công nhưng phản hồi thiếu giá trước thuế hoặc giá sau thuế." });
                    var normalizedData = beforeTax.HasValue && afterTax.HasValue
                        ? new { GIA_TRUOC_THUE = beforeTax.Value, GIA_SAU_THUE = afterTax.Value }
                        : null;
                    return Json(new { success, message, data = normalizedData });
                }
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        private static decimal? FindDecimalValue(JToken token, string propertyName)
        {
            var obj = token as JObject;
            if (obj != null)
            {
                var property = obj.Properties().FirstOrDefault(x => String.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                decimal value;
                if (property != null && Decimal.TryParse(property.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    return value;
                foreach (var child in obj.Properties())
                {
                    var nestedValue = FindDecimalValue(child.Value, propertyName);
                    if (nestedValue.HasValue) return nestedValue;
                }
            }
            var array = token as JArray;
            if (array != null)
            {
                foreach (var child in array)
                {
                    var nestedValue = FindDecimalValue(child, propertyName);
                    if (nestedValue.HasValue) return nestedValue;
                }
            }
            return null;
        }

        private void LoadGroups(PriceFormulaEditModel model)
        {
            ApiResponse response = Utility.GetListData<PriceFormulaGroupDto>("GroupProduct", "", "", Utility.LOC_ID);
            model.Groups = response.Success ? response.Data as List<PriceFormulaGroupDto> : new List<PriceFormulaGroupDto>();
            model.Groups = model.Groups ?? new List<PriceFormulaGroupDto>();
        }

        private static object ToRequest(PriceFormulaEditModel model)
        {
            var groupIds = model.ID_NHOMHANGHOAS ?? new List<string>();
            var legacyGroupId = groupIds.FirstOrDefault();
            return new { LOC_ID = model.LOC_ID, model.MA, model.NAME, ID_NHOMHANGHOA = legacyGroupId, ID_NHOMHANGHOAS = groupIds, model.LOAIGIA, model.CONGTHUC, model.TINHTHUE, model.TUNGAY, model.DENNGAY, model.ISACTIVE,
                Components = model.Components.Select((x, i) => new { x.MA, x.TEN, x.KIEUGIATRI, x.GIATRI_MACDINH, STT = i + 1 }).ToList() };
        }
    }
}
