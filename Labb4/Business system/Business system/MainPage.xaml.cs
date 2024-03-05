using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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

            var pList = new List<Product>()
            {
                new Book { ID = 1, Name = "Bello Gallico et Civili", Price = 449, Qty = 10},
                new Book { ID = 2, Name = "How to Read a Book", Price = 149, Qty = 5},
                new Book { ID = 3, Name = "Moby Dick", Price = 49}
            };

        }

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			var file = await localFolder.GetFileAsync("data.txt");
            string result = await FileIO.ReadTextAsync(file);
			MyText.Text = result;
		}

	}
}
