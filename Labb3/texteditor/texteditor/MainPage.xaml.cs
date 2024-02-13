using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace texteditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		private Windows.Storage.StorageFile savefile;
		private Boolean unsaved_changes;
		private Boolean fileExists;
		public MainPage()
        {
			unsaved_changes = false;
			fileExists = false;
            this.InitializeComponent();
        }

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			Clear_Dialog($"All text försvinner om du rensar.");
		}

		private async void Clear_Dialog(string msg)
		{
			MessageDialog sure_Dialog = new MessageDialog(msg);
			sure_Dialog.Commands.Add(new UICommand("Rensa", x =>
			{
				MainTextBox.Text = string.Empty;
			}));
			sure_Dialog.Commands.Add(new UICommand("Avbryt", x =>
			{
				return;
			}));
			await sure_Dialog.ShowAsync();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (!fileExists)
			{
				SaveAs();
			} else
			{
				Save();
			}
		}
		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			openFile();
		}

		private void SaveAsButton_Click(object sender, RoutedEventArgs e)
		{
			SaveAs();
		}

		private async void Save()
		{
			if (savefile != null)
			{
				await Windows.Storage.FileIO.WriteTextAsync(savefile, MainTextBox.Text);
				unsaved_changes = false;
				changeTitle(savefile.Name);
			}
		}

		private async void openFile()
		{
			FileOpenPicker fileOpenPicker = new FileOpenPicker
			{
				ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			fileOpenPicker.FileTypeFilter.Add(".txt");
			savefile = await fileOpenPicker.PickSingleFileAsync();
			if(savefile != null)
			{
				toggleTextChange(false);
				unsaved_changes = false;
				fileExists = true;
				MainTextBox.Text = await Windows.Storage.FileIO.ReadTextAsync(savefile);
				changeTitle(savefile.Name);
				toggleTextChange(true);
			}
		}

		private void toggleTextChange(bool v)
		{
			if (v)
			{
				MainTextBox.TextChanged += MainTextBox_TextChanged; //Från chatGPT
			} else
			{
				MainTextBox.TextChanged -= MainTextBox_TextChanged; //Från chatGPT
			}
		}

		private async void SaveAs()
		{
			FileSavePicker fileSavePicker = new FileSavePicker
			{
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			fileSavePicker.FileTypeChoices.Add("Plain Text", new List<string>() {".txt"});
			fileSavePicker.SuggestedFileName = "dokument";

			savefile = await fileSavePicker.PickSaveFileAsync();
			if (savefile != null)
			{
				unsaved_changes = false;
				fileExists = true;
				await Windows.Storage.FileIO.WriteTextAsync(savefile, MainTextBox.Text);
				changeTitle(savefile.Name);
			}
		}
		private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (fileExists && !unsaved_changes)
			{
				changeTitle("*" + savefile.Name);
				unsaved_changes = true;
			}
		}
		private void changeTitle(String title)
		{
			ApplicationView.GetForCurrentView().Title = title;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Exit();
		}
	}
}
