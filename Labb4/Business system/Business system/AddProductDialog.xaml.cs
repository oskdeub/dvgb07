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
		public AddProductDialog()
		{
			this.InitializeComponent();
		}
		

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DynamicPanel.Children.Clear();

			var type = ProductComboBox.SelectedItem as ComboBoxItem;
			if (type != null)
			{
				switch(type.Content.ToString())
				{
					case "Book":
						AddBookFields();
						break;
					case "Movie":
						AddMovieFields();
						break;
					case "Videogame":
						AddVideogameFields();
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
