using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal class Videogame : Product
	{
		public string Platform { get; set; }
		
		public Videogame() : base() {
			ProductType = Business_system.ProductType.Videogame;
		}

		public override string[] ToCsv()
		{
			var csv_s = base.ToCsv().ToList();
			csv_s.AddRange(new string[] { 
				Platform
			});
			return csv_s.ToArray();
		}
	}
}
