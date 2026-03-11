using DatabaseTHP.Class;
using MVC_QuanLyTHP.Controllers;

namespace MVC_QuanLyTHP.Class
{

	public class ComboboxFrom
	{
		public string ID { get; set; }

		public string MA { get; set; }

		public string NAME { get; set; }

		public bool ISACTIVE { get; set; }

		public bool ISDEFAULT { get; set; }

		public double? LATITUDE { get; set; }

		public double? LONGITUDE { get; set; }

		public double KHOANGCACH => API.CalculateDistance(Utility.Latitude, Utility.Longitude, LATITUDE.GetValueOrDefault(), LONGITUDE.GetValueOrDefault());
	}
}
