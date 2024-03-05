using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal class Book : Product
	{
		public string Author { get; set; }
		public string bookGenre { get; set; }
		public BookFormat BookFormat { get; set; }
		public BookLanguage Language { get; set; }
		public Book() : base() { }
	}
}
