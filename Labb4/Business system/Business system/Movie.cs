using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	
	internal class Movie : Product
	{
		public MovieFormat MovieFormat { get; set; }
		public int Playtime { get; set; }
		public Movie() : base() { }
	}
}
