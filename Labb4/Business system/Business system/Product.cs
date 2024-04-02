using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	public class Product : ICsvSerializable
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int Qty {  get; set; }
		public ProductType? ProductType { get; set; }

		protected Product(int ID, string Name, int Price, int Qty)
		{
			this.ID = ID;
			this.Name = Name;
			this.Price = Price;
			this.Qty = Qty;
			this.ProductType = null;
		}
		protected Product(int ID, string Name, int Price, ProductType type)
		{
			this.ID = ID;
			this.Name = Name;
			this.Price = Price;
			this.Qty = 0;
			this.ProductType = type;
		}

		protected Product(int ID, string Name, int Price, int Qty, ProductType type)
		{
			this.ID = ID;
			this.Name = Name;
			this.Price = Price;
			this.Qty = Qty;
			this.ProductType = type;
		}

		public Product()
		{
		}

		public void IncreaseQty(int amount)
		{
			//handle negative amount here?
			Qty += amount;
		}
		public void SubtractQty(int amount) {
			//handle negative qty after subtracting here?
			Qty -= amount;
		}

		public virtual string[] ToCsv()
		{
			return new string[]
			{
				ID.ToString(),
				Name,
				Price.ToString(),
				Qty.ToString(),
				ProductType.ToString(),
			};
		}
	}
}
