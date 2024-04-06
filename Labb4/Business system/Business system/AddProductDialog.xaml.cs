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
		public AddProductDialog()
		{
			this.InitializeComponent();
			BookFormatComboBox.ItemsSource = Enum.GetValues(typeof(BookFormat));
			BookLanguageComboBox.ItemsSource = Enum.GetValues(typeof (BookLanguage));
			MovieFormatComboBox.ItemsSource = Enum.GetValues(typeof(MovieFormat));
			PlatformComboBox.ItemsSource = Enum.GetValues(typeof(VideogamePlatform));
		}
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
		private bool isNegative(TextBox textBox)
		{
			if (textBox.Text != string.Empty)
			{
				if (int.TryParse(textBox.Text, out int result))
				{
					if (result < 0)
					{
						textBox.Text = "";
						textBox.PlaceholderText = "Input must be a positive number!";
						textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
						return true;
					}
				}
			}
			return false;
		}
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

		
		/* Referens: ChatGPT */
		private string FindTextBoxValueByTag(string tag)
		{
			//Because we're creating dynamic TextBoxes and applying Tags we can find these by searching in DynamicPanel.Children
			var textBox = DynamicPanel.Children.OfType<TextBox>().FirstOrDefault(tb => tb.Tag?.ToString() == tag);
			return textBox?.Text;
		}
		private T FindComboBoxValueByTag<T>(string tag) where T : Enum
		{
			var comboBox = DynamicPanel.Children.OfType<ComboBox>().FirstOrDefault(cb => cb.Tag?.ToString() == tag);
			if (comboBox?.SelectedItem != null)
			{
				return (T)comboBox.SelectedItem;
			}
			return default;
		}
		/* Slut referens */
		
	

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

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
		private void AddBookFields()
		{
			var authorTextBox = new TextBox { Header = "Author", Tag = "Author" };
			var genreTextBox = new TextBox { Header = "Genre", Tag = "Genre" };
			var formatComboBox = new ComboBox { Header = "Format", Tag = "BookFormat" };
			var languageComboBox = new ComboBox { Header = "Language", Tag = "Language" };
			foreach (var format in Enum.GetValues(typeof(BookFormat)))
			{
				formatComboBox.Items.Add(format);
			}

			foreach (var language in Enum.GetValues(typeof(BookLanguage)))
			{
				languageComboBox.Items.Add(language);
			}

			DynamicPanel.Children.Add(authorTextBox);
			DynamicPanel.Children.Add(genreTextBox);
			DynamicPanel.Children.Add(formatComboBox);
			DynamicPanel.Children.Add(languageComboBox);
		}
		private void AddMovieFields()
		{
			var playtimeTextBox = new TextBox { Header = "Playtime", Tag = "Playtime" };

			var formatComboBox = new ComboBox { Header = "Format", Tag = "MovieFormat" };
			foreach (var format in Enum.GetValues(typeof(MovieFormat)))
			{
				formatComboBox.Items.Add(format);
			}

			DynamicPanel.Children.Add(playtimeTextBox);
			DynamicPanel.Children.Add(formatComboBox);
		}
		private void AddVideogameFields()
		{
			var platformComboBox = new ComboBox { Header = "Platform", Tag = "VideogamePlatform" };
			foreach (var platform in Enum.GetValues(typeof(VideogamePlatform)))
			{
				platformComboBox.Items.Add(platform);
			}
			DynamicPanel.Children.Add(platformComboBox);
		}
	}
}
