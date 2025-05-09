using DatabaseTHP.Class;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Class
{
    public class ComboboxFrom
    {
        public string ID { get; set; }
        public string MA { get; set; }
        public string NAME { get; set; }
        public Boolean ISACTIVE { get; set; }
        public Boolean ISDEFAULT { get; set; }

        public Double? LATITUDE { get; set; }

        public Double? LONGITUDE { get; set; }

        public Double KHOANGCACH {
            get 
            {
                var khoangcach = API.CalculateDistance(Utility.Latitude, Utility.Longitude, LATITUDE ?? 0, LONGITUDE ?? 0);
                return khoangcach;
            }
        }
    }
}