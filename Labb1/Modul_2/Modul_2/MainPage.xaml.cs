using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture.Frames;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Modul_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public const int NumMax = 35;
        public const int NumMin = 1;
        public const int LotterySize = 7;
        List<int> numbers;
        public MainPage()
        {
            numbers = new List<int>(LotterySize);
            this.InitializeComponent();
        }

        /// <summary>
        /// Checks if input is valid
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        private async Task<bool> IsValidInput(TextBox textBox)
        {
            int number;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                await InvalidInput("Samtliga lottonummer måste fyllas i! Försök igen.");
                return false;
            }

            // Check type
            if (int.TryParse(textBox.Text, out number))
            {
                // Check if in range: numMin - numMax
                if (number < NumMin || number > NumMax)
                {
                    await InvalidInput("Endast heltal mellan 1 - 35 är tillåtet! Försök igen.");
                    return false;
                }
                //Check for doubled number in numbers
                if (numbers.Contains(number))
                {
                    await InvalidInput("Lottonummer måste vara unika! Försök igen.");
                    return false;
                }
                else
                {
                    numbers.Add(number);
                }
            }
            else
            {
                await InvalidInput("Endast heltal tillåtna i lottoraden! Försök igen.");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Displays a Dialog
        /// </summary>
        /// <param name="eMsg"></param>
        /// <returns></returns>
        private async Task InvalidInput(string eMsg)
        {
            MessageDialog msg = new MessageDialog(eMsg);
            msg.Commands.Add(new UICommand("OK"));
            await msg.ShowAsync();
        }
        /// <summary>
        /// Validates number of draws input
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        private async Task<bool> ValidateNoDraws(TextBox textBox)
        {
            int number;
            if (!int.TryParse(textBox.Text, out number))
            {
                await InvalidInput("Endast heltal tillåtna i fältet 'Antal Dragningar'! Försök igen.");
                return false;
            }
            else if (number < 0)
            {
                await InvalidInput("Fältet 'Antal Dragningar' måste vara ett positivt heltal");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Handles the button that starts the lottery numbers generator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartDraw(object sender, RoutedEventArgs e)
        {
            
            //Reset List numbers
            numbers.Clear();

            //Validate input of lottery numbers
            for (int i = 1; i <= LotterySize; i++)
            {
                //----- Kod från chatGPT ------ 
                TextBox textBox = lotteryRow.FindName("Num" + i) as TextBox;

                if (textBox == null || !await IsValidInput(textBox))
                {
                    return;
                }
                // -----------------------------
            }

            //Validate number of draws-field 'tbNoDraws'
            if (!await ValidateNoDraws(tbNoDraws))
            {
                return;
            }


            int noDraws = int.Parse(tbNoDraws.Text);

            DrawAndCount(noDraws);

            //Start drawing numbers
        }

        /// <summary>
        /// Draws lottery numbers and counts the amount of correct numbers of the user entered numbers
        /// </summary>
        /// <param name="n">Amount of draws</param>
        private void DrawAndCount(int n)
        {
            int fives = 0, sixes = 0, sevens = 0;

            int matches;
            List<int> LotteryDraw = new List<int>(LotterySize);
            for (int i = 0; i < n; i++)
            {
                matches = 0;
                LotteryDraw = GenerateDraw(LotterySize);
                matches = numbers.Count(element => LotteryDraw.Contains(element)); 

                switch(matches)
                {
                    case 5: fives++; break;
                    case 6: sixes++; break;
                    case 7: sevens++; break;
                }
            }

            updateAfterDraw(fives, sixes, sevens);

        }
        /// <summary>
        /// Updates the TextBoxes to display the statistics of the draws
        /// </summary>
        /// <param name="fives"></param>
        /// <param name="sixes"></param>
        /// <param name="sevens"></param>
        private void updateAfterDraw(int fives, int sixes, int sevens)
        {
            tb5.Text = fives.ToString();
            tb6.Text = sixes.ToString();
            tb7.Text = sevens.ToString();
        }

        /// <summary>
        /// Generates random numbers within NumMin - NumMax (+1 to include NumMax).
        /// </summary>
        /// <param name="n">Amount of numbers to draw</param>
        /// <returns></returns>
        private List<int> GenerateDraw(int n)
        {
            List<int> draws = new List<int>(n);
            Random rnd = new Random();
            for (int i = 0; i < n; i++) {
                int x;
                do
                {
                    x = rnd.Next(NumMin, NumMax+1);
                }
                while (draws.Contains(x));
                draws.Add(x);
            }
            return draws;
        }
    }
}
