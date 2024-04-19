using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Business_system
{
	
	internal class Movie : Product
	{
		public string MovieFormat { get; set; }
		public int? Playtime { get; set; }
		public Movie() : base() {
			ProductType = Business_system.ProductType.Movie;
		}

		public override string[] ToCsv()
		{
			var csv_s = base.ToCsv().ToList();
			csv_s.AddRange(new string[] { 
				MovieFormat, 
				Playtime.ToString() 
			});
			return csv_s.ToArray();
		}
	}
	
}
