using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business_system
{
	public class Product : ICsvSerializable, INotifyPropertyChanged
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int Qty {  get; set; }
		private string changingProp;
		
		public ProductType? ProductType { get; set; }

		public Product()
		{
		}
		public string ChangingProperty
		{//Referens: ChatGPT
			get => changingProp;
			set
			{
				if (changingProp != value)
				{
					changingProp = value;
					OnPropertyChanged(nameof(ChangingProperty));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}//Slut Referens
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
