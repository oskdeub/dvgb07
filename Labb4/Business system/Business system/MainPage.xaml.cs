using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Business_system
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		List<Product> masterProducts = new List<Product>();
		List<Product> deliveryProducts = new List<Product>();
		List<Book> bookList = new List<Book>();
		List<Movie> movieList = new List<Movie>();
		List<Videogame> videogameList = new List<Videogame>();
		List<Product> cartProducts = new List<Product>();
		StorageFile CSVFile = null;
		uint TotalPrice;
		
		private int id_counter;
		public MainPage()
		{
			TotalPrice = 0;
			id_counter = 0;
			this.InitializeComponent();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			await openFile();
			if(CSVFile != null)
			{
				List<string[]> data = await ReadCSVFile(CSVFile);
				populateList(data);
			} else
			{
				await ErrorDialog("CSV-file is null.");
			}
			
		}

		private void updateMasterProductsList()
		{
			id_counter = masterProducts.Count();
			var displayItems = new ObservableCollection<Product>(masterProducts);
			ProductList.ItemsSource = displayItems;
			WriteToFile();
		}
		public void populateList(List<string[]> data) {
			//Clear before reading
			masterProducts.Clear();
			//Add allez producten por favor
			masterProducts.AddRange(parseCsvData(data));

			updateMasterProductsList();
			updateSubclassLists();
		}

		internal List<Product> parseCsvData(List<string[]> data)
		{
			var products = new List<Product>();
			foreach (var line in data.Skip(1))
			{
				products.Add(parseProductFromCsvLine(line));
			}
			return products;
		}

		private Product parseProductFromCsvLine(string[] line)
		{
			//Handle empty fields
			switch (line[4])
			{
				case "Book":
					Book book = new Book();
					book.ID = int.Parse(line[0]);
					book.Name = line[1];
					book.Price = int.Parse(line[2]);
					book.Qty = int.Parse(line[3]);
					book.ProductType = ProductType.Book;
					book.Author = line[5];
					book.bookGenre = line[6];
					if (Enum.TryParse<BookFormat>(line[7], out BookFormat bookFormat))
					{
						book.BookFormat = bookFormat;
					} else
					{
						book.BookFormat = null;
					}
					if (Enum.TryParse<BookLanguage>(line[8], out BookLanguage language))
					{
						book.Language = language;
					} else
					{
						book.Language = null;
					}
					return book;

				case "Movie":
					Movie movie = new Movie();
					movie.ID = int.Parse(line[0]);
					movie.Name = line[1];
					movie.Price = int.Parse(line[2]);
					movie.Qty = int.Parse(line[3]);
					movie.ProductType = ProductType.Movie;
					if (Enum.TryParse<MovieFormat>(line[5], out MovieFormat movieFormat))
					{
						movie.MovieFormat = movieFormat;
					} else
					{
						movie.MovieFormat = null;
					}
					movie.Playtime = int.Parse(line[6]);
					return movie;

				case "Videogame":
					Videogame videogame = new Videogame();
					videogame.ID = int.Parse(line[0]);
					videogame.Name = line[1];
					videogame.Price = int.Parse(line[2]);
					videogame.Qty = int.Parse(line[3]);
					videogame.ProductType = ProductType.Videogame;
					if (Enum.TryParse<VideogamePlatform>(line[5], out VideogamePlatform videogamePlatform))
					{
						videogame.Platform = videogamePlatform;
					} else
					{
						videogame.Platform = null;
					}
					return videogame;

				default:
					Product product = new Product();
					product.ID = int.Parse(line[0]);
					product.Name = line[1];
					product.Price = int.Parse(line[2]);
					product.Qty = int.Parse(line[3]);
					return product;

			}
		}
		private async Task openFile()
		{
			FileOpenPicker fileOpenPicker = new FileOpenPicker
			{
				ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			fileOpenPicker.FileTypeFilter.Add(".csv");
			CSVFile = await fileOpenPicker.PickSingleFileAsync();
		}
		public async Task<List<string[]>> ReadCSVFile(StorageFile file)
		{
			List<string[]> data = new List<string[]>();
			IList<string> lines = await FileIO.ReadLinesAsync(file);

			foreach (var line in lines)
			{
				string[] parts = line.Split(';');
				data.Add(parts);
			}
			return data;
		}
		
		private async void WriteToFile()
		{
			await WriteToCsv(masterProducts);
		}
		private async Task ReadFromFile()
		{
			List<string[]> data = await ReadCSVFile(CSVFile);
			populateList(data);
		}
		internal async Task WriteToCsv(List<Product> products)
		{
			StringBuilder content = new StringBuilder();
			content.AppendLine("ID; Name; Price; Qty; Product;");
			foreach (var product in products)
			{
				content.AppendLine(string.Join(";", product.ToCsv()));
			}
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			
			await FileIO.WriteTextAsync(CSVFile, content.ToString());
		}
		private async void Read_Click(object sender, RoutedEventArgs e)
		{
			await ReadFromFile();
		}

		private async void Save_Click(object sender, RoutedEventArgs e)
		{
			await WriteToCsv(masterProducts);
		}

		private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (args.SelectedItem is NavigationViewItem selectedItem)
			{
				// Hide all panels
				KassaPanel.Visibility = Visibility.Collapsed;
				LagerPanel.Visibility = Visibility.Collapsed;

				// Show the selected panel
				switch (selectedItem.Tag.ToString())
				{
					case "Kassa":
						KassaPanel.Visibility = Visibility.Visible;
						TotalPrice = 0;
						updateSubclassLists();
						break;
					case "Lager":
						LagerPanel.Visibility = Visibility.Visible;
						break;
				}
			}
		}
		private async void ShowAddProductDialog()
		{
			AddProductDialog addDialog = new AddProductDialog();
			var result = await addDialog.ShowAsync();
			if (result == ContentDialogResult.Primary)
			{
				Product newProduct = addDialog.NewProduct;
				newProduct.ID = ++id_counter;
				while (isIdUsedInMasterdata(newProduct.ID))
				{
					newProduct.ID = ++id_counter;
				}
				masterProducts.Add(newProduct);
				updateMasterProductsList();
			}
		}
		private bool isIdUsedInMasterdata(int id)
		{
			for (int i = 0; i < masterProducts.Count; i++)
			{
				if (masterProducts[i].ID == id) return true;
			}

			return false;
		}
		private void NewProduct_Click(object sender, RoutedEventArgs e)
		{
			ShowAddProductDialog();
		}

		// DELIVERY ---------------------------------------------------------------
		private void DeliveryButton_Click(object sender, RoutedEventArgs e)
		{
			deliveryProducts.Clear();
			updateDeliveryProductsList();
			DeliveryList.Visibility = Visibility.Visible;
			DoneButton.Visibility = Visibility.Visible;
			CancelButton.Visibility = Visibility.Visible;
			DeliveryButton.Visibility = Visibility.Collapsed;
			ProductList.ItemClick -= ProductList_ItemClick;
			ProductList.ItemClick += ProductList_AddItemToDelivery;
		}
		private async void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			bool validation = await validateChangingProperties(deliveryProducts);
			if (validation)
			{
				RegisterDelivery();
				HideDeliveryUI();
			}
		}

		private async Task<bool> validateChangingProperties(List<Product> products)
		{
			foreach (var product in products)
			{
				if(int.TryParse(product.ChangingProperty, out int qty))
				{
					if (qty < 0)
					{
						//value is negative
						await ErrorDialog($"Product id {product.ID}: quantity is not a positive number. Please try again with a positive number.");
						return false;
					}
				} else
				{
					//value is not an int
					if (product.ChangingProperty == string.Empty)
					{
						await ErrorDialog($"Product id {product.ID}: quantity is empty, which is not allowed. Please enter a delivery quantity or remove it from delivery.");
						return false;
					} else
					{
						await ErrorDialog($"Product id {product.ID}: quantity is not a number. Please try again with a positive number.");
						return false;
					}
				}
			}
			return true;
		}

		private async Task ErrorDialog(string message)
		{
			MessageDialog ErrorDialog = new MessageDialog(message);
			ErrorDialog.Title = "Error!";
			ErrorDialog.Commands.Add(new UICommand("OK", x =>
			{
				
			}));
			await ErrorDialog.ShowAsync();
		}

		private void RegisterDelivery()
		{
			//Pre: validated ChangingProperty of all delvieryProducts
			foreach (var deliveryProduct in deliveryProducts)
			{
				int qty = int.Parse(deliveryProduct.ChangingProperty);
				var masterProduct = masterProducts.FirstOrDefault(p => p.ID == deliveryProduct.ID);
				if (masterProduct != null)
				{
					masterProduct.IncreaseQty(qty);
				}
			}
			updateMasterProductsList();
		}
		private void ClearChangingProperties() {
			foreach(var product in masterProducts)
			{
				product.ChangingProperty = string.Empty;
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			HideDeliveryUI();
		}
		private void HideDeliveryUI()
		{
			deliveryProducts.Clear();
			DeliveryButton.Visibility = Visibility.Visible;
			DeliveryList.Visibility = Visibility.Collapsed;
			DoneButton.Visibility = Visibility.Collapsed;
			CancelButton.Visibility = Visibility.Collapsed;
			ProductList.ItemClick -= ProductList_AddItemToDelivery;
			ProductList.ItemClick += ProductList_ItemClick;
			
		}
		private void ProductList_AddItemToDelivery(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			clickedItem.ChangingProperty = string.Empty;
			if (!deliveryProducts.Contains(clickedItem))
			{
				deliveryProducts.Add(clickedItem);
				updateDeliveryProductsList();
			}
		}
		private void updateDeliveryProductsList()
		{
			var displayItems = new ObservableCollection<Product>(deliveryProducts);
			DeliveryList.ItemsSource = displayItems;
		}
		private void DeliveryList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			deliveryProducts.Remove(clickedItem);
			clickedItem.ChangingProperty = string.Empty;
			updateDeliveryProductsList();
		}

		private async void ProductList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			ProductInfoDialog productInfoDialog = new ProductInfoDialog(clickedItem);
			var result = await productInfoDialog.ShowAsync();
			if(result == ContentDialogResult.Secondary)
			{
				if(clickedItem.Qty > 0)
				{
					await RemoveProductDialog(clickedItem, $"You are about to remove a product. \nNote: There is still {clickedItem.Qty} piece(s) left on stock of this item!");
				}
				else
				{
					await RemoveProductDialog(clickedItem,"You are about to remove a product.");
				}
			}
		}

		// REMOVE PRODUCT ----------------------------------------------------------
		private async Task RemoveProductDialog(Product clickedItem, string message)
		{
			
			MessageDialog removeProductDialog = new MessageDialog(message);
			removeProductDialog.Title = "Remove Product?";
			removeProductDialog.Commands.Add(new UICommand("Remove", x =>
			{
				removeProductFromMasterdata(clickedItem);
			}));
			removeProductDialog.Commands.Add(new UICommand("Cancel", x =>
			{
				//I want to relaunch the ProductInfoDialog when cancelling
				return;
			}));
			await removeProductDialog.ShowAsync();
		}
		private void removeProductFromMasterdata(Product product)
		{
			masterProducts.Remove(product);
			updateMasterProductsList();
		}

		// CASHIER -----------------------------------------------------------------
		private void updateSubclassLists()
		{
			bookList.Clear();
			movieList.Clear();
			videogameList.Clear();
			foreach (var product in masterProducts)
			{
				if(product is Book book)
				{
					bookList.Add(book);
				} else if(product is Movie movie)
				{
					movieList.Add(movie);
				} else if(product is Videogame videogame)
				{
					videogameList.Add(videogame);
				}
			}
			PupulateSubclassListViews();
		}
		private void PupulateSubclassListViews()
		{
			var bookListViewItems = new ObservableCollection<Book>(bookList);
			BookListView.ItemsSource = bookListViewItems;

			var movieListViewItems = new ObservableCollection<Movie>(movieList);
			MovieListView.ItemsSource = movieListViewItems;

			var videogameListViewItems = new ObservableCollection<Videogame>(videogameList);
			VideogameListView.ItemsSource = videogameListViewItems;
		}
		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			clickedItem.ChangingProperty = "1";
			if (!cartProducts.Contains(clickedItem))
			{
				cartProducts.Add(clickedItem);
				updateCartList();
			}
		}
		private void updateCartList()
		{
			var displayItems = new ObservableCollection<Product>(cartProducts);
			updateTotalPrice();
			CartList.ItemsSource = displayItems;
		}
		private void updateTotalPrice()
		{
			int totalPrice = 0;
			foreach (var product in cartProducts)
			{
				if(product.ChangingProperty != string.Empty)
				{
					if (int.TryParse(product.ChangingProperty, out int cartQty))
					{
						for (int i = 0; i < cartQty; i++)
						{
							totalPrice += product.Price;
						}
					}
				}
			}
			TotalPriceTextBlock.Text = totalPrice.ToString();
		}
		private void CartList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			cartProducts.Remove(clickedItem);
			clickedItem.ChangingProperty = string.Empty;
			updateCartList();
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = (TextBox)sender;
			if (textBox != null)
			{
				if (!int.TryParse(textBox.Text, out int _)){
					textBox.Background = new SolidColorBrush(Windows.UI.Colors.Salmon);
				} else
				{
					textBox.Background = new SolidColorBrush(Windows.UI.Colors.White);
					updateTotalPrice();
				}
			}
		}
		private async Task<bool> validateCart()
		{
			bool isValid = await validateChangingProperties(cartProducts);
			foreach (var product in cartProducts)
			{
				int qty = int.Parse(product.ChangingProperty);
					if (product.Qty < qty)
					{
						//oopsie error! tried to sell more than we have on stock!
						await ErrorDialog($"Tried to sell {qty} of ID: {product.ID} with only {product.Qty} pieces on stock. Lower cart qty and try again!");
						isValid = false;
					}
				
			}
			return isValid;
		}
		private async void CheckOutButton_Click(object sender, RoutedEventArgs e)
		{
			//check if enough qty on stock
			bool isValid = await validateCart();

			// subtract qty

			if (isValid)
			{
				await MakePurchaseDialog();
			}
		}
		private async Task MakePurchaseDialog()
		{

			MessageDialog makePurchaseDialog = new MessageDialog("Awaiting payment...");
			makePurchaseDialog.Title = "Check out";
			makePurchaseDialog.Commands.Add(new UICommand("Payment Received", x =>
			{
				makePurchase();
			}));
			makePurchaseDialog.Commands.Add(new UICommand("Cancel", x =>
			{
				
			}));
			await makePurchaseDialog.ShowAsync();
		}

		private void makePurchase()
		{
			foreach (var product in cartProducts)
			{
				int qty = int.Parse(product.ChangingProperty);
				product.SubtractQty(qty);
			}

			cartProducts.Clear();
			updateCartList();
			updateSubclassLists();
		}

		private void ClearCartButton_Click(object sender, RoutedEventArgs e)
		{
			foreach(var product in cartProducts)
			{
				product.ChangingProperty = string.Empty;
			}
			cartProducts.Clear();
			updateCartList();
		}
	}
}
