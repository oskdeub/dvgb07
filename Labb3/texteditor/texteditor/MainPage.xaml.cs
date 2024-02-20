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
using System.Reflection.Metadata;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.UI.Core.Preview;

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
		private String defaultDocname = "nytt_dokument";
		public MainPage()
        {
			unsaved_changes = false;
			fileExists = false;
			changeTitle(defaultDocname);
            this.InitializeComponent();
			// från https://stackoverflow.com/questions/62910280/is-it-possible-to-pop-up-my-dialog-box-when-click-the-close-icon-on-the-upper-ri
			Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested +=
				async (sender, args) =>
				{
					args.Handled = true;// slut referens
					if (unsaved_changes)
					{
						await Exit_Dialog("Du har osparade ändringar.");
					} else { Application.Current.Exit(); }
					
				};
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			Clear_Dialog("All text försvinner om du rensar.");
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			if (unsaved_changes)
			{
				Exit_Dialog("Du har osparade ändringar.");
			} else
			{
				Application.Current.Exit();
			}
		}

		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			if (unsaved_changes)
			{ 
			NewDocument_Dialog("Du har osparade ändringar.");
			} else
			{
				NewSheet();
			}
		}

		private async Task Exit_Dialog(string v)
		{
			MessageDialog exitDialog = new MessageDialog(v);
			exitDialog.Title = "Spara innan du avslutar?";
			exitDialog.Commands.Add(new UICommand("Spara och Avsluta", async x =>
			{
				await SaveFile();
				Application.Current.Exit();
			}));
			exitDialog.Commands.Add(new UICommand("Avsluta utan att spara", x =>
			{
				Application.Current.Exit();
			}));
			exitDialog.Commands.Add(new UICommand("Avbryt", x =>
			{
				return;
			}));
			await exitDialog.ShowAsync();
		}
		private async void NewDocument_Dialog(string v)
		{
			MessageDialog newDocDialog = new MessageDialog(v);
			newDocDialog.Title = "Spara innan du fortsätter?";
			newDocDialog.Commands.Add(new UICommand("Spara", x =>
			{
				SaveFile();
				NewSheet();
			}));
			newDocDialog.Commands.Add(new UICommand("Fortsätt utan att spara", x =>
			{
				NewSheet();
			}));
			newDocDialog.Commands.Add(new UICommand("Avbryt", x =>
			{
				return;
			}));
			await newDocDialog.ShowAsync();
		}
		private void NewSheet()
		{
			savefile = null;
			MainTextBox.Text = string.Empty;
			changeTitle(defaultDocname);
			unsaved_changes = false;
			fileExists = false;
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			SaveFile();
		}
		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
				if (unsaved_changes)
				{
					DiscardChanges_Dialog("Du har osparade ändringar.");
				} else
				{
					openFile();
				}
		}
		
		private async void Clear_Dialog(string msg)
		{
			MessageDialog clearDialog = new MessageDialog(msg);
			clearDialog.Title = "Rensa text?";
			clearDialog.Commands.Add(new UICommand("Rensa", x =>
			{
				MainTextBox.Text = string.Empty;
			}));
			clearDialog.Commands.Add(new UICommand("Avbryt", x =>
			{
				return;
			}));
			await clearDialog.ShowAsync();
		}

		private async void DiscardChanges_Dialog(string msg)
		{
			MessageDialog discard_Dialog = new MessageDialog(msg);
			discard_Dialog.Title = "Spara innan?";
			discard_Dialog.Commands.Add(new UICommand("Spara och Öppna", x =>
			{
				SaveFile();
				openFile();
			}));
			discard_Dialog.Commands.Add(new UICommand("Öppna utan att spara", x =>
			{
				openFile();
			}));
			discard_Dialog.Commands.Add(new UICommand("Avbryt", x =>
			{
				return;
			}));
			await discard_Dialog.ShowAsync();
		}

		private void SaveAsButton_Click(object sender, RoutedEventArgs e)
		{
			SaveAs();
		}

		private async Task SaveFile()
		{
			if (!fileExists)
			{
				await SaveAs();
			}
			else
			{
				await Save();
			}
		}

		private async Task Save()
		{
			if (savefile != null)
			{
				await Windows.Storage.FileIO.WriteTextAsync(savefile, MainTextBox.Text);
				unsaved_changes = false;
				changeTitle(savefile.Name);
			}
		}

		private async Task SaveAs()
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
		private async void openFile()
		{
			FileOpenPicker fileOpenPicker = new FileOpenPicker
			{
				ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
				SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
			};
			fileOpenPicker.FileTypeFilter.Add(".txt");
			savefile = await fileOpenPicker.PickSingleFileAsync();
			if (savefile != null)
			{
				toggleTextChange(false);
				unsaved_changes = false;
				fileExists = true;
				MainTextBox.Text = await Windows.Storage.FileIO.ReadTextAsync(savefile);
				changeTitle(savefile.Name);
				toggleTextChange(true);
				updateCounters(MainTextBox.Text);
			}
		}

		private void toggleTextChange(bool v)
		{
			if (v)
			{
				MainTextBox.TextChanged += MainTextBox_TextChanged; //Från chatGPT
			}
			else
			{
				MainTextBox.TextChanged -= MainTextBox_TextChanged; //Från chatGPT
			}
		}
		private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			AddAsterixToTitle();
			updateCounters(MainTextBox.Text);
		}
		private void updateCounters(string text) {
			update_CharWithSpaceCounter(text);
			update_CharNoSpaceCounter(text);
			update_WordCounter(text);
			update_RowCounter(text);
		}
		private void update_CharWithSpaceCounter(string text)
		{
			CharWithSpaceCounter.Text = text.Length.ToString();
		}
		private void update_CharNoSpaceCounter(string text)
		{
			text = text.Replace(" ", "").Replace("\n","").Replace("\r","");
			CharNoSpaceCounter.Text = text.Length.ToString();
		}

		private void update_WordCounter(string text)
		{
			char[] delimiters = new char[] {' ', '\r', '\n', '\t'};
			int wordCounter = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
			WordCounter.Text = wordCounter.ToString();
		}
		private void update_RowCounter(string text)
		{
			char[] delimiters = new char[] { '\r', '\n' };
			int rowCounter = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
			RowCounter.Text = rowCounter.ToString();
		}

		private void AddAsterixToTitle()
		{
			if (fileExists && !unsaved_changes)
			{
				changeTitle("*" + savefile.Name);
			}
			unsaved_changes = true;
		}
		private void changeTitle(String title)
		{
			ApplicationView.GetForCurrentView().Title = title;
		}
	}
}
