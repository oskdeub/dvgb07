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
        public MainPage()
        {
            this.InitializeComponent();
        }

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
            List<string[]> data = await ReadCSVFile("data.csv");
            TBTEST.Text = data.Count.ToString();

			List<Product> products = parseCsvData(data);
			var displayItems = new ObservableCollection<Product>(products);
            ProductList.ItemsSource = displayItems;

			var dItems = new ObservableCollection<string>();
			foreach (var line in data.Skip(1))
            {
                var lineDisplay = string.Join("; ", line);
                dItems.Add(lineDisplay);

            }
            CsvList.ItemsSource = dItems;
            
            /*
			Movie newMovie = (Movie)ProductFactory.CreateProduct(ProductType.Movie, 6, "Nyckeln till frihet", 99);
			newMovie.Playtime = 142;
			newMovie.MovieFormat = MovieFormat.DVD;

			Videogame newVidya = (Videogame)ProductFactory.CreateProduct(ProductType.Videogame, 7, "Demon's Souls", 499);
			newVidya.Platform = VideogamePlatform.Playstation_5;

			Book newBook = (Book)ProductFactory.CreateProduct(ProductType.Book, 8, "Great Gatsby", 79);
			newBook.Author = "F. Scoot Fitzgerald";
			newBook.bookGenre = "Noir";
			newBook.BookFormat = BookFormat.Ebook;
			var pList = new List<Product>()
			{

				new Book { ID = 1, Name = "Bello Gallico et Civili", Price = 449, Qty = 10, Author = "Julius Caesar", BookFormat = BookFormat.Paperback, Language = BookLanguage.Latin, bookGenre = "Historia"},
				new Book { ID = 2, Name = "How to Read a Book", Price = 149, Qty = 5},
				new Book { ID = 3, Name = "Moby Dick", Price = 49},
				new Videogame { ID = 4, Name = "Elden Ring", Price = 599, Platform = VideogamePlatform.Playstation_5, Qty = 1 },
				new Movie { ID = 5, Name = "Gudfadern", Price = 99, Playtime = 152, MovieFormat = MovieFormat.DVD},
				newMovie,
				newVidya,
				newBook,
			};
            var pList2 = new List<Product>();
            //await WriteToCsv("data.csv", pList2);
            await WriteToCsv("data.csv", pList);
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

        //Gör sönder när vissa fält är tomma, kanske pga hårdkodat direkt i csv-filen
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
                        MovieFormat = (MovieFormat)Enum.Parse(typeof(MovieFormat), line[5]),
                        Playtime = int.Parse(line[6]),
					};
				case "Videogame":
                    return new Videogame { 
                        ID = int.Parse(line[0]), 
                        Name = line[1], 
                        Price = int.Parse(line[2]), 
                        Qty = int.Parse(line[3]),
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

		private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
            if(args.InvokedItemContainer is NavigationViewItem item)
            {
                var tag = item.Tag.ToString();
                if(tag == "Kassa")
                {
                    ContentFrame.Navigate(typeof(Kassa));
                } else if(tag == "Lager")
                {
                    ContentFrame.Navigate(typeof(Lager));
                }
            }
		}
	}
}
