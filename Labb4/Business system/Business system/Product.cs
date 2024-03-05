using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	internal class Product
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int Qty {  get; set; }

		protected Product(int ID, string Name, int Price, int Qty)
		{
			this.ID = ID;
			this.Name = Name;
			this.Price = Price;
			this.Qty = Qty;
		}
		protected Product(int ID, string Name, int Price)
		{
			this.ID = ID;
			this.Name = Name;
			this.Price = Price;
			this.Qty = 0;
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
		
	}
}
