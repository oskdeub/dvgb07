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
		public BookFormat BookFormat { get; set; }
		public BookLanguage Language { get; set; }
		public Book() : base() { }

		public override string[] ToCsv()
		{
			var csv_s = base.ToCsv().ToList();
			csv_s.AddRange(new string[] {ProductType.Book.ToString(), Author, bookGenre, BookFormat.ToString(), Language.ToString() });
			return csv_s.ToArray();
		}
	}
}
