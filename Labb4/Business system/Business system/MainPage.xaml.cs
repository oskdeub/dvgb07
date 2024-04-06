using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        private int id_counter;
        public MainPage()
        {
            id_counter = 0;
            this.InitializeComponent();
        }

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			List<string[]> data = await ReadCSVFile("data.csv");
            populateList(data);
		}
		
		private void updateList()
		{
			id_counter = masterProducts.Count();
			var displayItems = new ObservableCollection<Product>(masterProducts);
			ProductList.ItemsSource = displayItems;
		}
        public void populateList(List<string[]> data) {
			//Clear before reading
			masterProducts.Clear();
			//Add allez producten por favor
			masterProducts.AddRange(parseCsvData(data));

			id_counter = masterProducts.Count();
			var displayItems = new ObservableCollection<Product>(masterProducts);
			ProductList.ItemsSource = displayItems;

			/*
			var dItems = new ObservableCollection<string>();
			foreach (var line in data.Skip(1))
			{
				var lineDisplay = string.Join("; ", line);
				dItems.Add(lineDisplay);
				id_counter++;
			}
			CsvList.ItemsSource = dItems;

			XAML
			<ListView Name="CsvList" Grid.Column="1">
				<ListView.ItemTemplate>
					<DataTemplate>
						<TextBlock Text="{Binding}" />
					</DataTemplate>
				</ListView.ItemTemplate>
			</ListView>
			*/
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

		private Book createBookFromString(string[] line)
		{
            Book book = new Book();
            int id, price, qty;
            BookFormat format = (BookFormat)Enum.Parse(typeof(BookFormat), line[7]);


            if (int.TryParse(line[0], out var id_num) && int.TryParse(line[2], out var price_num) && int.TryParse(line[3], out var qty_num))
            {
                id = id_num;
                price = price_num;
                qty = qty_num;
            }
            return book;
		}


		public async Task<List<string[]>> ReadCSVFile (string filename)
        {
            List<string[]> data = new List<string[]>();
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			var file = await localFolder.GetFileAsync(filename);
            IList<string> lines = await FileIO.ReadLinesAsync(file);

            foreach (var line in lines)
            {
                string[] parts = line.Split(';');
                data.Add(parts);
            }
            return data;
		}

        internal async Task WriteToCsv(string filename, List<Product> products)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("ID; Name; Price; Qty; Product;");
            foreach (var product in products)
            {
                content.AppendLine(string.Join(";", product.ToCsv()));
            }
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			var file = await localFolder.GetFileAsync(filename);
			await FileIO.WriteTextAsync(file, content.ToString());
		}
		private async void Open_Click(object sender, RoutedEventArgs e)
		{
			List<string[]> data = await ReadCSVFile("data.csv");
			populateList(data);
		}

		private async void Save_Click(object sender, RoutedEventArgs e)
		{
            await WriteToCsv("data.csv", masterProducts);
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
			if(result == ContentDialogResult.Primary)
			{
				Product newProduct = addDialog.NewProduct;
				newProduct.ID = ++id_counter;
				masterProducts.Add(newProduct);
				updateList();
			}
		}
		private void NewProduct_Click(object sender, RoutedEventArgs e)
		{
			ShowAddProductDialog();
		}

		private void RemoveProduct_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ProductList_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (Product)e.ClickedItem;

			var typeClicked = clickedItem.ProductType.ToString();
			switch(typeClicked)
			{
				case "Book":

			}
		}
	}
}
