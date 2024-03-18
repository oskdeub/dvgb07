using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal interface ICsvSerializable
	{
		string[] ToCsv();
	}
}
