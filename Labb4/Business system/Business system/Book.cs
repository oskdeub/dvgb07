using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal class Book : Product, ICsvSerializable
	{
		public string Author { get; set; }
		public string bookGenre { get; set; }
		public string BookFormat { get; set; }
		public string Language { get; set; }
		public Book() : base() {
			ProductType = Business_system.ProductType.Book;
		}

		public override string[] ToCsv()
		{
			var csv_s = base.ToCsv().ToList();
			csv_s.AddRange(new string[] { 
				Author, 
				bookGenre, 
				BookFormat, 
				Language 
			});
			return csv_s.ToArray();
		}
	}
}
