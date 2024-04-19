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
	public sealed partial class ProductInfoDialog : ContentDialog
	{
		Product CurrentProduct;
		public ProductInfoDialog(Product product)
		{
			this.CurrentProduct = product;
			this.InitializeComponent();
			
			DisplayProductFields();
			
			
		}
		/// <summary>
		/// Visar produktinfo baserat på typ av produkt.
		/// </summary>
		private void DisplayProductFields()
		{
			BookPanel.Visibility = Visibility.Collapsed;
			MoviePanel.Visibility = Visibility.Collapsed;
			VideogamePanel.Visibility = Visibility.Collapsed;
			PopulateCommonInfo();
			if (CurrentProduct is Book book) {
				BookPanel.Visibility = Visibility.Visible;
				PopulateBookInfo(book);
			} else if (CurrentProduct is Movie movie)
			{
				MoviePanel.Visibility = Visibility.Visible;
				PopulateMovieInfo(movie);
			} else if (CurrentProduct is Videogame videogame)
			{
				VideogamePanel.Visibility = Visibility.Visible;
				PopulateVideogameInfo(videogame);
			}
				
		}
		/// <summary>
		/// Fyller delad info
		/// </summary>
		private void PopulateCommonInfo()
		{
			NameTextBlock.Text = CurrentProduct.Name;
			TypeTextBlock.Text = CurrentProduct.ProductType.ToString();
			PriceTextBlock.Text = CurrentProduct.Price.ToString();
			QuantityTextBlock.Text = CurrentProduct.Qty.ToString();
		}

		private void PopulateBookInfo(Book book)
		{
			AuthorTextBlock.Text = book.Author;
			BookGenreTextBlock.Text = book.bookGenre;
			BookFormatTextBlock.Text = book.BookFormat.ToString();
			LanguageTextBlock.Text = book.Language.ToString();
		}
		private void PopulateMovieInfo(Movie movie)
		{
			MovieFormatTextBlock.Text = movie.MovieFormat.ToString();
			PlaytimeTextBlock.Text = movie.Playtime.ToString();
		}
		private void PopulateVideogameInfo(Videogame videogame)
		{
			PlatformTextBlock.Text = videogame.Platform.ToString();
		}
		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}
	}
}
