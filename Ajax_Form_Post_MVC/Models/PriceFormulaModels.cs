using System;
using System.Collections.Generic;

namespace MVC_QuanLyTHP.Models
{
    public class PriceFormulaPageModel
    {
        public List<PriceFormulaDto> Formulas { get; set; } = new List<PriceFormulaDto>();
        public string Search { get; set; }
    }

    public class PriceFormulaEditModel
    {
        public string ID { get; set; }
        public string LOC_ID { get; set; }
        public string MA { get; set; }
        public string NAME { get; set; }
        public string ID_NHOMHANGHOA { get; set; }
        public List<string> ID_NHOMHANGHOAS { get; set; } = new List<string>();
        public string LOAIGIA { get; set; }
        public string CONGTHUC { get; set; }
        public bool TINHTHUE { get; set; }
        public DateTime TUNGAY { get; set; } = DateTime.Today;
        public DateTime? DENNGAY { get; set; }
        public bool ISACTIVE { get; set; } = true;
        public List<PriceFormulaComponentDto> Components { get; set; } = new List<PriceFormulaComponentDto>();
        public List<PriceFormulaGroupDto> Groups { get; set; } = new List<PriceFormulaGroupDto>();
        public bool IsEdit { get; set; }
    }

    public class PriceFormulaDto
    {
        public string LOC_ID { get; set; }
        public string ID { get; set; }
        public string MA { get; set; }
        public string NAME { get; set; }
        public string ID_NHOMHANGHOA { get; set; }
        public string TEN_NHOMHANGHOA { get; set; }
        public List<string> ID_NHOMHANGHOAS { get; set; } = new List<string>();
        public string TEN_NHOMHANGHOAS { get; set; }
        public string LOAIGIA { get; set; }
        public string CONGTHUC { get; set; }
        public bool TINHTHUE { get; set; }
        public DateTime TUNGAY { get; set; }
        public DateTime? DENNGAY { get; set; }
        public bool ISACTIVE { get; set; }
        public List<PriceFormulaComponentDto> Components { get; set; } = new List<PriceFormulaComponentDto>();
    }

    public class PriceFormulaComponentDto
    {
        public string LOC_ID { get; set; }
        public string ID { get; set; }
        public string ID_CONGTHUCGIA { get; set; }
        public string MA { get; set; }
        public string TEN { get; set; }
        public string KIEUGIATRI { get; set; } = "TIEN";
        public decimal GIATRI_MACDINH { get; set; }
        public int STT { get; set; }
    }

    public class PriceFormulaGroupDto
    {
        public string ID { get; set; }
        public string MA { get; set; }
        public string NAME { get; set; }
        public decimal MUCTHUE { get; set; }
    }

    public class PriceFormulaPreviewModel
    {
        public string CONGTHUC { get; set; }
        public Dictionary<string, decimal> GIA_TRI { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        public bool TINHTHUE { get; set; }
        public decimal THUE_SUAT { get; set; }
    }

    public class PriceCalculationRunModel
    {
        public bool Embedded { get; set; }
        public string LOC_ID { get; set; }
        public List<PriceFormulaGroupDto> Groups { get; set; } = new List<PriceFormulaGroupDto>();
        public bool CanApply { get; set; }
    }

    public class PriceCalculationBatchRequest
    {
        public bool USE_EXCEL_PURCHASE_PRICE { get; set; }
        public string LOC_ID { get; set; }
        public string LOAIGIA { get; set; } = "GIA01";
        public DateTime NGAYTINH { get; set; } = DateTime.Today;
        public List<string> ID_NHOMHANGHOAS { get; set; } = new List<string>();
        public List<PriceCalculationBatchItem> ITEMS { get; set; } = new List<PriceCalculationBatchItem>();
        public PriceFormulaDto DRAFT_FORMULA { get; set; }
        public string ID_CONGTHUCGIA { get; set; }
        public string SEARCH { get; set; }
    }

    public class PriceComparisonRequest : PriceCalculationBatchRequest
    {
        public int PAGE { get; set; } = 1;
        public bool ONLY_BELOW { get; set; }
    }

    public class PriceCalculationBatchItem
    {
        public string MAHANG { get; set; }
        public string TEN_HANG { get; set; }
        public decimal GIAHIENTAI { get; set; }
    }

    public class PriceCalculationApplyRequest : PriceCalculationBatchRequest
    {
        public List<string> MAHANG_APDUNG { get; set; } = new List<string>();
        public Dictionary<string, decimal> GIA_MOI_APDUNG { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
    }

    public class PriceCalculationRestoreRequest
    {
        public string LOC_ID { get; set; }
        public List<string> HISTORY_IDS { get; set; } = new List<string>();
    }
}
