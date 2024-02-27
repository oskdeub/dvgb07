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
// Oskar Deubler
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
            this.InitializeComponent();
			NewSheet();
			// från https://stackoverflow.com/questions/62910280/is-it-possible-to-pop-up-my-dialog-box-when-click-the-close-icon-on-the-upper-ri
			// Adds the Exit_Dialog when pressing 'X'
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
		//  ------------- CLICKS ---------------
		/// <summary>
		/// Click event for 'ClearButton'
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			if (MainTextBox.Text != "")
			{
				Clear_Dialog("All text försvinner om du rensar.");
			}
		}
		/// <summary>
		/// Event for CloseButton, 
		/// Prompts user to save if it has unsvaed changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Event for NewButton,
		/// Prompts user to save if it has unsaved changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Click event for SaveButton,
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			SaveFile();
		}

		/// <summary>
		/// Click event for OpenButton
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			if (unsaved_changes)
			{
				DiscardChanges_Dialog("Du har osparade ändringar.");
			}
			else
			{
				openFile();
			}
		}
		/// <summary>
		/// Click event for SaveAsButton
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAsButton_Click(object sender, RoutedEventArgs e)
		{
			SaveAs();
		}

		//  ------------- TEXT CHANGE ---------------
		/// <summary>
		/// TextChanged event for MainTextBox,
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)													//Oskar Deubler
		{
			AddAsterixToTitle();
			updateCounters(MainTextBox.Text);
		}

		//  ------------- HELP FUNCTIONS ---------------
		/// <summary>
		/// Generates a 'new' textsheet, and resets everything that needs resetting
		/// savefile is set to null to 'close' any opened files
		/// </summary>
		private void NewSheet()
		{
			savefile = null;
			MainTextBox.Text = string.Empty;
			changeTitle(defaultDocname);
			unsaved_changes = false;
			fileExists = false;
		}
		/// <summary>
		/// if file exists, then the SaveFile functions makes sure the user gets to Save As... 
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// A function that adds/removes the TextChanged event from MainTextBox. 
		/// Needed since open file changes the content of the textbox, which then performs the asterix function
		/// to add an asterix in the title.
		/// </summary>
		/// <param name="v">Toggles the TextChanged event on MainTextBox</param>
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

		//  ------------- DIALOGS ---------------
		/// <summary>
		/// Dialog upon exiting application. Let user Save and Exit, Exit without Saving, or Cancel the Exit.
		/// await SaveFile() wait for SaveFile to finish before continuing to shutting down the application.
		/// </summary>
		/// <param name="v">Dialog message</param>
		/// <returns></returns>
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
		/// <summary>
		/// Dialog to prompt user to save before continuing. SaveFile will either Save or SaveAs, depending on file_exists.
		/// </summary>
		/// <param name="v">Dialog message</param>
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
		/// <summary>
		/// Dialog to warn user that everything unsaved will be lost.
		/// </summary>
		/// <param name="msg">Dialog message</param>
		private async void Clear_Dialog(string msg)
		{																																																																																			//Oskar Deubler
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
		/// <summary>
		/// Dialog to ask user if it wants to Save and Open, Open without Saving or Cancel the Open File.
		/// </summary>
		/// <param name="msg"></param>
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

		//  ------------- IO ---------------
		/// <summary>
		/// Save file, set unsaved changes to false,
		/// changeTitle() up for deletion.
		/// </summary>
		/// <returns></returns>
		private async Task Save()
		{
			if (savefile != null)
			{
				await Windows.Storage.FileIO.WriteTextAsync(savefile, MainTextBox.Text);
				unsaved_changes = false;
				changeTitle(savefile.Name);
			}
		}
		/// <summary>
		/// Save As function using FileSavePicker.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// Open file using fileOpenPicker. 
		/// Changes title, updates counters, setting flags and adds text to MainTextBox if file exists.
		/// </summary>
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

		// -------------- UPDATE UI ----------------
		/// <summary>
		/// Updates all counters based of text
		/// </summary>
		/// <param name="text">Usually MainTextBox.Text</param>
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
		/// <summary>
		/// The delimiters are used to split the string into different sub-strings, 
		/// allowing the strings in between the delimiters chars to be counted with .Length.
		/// </summary>
		/// <param name="text">MainTextBox.Text</param>
		private void update_WordCounter(string text)
		{
			char[] delimiters = new char[] {' ', '\r', '\n', '\t'};
			int wordCounter = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
			WordCounter.Text = wordCounter.ToString();
		}
		/// <summary>
		/// Same as update_WordCounter but only with new line characters
		/// </summary>
		/// <param name="text">MainTextBox.Text</param>
		private void update_RowCounter(string text)
		{
			char[] delimiters = new char[] { '\r', '\n' };
			int rowCounter = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
			RowCounter.Text = rowCounter.ToString();
		}
		/// <summary>
		/// Adds an asterix * to the title to indicate unsaved changes to a file.
		/// </summary>
		private void AddAsterixToTitle()
		{
			if (fileExists && !unsaved_changes)
			{
				changeTitle("*" + savefile.Name);
			}
			unsaved_changes = true;
		}
		/// <summary>
		/// Changes the title of the application.
		/// </summary>
		/// <param name="title">String to change title into</param>
		private void changeTitle(String title)
		{
			ApplicationView.GetForCurrentView().Title = title;
		}
	}
}
