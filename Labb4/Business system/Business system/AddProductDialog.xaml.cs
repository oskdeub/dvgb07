using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Business_system
{
	public sealed partial class AddProductDialog : ContentDialog
	{
		public Product NewProduct { get; private set; }
		/// <summary>
		/// Instansierar kompnenten och hämtar information från Enums.
		/// </summary>
		public AddProductDialog()
		{
			this.InitializeComponent();
			BookFormatComboBox.ItemsSource = Enum.GetValues(typeof(BookFormat));
			BookLanguageComboBox.ItemsSource = Enum.GetValues(typeof (BookLanguage));
			MovieFormatComboBox.ItemsSource = Enum.GetValues(typeof(MovieFormat));
			PlatformComboBox.ItemsSource = Enum.GetValues(typeof(VideogamePlatform));
		}
		/// <summary>
		/// Huvudknappens händelse, kontrollerar inputfälten men stänger inte dialogen om valideringen inte går igenom.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (!isFormValid())
			{
				args.Cancel = true;
			} else
			{
				NewProduct = ExtractProductInfo();
			}
		}
		/// <summary>
		/// – Kontrollerar alla nödvändiga inputfält och om dess värden är av rätt typer. 
		/// </summary>
		/// <returns></returns>
		private bool isFormValid()
		{
			// Initial form validation: Name, Price given. QTY can be left empty
			
			
			if (isRequiredFieldEmpty(NameTextBox) ||
				isRequiredFieldEmpty(PriceTextBox) ||
				!isFieldPositiveInteger(PriceTextBox) || 
				!isFieldPositiveInteger(QtyTextBox)) 
			{
				return false;
			}

			var selectedProduct = ProductComboBox.SelectedItem as ComboBoxItem;
			if(selectedProduct == null)
			{
				ProductComboBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
				return false;
			}

			string productType = selectedProduct.Content.ToString();
			switch (productType)
			{
				case "Book":
					return true;
				case "Movie":
					//Special case for Movie that has int PlayTime.
					if (!isFieldPositiveInteger(PlaytimeTextBox))
					{
						PlaytimeTextBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
						return false;
					}
					return true;
				case "Videogame":
					return true;
				default:
					return false;
			}
		}
		/// <summary>
		/// Kontrollerar textbox.Text om det är en int och inte mindre än 0 (negativ).
		/// </summary>
		/// <param name="textBox"></param>
		/// <returns></returns>
	
		private bool isFieldPositiveInteger(TextBox textBox)
		{
			if(textBox.Text != string.Empty)
			{
				if (int.TryParse(textBox.Text, out int result))
				{
					if(result < 0)
					{
						textBox.Text = "";
						textBox.PlaceholderText = "Input must be a positive number!";
						textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
						return false;
					}
				} else 
				{
					textBox.Text = "";
					textBox.PlaceholderText = "Input must be an integer!";
					textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
					return false;
				} 
			}
			return true;
		}
			
		/// <summary>
		/// Kontrollerar om ett nödvändigt fält inte är ifyllt
		/// </summary>
		/// <param name="textBox"></param>
		/// <returns></returns>
		private bool isRequiredFieldEmpty(TextBox textBox)
		{
			if (textBox.Text == string.Empty)
			{
				textBox.PlaceholderText = "required";
				textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
				return true;
			}
			textBox.BorderBrush = default;
			return false;
		}
		/// <summary>
		/// – Extraherar först de delade attributen (ID, Name, Price, Qty) via GetCommonProductInfo()
		/// och sedan produkttypsspecifika attribut från respektive subklass.
		/// </summary>
		/// <returns></returns>
		private Product ExtractProductInfo()
		{
			var selectedProduct = ProductComboBox.SelectedItem as ComboBoxItem;
			if (selectedProduct == null) return null;

			string productType = selectedProduct.Content.ToString();
			switch (productType)
			{
				case "Book":
					return ExtractBookInfo();
				case "Movie":
					return ExtractMovieInfo();
				case "Videogame":
					return ExtractVideogameInfo();
				default:
					return null;
			}
		}
		private void GetCommonProductInfo(Product pr)
		{
			pr.Name = NameTextBox.Text;
			if (int.TryParse(PriceTextBox.Text, out int price))
			{
				pr.Price = price;
			} else
			{
				pr.Price = -1;
			}
			//Handles empty Qty field
			if(int.TryParse(QtyTextBox.Text, out int qty))
			{
				pr.Qty = qty;
			} else
			{
				pr.Qty = 0;
			}
			
		}

		private Book ExtractBookInfo()
		{
			var book = new Book();
			GetCommonProductInfo(book);

			book.Author = AuthorTextBox.Text;
			book.bookGenre = GenreTextBox.Text;
			
			if (BookFormatComboBox.SelectedItem is BookFormat selectedBookFormat)
			{
				book.BookFormat = selectedBookFormat;
			} else
			{
				book.BookFormat = null;
			}

			if (BookLanguageComboBox.SelectedItem is BookLanguage selectedLanguage)
			{
				book.Language = selectedLanguage;
			}
			else
			{
				book.Language = null;
			}

			return book;
		}
		
		private Movie ExtractMovieInfo()
		{
			var movie = new Movie();
			GetCommonProductInfo(movie);
			
			if(int.TryParse(PlaytimeTextBox.Text, out var playtime))
			{
				movie.Playtime = playtime;
			} else
			{
				movie.Playtime = null;
			}

			if (MovieFormatComboBox.SelectedItem is MovieFormat selectedFormat)
			{
				movie.MovieFormat = selectedFormat;
			}
			else
			{
				movie.MovieFormat = null;
			}

			return movie;
		}

		private Videogame ExtractVideogameInfo()
		{
			var videogame = new Videogame();
			GetCommonProductInfo(videogame);

			if(PlatformComboBox.SelectedItem is VideogamePlatform selectedPlatform)
			{
				videogame.Platform = selectedPlatform;
			} else
			{
				videogame.Platform = null;
			}

			return videogame;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
		/// <summary>
		/// Visar de textboxar som hör till vald produkttyp, och gömmer de andra.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AuthorTextBox.Visibility = Visibility.Collapsed;
			GenreTextBox.Visibility = Visibility.Collapsed;
			BookFormatComboBox.Visibility = Visibility.Collapsed;
			BookLanguageComboBox.Visibility = Visibility.Collapsed;
			PlaytimeTextBox.Visibility = Visibility.Collapsed;
			MovieFormatComboBox.Visibility = Visibility.Collapsed;
			PlatformComboBox.Visibility = Visibility.Collapsed;

			var type = ProductComboBox.SelectedItem as ComboBoxItem;
			if (type != null)
			{
				switch(type.Content.ToString())
				{
					case "Book":
						//AddBookFields();
						AuthorTextBox.Visibility = Visibility.Visible;
						GenreTextBox.Visibility = Visibility.Visible;
						BookFormatComboBox.Visibility = Visibility.Visible;
						BookLanguageComboBox.Visibility = Visibility.Visible;
						break;
					case "Movie":
						//AddMovieFields();
						PlaytimeTextBox.Visibility = Visibility.Visible;
						MovieFormatComboBox.Visibility = Visibility.Visible;
						break;
					case "Videogame":
						//AddVideogameFields();
						PlatformComboBox.Visibility = Visibility.Visible;
						break;
				}
			}
			
		}
	}
}
