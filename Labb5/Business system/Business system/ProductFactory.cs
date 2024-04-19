using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal static class ProductFactory
	{
		internal static Product CreateProduct(ProductType productType, int id, string name, int price)
		{
			switch (productType)
			{
				case ProductType.Book:
					return new Book { ID = id, Name = name, Price = price };
				case ProductType.Movie:
					return new Movie { ID = id, Name = name, Price = price };
				case ProductType.Videogame:
					return new Videogame { ID = id, Name = name, Price = price };
				default:
					throw new ArgumentException("No productType listed.");
			}
		}
	}
}
