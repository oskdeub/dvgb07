using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal class Videogame : Product
	{
		public VideogamePlatform Platform { get; set; }
		
		public Videogame() : base() { }

		public override string[] ToCsv()
		{
			var csv_s = base.ToCsv().ToList();
			csv_s.AddRange(new string[] { ProductType.Videogame.ToString(), Platform.ToString() });
			return csv_s.ToArray();
		}
	}
}
