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
            switch (line[4])
            {
                case "Book":
                    //Book book = createBookFromString(line);
                    return new Book { 
                        ID = int.Parse(line[0]), 
                        Name = line[1], 
                        Price = int.Parse(line[2]), 
                        Qty = int.Parse(line[3]), 
                        ProductType = ProductType.Book,
                        Author = line[5], 
                        bookGenre = line[6], 
                        BookFormat = (BookFormat)Enum.Parse(typeof(BookFormat), line[7]), 
                        Language = (BookLanguage)Enum.Parse(typeof(BookLanguage), line[8]), 
                    };
                    
                case "Movie":
                    return new Movie {
                        ID = int.Parse(line[0]),
                        Name = line[1],
                        Price = int.Parse(line[2]),
                        Qty = int.Parse(line[3]),
                        ProductType = ProductType.Movie,
                        MovieFormat = (MovieFormat)Enum.Parse(typeof(MovieFormat), line[5]),
                        Playtime = int.Parse(line[6]),
					};
				case "Videogame":
                    return new Videogame { 
                        ID = int.Parse(line[0]), 
                        Name = line[1], 
                        Price = int.Parse(line[2]), 
                        Qty = int.Parse(line[3]),
                        ProductType = ProductType.Videogame,
                        Platform = (VideogamePlatform)Enum.Parse(typeof(VideogamePlatform), line[5]),
					};
                default:
                    return new Product {
						ID = int.Parse(line[0]),
						Name = line[1],
						Price = int.Parse(line[2]),
						Qty = int.Parse(line[3]),
					};
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
			await addDialog.ShowAsync();
		}
		private void NewProduct_Click(object sender, RoutedEventArgs e)
		{
			
			ShowAddProductDialog();
		}

		private void RemoveProduct_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
