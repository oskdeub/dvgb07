using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using static System.Net.WebRequestMethods;



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
		
		private int id_counter;
		public MainPage()
		{
			id_counter = 0;
			this.InitializeComponent();
		}
		/// <summary>
		/// Funktion som körs när programmet laddas. Öppnar en FilePicker för att öppna en .csv-fil.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			await openFile();
			if(CSVFile != null)
			{
				List<string[]> data = await ReadCSVFile(CSVFile);
				populateList(data);
			} else
			{
				await ErrorDialog("No file found. Please restart program.");
				
			}
			
		}
		/// <summary>
		/// Öppnar en fil och tilldelar den globala variabeln StorageFil CSVFile.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// Läser från csv-filen
		/// </summary>
		/// <param name="file"></param>
		/// <returns> En List<string> med alla rader i csv-filen </returns>
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
		/// <summary>
		/// Fyller masterProducts med produkter från csv-filens data.
		/// </summary>
		/// <param name="data"> csv-filens data </param>
		public void populateList(List<string[]> data)
		{
			//Clear before reading
			masterProducts.Clear();
			//Add all products to masterProducts
			masterProducts.AddRange(parseCsvData(data));

			updateMasterProductsList();
			updateSubclassLists();
		}
		/// <summary>
		/// Uppdaterar masterProducts och sparar dessa till fil.
		/// </summary>
		private void updateMasterProductsList()
		{
			id_counter = masterProducts.Count();
			var displayItems = new ObservableCollection<Product>(masterProducts);
			ProductList.ItemsSource = displayItems;
			updateSubclassLists();
			WriteToFile();
		}
		
		/// <summary>
		/// Parserar csv.filens data och gör varje rad till en produkt
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		internal List<Product> parseCsvData(List<string[]> data)
		{
			var products = new List<Product>();
			foreach (var line in data.Skip(1))
			{
				products.Add(parseProductFromString(line));
			}
			return products;
		}
		/// <summary>
		/// Hittar produktens typ och skapar därifrån det objektet typen är.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private Product parseProductFromString(string[] line)
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
					book.bookGenre = line[5];
					book.BookFormat = line[6];
					book.Language = line[7];
		
					return book;

				case "Movie":
					Movie movie = new Movie();
					movie.ID = int.Parse(line[0]);
					movie.Name = line[1];
					movie.Price = int.Parse(line[2]);
					movie.Qty = int.Parse(line[3]);
					movie.ProductType = ProductType.Movie;
					movie.MovieFormat = line[5];
					if (int.TryParse(line[6], out int x)){
						movie.Playtime = x;
					}
					
					return movie;

				case "Videogame":
					Videogame videogame = new Videogame();
					videogame.ID = int.Parse(line[0]);
					videogame.Name = line[1];
					videogame.Price = int.Parse(line[2]);
					videogame.Qty = int.Parse(line[3]);
					videogame.ProductType = ProductType.Videogame;
					videogame.Platform = line[5];
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
		
		
		/// <summary>
		/// Skriver masterProducts till CSVFile.
		/// </summary>
		private async void WriteToFile()
		{
			await WriteToCsv(masterProducts);
		}
		/// <summary>
		/// Skriver products till CSVFile, rad för rad
		/// </summary>
		/// <param name="products"></param>
		/// <returns></returns>
		internal async Task WriteToCsv(List<Product> products)
		{
			StringBuilder content = new StringBuilder();
			content.AppendLine("ID; Name; Price; Qty; Product;");
			foreach (var product in products)
			{
				content.AppendLine(string.Join(";", product.ToCsv()));
			}
			
			await FileIO.WriteTextAsync(CSVFile, content.ToString());
		}
		/// <summary>
		/// SelectionChanged-händelsens event handler för att hantera byte av Kassa och Lager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
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
						updateSubclassLists();
						cartProducts.Clear();
						updateCartList();
						break;
					case "Lager":
						LagerPanel.Visibility = Visibility.Visible;
						break;
				}
			}
		}
		/// <summary>
		/// Visar en Dialog för att lägga till en produkt
		/// </summary>
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
		/// <summary>
		/// Kollar om ett id redan finns i masterProducts
		/// </summary>
		/// <param name="id"></param>
		/// <returns>true om id:t redan finns i masterProducts, annars false</returns>
		private bool isIdUsedInMasterdata(int id)
		{
			for (int i = 0; i < masterProducts.Count; i++)
			{
				if (masterProducts[i].ID == id) return true;
			}

			return false;
		}
		/// <summary>
		/// Händelsehanterare vid tryck på New Product-knappen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewProduct_Click(object sender, RoutedEventArgs e)
		{
			ShowAddProductDialog();
		}

		// DELIVERY ---------------------------------------------------------------
		/// <summary>
		/// Visar delivery-rutan och ändrar ProductList.ItemClick till AddItemToDelivery.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeliveryButton_Click(object sender, RoutedEventArgs e)
		{
			deliveryProducts.Clear();
			updateDeliveryProductsList();
			DeliveryButton.IsEnabled = false;
			DoneButton.IsEnabled = true;
			CancelButton.IsEnabled = true;
			DeliveryList.Visibility = Visibility.Visible;
			ProductList.ItemClick -= ProductList_ItemClick;
			ProductList.ItemClick += ProductList_AddItemToDelivery;
		}
		/// <summary>
		/// Klar med delivery, startar validering av TextBoxarna, eller snarare deras ChangingProperty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			bool validation = await validateChangingProperties(deliveryProducts);
			if (validation)
			{
				RegisterDelivery();
				HideDeliveryUI();
			}
		}
		/// <summary>
		/// Validerar alla ChangingProperty i products
		/// </summary>
		/// <param name="products"></param>
		/// <returns>true om valideringen gick bra, annars false</returns>
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
		/// <summary>
		/// Visar en error-dialog.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private async Task ErrorDialog(string message)
		{
			MessageDialog ErrorDialog = new MessageDialog(message);
			ErrorDialog.Title = "Error!";
			ErrorDialog.Commands.Add(new UICommand("OK", x =>
			{
				
			}));
			await ErrorDialog.ShowAsync();
		}
		/// <summary>
		/// Sköter uppdateringen av alla produkters qty-fält vid in-leverans.
		/// </summary>
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
		/// <summary>
		/// Rensar alla ChangingProperty så att dessa inte hänger med vid nästa delivery.
		/// </summary>
		private void ClearChangingProperties() {
			foreach(var product in masterProducts)
			{
				product.ChangingProperty = string.Empty;
			}
		}
		/// <summary>
		/// Avbryter in-leverans och gömmer Delivery UI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			HideDeliveryUI();
			ClearChangingProperties();
		}
		/// <summary>
		/// Gömmer all UI för Delivery och återställer ProductList.ItemClick till ItemClick.
		/// </summary>
		private void HideDeliveryUI()
		{
			deliveryProducts.Clear();
			DeliveryButton.IsEnabled = true;
			DeliveryList.Visibility = Visibility.Collapsed;
			DoneButton.IsEnabled = false;
			CancelButton.IsEnabled = false;
			ProductList.ItemClick -= ProductList_AddItemToDelivery;
			ProductList.ItemClick += ProductList_ItemClick;
			
		}
		/// <summary>
		/// Händelse-event som lägger till en produkt i deliveryProducts från masterProducts.
		/// Intressant är att inga kopior skapas utan produkterna länkas med reference till produkten i masterProducts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Uppdaterar DeliveryList ListView och populerar dess lista med deliveryProducts.
		/// </summary>
		private void updateDeliveryProductsList()
		{
			var displayItems = new ObservableCollection<Product>(deliveryProducts);
			DeliveryList.ItemsSource = displayItems;
		}
		/// <summary>
		/// Vid klick i DeliveryList ska produkten tas bort från listan. Uppdaterar sedan UI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeliveryList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			deliveryProducts.Remove(clickedItem);
			clickedItem.ChangingProperty = string.Empty;
			updateDeliveryProductsList();
		}
		/// <summary>
		/// Visar information om en klickad produkt i ProductInfoDialog (se AddProductDialog.xaml)
		/// Agerar baserat på resultatet av att klicka på Remove Product i dialogen, varpå en till Kontroll-Dialog visas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Visar en dialog som frågan användaren om den verkligen vill ta bort produkten.
		/// </summary>
		/// <param name="clickedItem"></param>
		/// <param name="message"></param>
		/// <returns></returns>
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
		/// <summary>
		/// Tar bort en produkt ur masterProducts och uppdaterar sedan ui.
		/// </summary>
		/// <param name="product"></param>
		private void removeProductFromMasterdata(Product product)
		{
			masterProducts.Remove(product);
			updateMasterProductsList();
		}

		// CASHIER -----------------------------------------------------------------
		/// <summary>
		/// Fyller subklasserna till Product:s listor.
		/// </summary>
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
			PopulateSubclassLists();
		}
		/// <summary>
		/// Fyller subklassernas ListViews.
		/// </summary>
		private void PopulateSubclassLists()
		{
			var bookListViewItems = new ObservableCollection<Book>(bookList);
			BookListView.ItemsSource = bookListViewItems;

			var movieListViewItems = new ObservableCollection<Movie>(movieList);
			MovieListView.ItemsSource = movieListViewItems;

			var videogameListViewItems = new ObservableCollection<Videogame>(videogameList);
			VideogameListView.ItemsSource = videogameListViewItems;
		}
		/// <summary>
		/// Klickfunktion för samtliga subklass-listor. Fyller Kundkorgen!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			clickedItem.ChangingProperty = "1";
			if (!cartProducts.Contains(clickedItem) && clickedItem.Qty > 0)
			{
				cartProducts.Add(clickedItem);
				updateCartList();
			}
		}
		/// <summary>
		/// Uppdaterar UI för kundkorgen
		/// </summary>
		private void updateCartList()
		{
			var displayItems = new ObservableCollection<Product>(cartProducts);
			updateTotalPrice();
			CartList.ItemsSource = displayItems;
		}
		/// <summary>
		/// Uppdaterar priset som visas i TotalPriceTextBlock baserat på antalet i qty och produktens pris
		/// </summary>
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
			if (totalPrice < 0)
			{
				TotalPriceTextBlock.Text = "Error, price too high";
			}
			TotalPriceTextBlock.Text = totalPrice.ToString();
		}
		/// <summary>
		/// Tar bort ett klickat item i Kundkorgen och cartProducts. Uppdaterar därefter UI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CartList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;
			cartProducts.Remove(clickedItem);
			clickedItem.ChangingProperty = string.Empty;
			updateCartList();
		}

		/// <summary>
		/// Reagerar på TextBoxarna i Kundkorgen för att dra slutsats om det är en accepterad input och uppdaterar priset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Validerar kundkorgen för att motverka icke-numerisk inmatning.
		/// </summary>
		/// <returns></returns>
		private async Task<bool> validateCart()
		{
			bool isValid = await validateChangingProperties(cartProducts);
			if (isValid)
			{
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
			}
			
			return isValid;
		}
		/// <summary>
		/// händelsedriven funktion som reagerar på knapptryck av Check out-knappen. Påbörjar köpet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// En dialogruta för att acceptera (simulerad) betalning.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// Uppdaterar qty av berörda produkter, uppdaterar sedan ui.
		/// </summary>
		private void makePurchase()
		{
			foreach (var product in cartProducts)
			{
				int qty = int.Parse(product.ChangingProperty);
				product.SubtractQty(qty);
			}
			foreach (var product in cartProducts)
			{
				product.ChangingProperty = string.Empty;
			}
			cartProducts.Clear();
			updateCartList();
			updateSubclassLists();
			updateMasterProductsList();
		}
		/// <summary>
		/// Rensar kundkorgen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearCartButton_Click(object sender, RoutedEventArgs e)
		{
			foreach(var product in cartProducts)
			{
				product.ChangingProperty = string.Empty;
			}
			cartProducts.Clear();
			updateCartList();
		}

		//Labb 5
		static readonly HttpClient client = new HttpClient();
		private async void FetchData_Click(object sender, RoutedEventArgs e)
		{
			await FetchProductsFromAPI();
		}

		public async Task FetchProductsFromAPI()
		{   //Referens: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0
			// specifikt await client.GetStringAsync();
			try
			{
				// HttpResponseMessage response = await client.GetAsync("https://hex.cse.kau.se/~jonavest/csharp-api/");
				// response.EnsureSuccessStatusCode();
				// string responseBody = await response.Content.ReadAsStringAsync();

				string responseBody = await client.GetStringAsync("https://hex.cse.kau.se/~jonavest/csharp-api/");
				Debug.WriteLine(responseBody);
				loadXML(responseBody);
				
			} catch (HttpRequestException e)
			{
				await ErrorDialog($"Could not fetch data from central stock. \n {e}");
			}
			// Slut referens
		}
		private async Task<bool> isErrorResponse (string response)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(response);

			if (doc.FirstChild.FirstChild.Name == "error")
			{
				await ErrorDialog("Error in response from central warehouse. Try again.");
				return true;
			} else
			{
				return false;
			}
		}
		private async void loadXML(string xmlBody)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlBody);

			List<Product> pList = new List<Product>();
			//kolla om det är error

			if (isErrorResponse(xmlBody).Result)
			{
				return;
			} else
			{
				XmlNode products = doc.SelectSingleNode("/response/products");
				foreach (XmlNode product in products)
				{
					if (product.Name == "book")
					{
						pList.Add(await parseXmlBook(product));
					} else if (product.Name == "game")
					{
						pList.Add(await parseXmlVideogame(product));
					} else if (product.Name == "movie")
					{
						pList.Add(await parseXmlMovie(product));
					} else
					{
						await ErrorDialog($"Could not define a product of type {product.Name}");
					}
				}
				setFetchedProductsAsMaster(pList);
			}
		}

		private void setFetchedProductsAsMaster(List<Product> fetchedProducts)
		{
			foreach (Product fetchedP  in fetchedProducts)
			{
				foreach (Product masterP in masterProducts)
				{
					if(fetchedP.ID == masterP.ID)
					{
						masterP.Price = fetchedP.Price;
						masterP.Qty	= fetchedP.Qty;
						break;
					}
				}
			}
			setUpdatedTime();
			updateMasterProductsList();
		}

		private void setUpdatedTime()
		{
			FetchDataTextBox.Text = DateTime.Now.ToShortTimeString();
		}

		private async void PublishButton_Click(object sender, RoutedEventArgs e)
		{
			await PublishProductsWithAPI(masterProducts);
		}

		public async Task PublishProductsWithAPI(List<Product> productList)
		{   // Referens https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0
			// specifikt await client.GetStringAsync();
			try
			{

				string URLstart = "https://hex.cse.kau.se/~jonavest/csharp-api/?action=update&id=";
				string URLstock = "&stock=";
				foreach (Product product in productList)
				{
					string API_URL = URLstart + product.ID + URLstock + product.Qty;
					string responseBody = await client.GetStringAsync(API_URL);
					if(isErrorResponse(responseBody).Result)
					{
						return;
					}
				}
			}
			catch (HttpRequestException e)
			{
				await ErrorDialog($"Could not fetch data from central stock. \n {e}");
			}
			//Slut Referens
		}

		private async Task<Movie> parseXmlMovie(XmlNode movieNode)
		{
			Movie movie = new Movie();

			//ID
			if (int.TryParse(movieNode["id"].InnerText, out int res))
			{
				movie.ID = res;
			}
			else
			{
				await ErrorDialog($"Fetched movie with id {movieNode["id"].InnerText} could not parse ID to integer");
			}
			//Name
			movie.Name = movieNode["name"].InnerText;
			//Price
			if (int.TryParse(movieNode["price"].InnerText, out int res2))
			{
				movie.Price = res2;
			}
			else
			{
				await ErrorDialog($"Fetched movie with id {movieNode["id"].InnerText} could not parse price to integer");
			}
			//Qty/Stock
			if (int.TryParse(movieNode["stock"].InnerText, out int res3))
			{
				movie.Qty = res3;
			}
			else
			{
				await ErrorDialog($"Fetched movie with id {movieNode["id"].InnerText} could not parse playtime to integer");
			}
			//Playtime
			if (movieNode["playtime"] != null)
			{
				if (int.TryParse(movieNode["playtime"].InnerText, out int res4))
				{
					movie.Playtime = res4;
				}
				else
				{
					await ErrorDialog($"Fetched movie with id {movieNode["id"].InnerText} could not parse playtime to integer");
				}
			}
			//Format
			if (movieNode["format"] != null)
			{
				movie.MovieFormat = movieNode["format"].InnerText;
			}
			

			return movie;
		}

		private async Task<Videogame> parseXmlVideogame(XmlNode videogameNode)
		{
			Videogame videogame = new Videogame();

			if (int.TryParse(videogameNode["id"].InnerText, out int res))
			{
				videogame.ID = res;
			}
			else
			{
				await ErrorDialog($"Fetched game with id {videogameNode["id"].InnerText} could not parse ID to integer");
			}
			videogame.Name = videogameNode["name"].InnerText;
			if (int.TryParse(videogameNode["price"].InnerText, out int res2))
			{
				videogame.Price = res2;
			}
			else
			{
				await ErrorDialog($"Fetched game with id {videogameNode["id"].InnerText} could not parse price to integer");
			}
			if (int.TryParse(videogameNode["stock"].InnerText, out int res3))
			{
				videogame.Qty = res3;
			}
			else
			{
				await ErrorDialog($"Fetched game with id {videogameNode["id"].InnerText} could not parse stock to integer");
			}

			if (videogameNode["platform"] != null)
			{
				videogame.Platform = videogameNode["platform"].InnerText;
			}

			return videogame;
		}

		private async Task<Book> parseXmlBook(XmlNode bookNode)
		{
			Book book = new Book();
			
			if (int.TryParse(bookNode["id"].InnerText, out int res))
			{
				book.ID = res;
			} else
			{
				await ErrorDialog($"Fetched book with id {bookNode["id"].InnerText} could not parse ID to integer");
			}
			book.Name = bookNode["name"].InnerText;
			if (int.TryParse(bookNode["price"].InnerText, out int res2))
			{
				book.Price = res2;
			}
			else
			{
				await ErrorDialog($"Fetched book with id {bookNode["id"].InnerText} could not parse price to integer");
			}
			if (int.TryParse(bookNode["stock"].InnerText, out int res3))
			{
				book.Qty = res3;
			}
			else
			{
				await ErrorDialog($"Fetched book with id {bookNode["id"].InnerText} could not parse stock to integer");
			}

			if (bookNode["format"] != null)
			{
				book.BookFormat = bookNode["format"].InnerText;
			}
			if (bookNode["genre"] != null)
			{
				book.bookGenre = bookNode["genre"].InnerText;
			}
			if (bookNode["language"] != null)
			{
				book.Language = bookNode["language"].InnerText;
			}
			return book;
		}

	}
}
