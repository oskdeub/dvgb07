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
	}
}
