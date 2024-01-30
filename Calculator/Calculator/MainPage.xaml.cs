using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.UserDataAccounts.Provider;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Oskar Deubler, oskadeub100 @ Karlstads Universitet

namespace Calculator
{

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		enum CalcState
		{
			cleared,
			firstValueEntered,
			operatorEntered,
			secondValueEntered,
			executedEquation,
		}
		
		private CalcState calculatorState;
		private int val1, val2;
		private char op;
		
		public MainPage()
		{
			calculatorState = CalcState.cleared;
			val1 = 0;
			this.InitializeComponent();
		}

		/*------------- EVENT FUNCTIONS -------------*/

		/// <summary>
		/// Handles the event of clicking a numbered button
		/// </summary>
		/// <param name="sender">The button pressed</param>
		/// <param name="e"></param>
		private void CalculatorNumberButton_Click(object sender, RoutedEventArgs e)
		{
			int curr_num;
			var button = sender as Button;

			if (!int.TryParse(button.Content.ToString(), out curr_num))
			{
				OperationErrorOccured("Could not parse number button.");
			}
			else
			{
				NumberTBlock_set(curr_num);
			}

			// Change CalcState if cleared or operatorEntered
			if (calculatorState.Equals(CalcState.cleared) || calculatorState.Equals(CalcState.executedEquation))
			{
				calculatorState = CalcState.firstValueEntered;
				SetOperandButtonsEnabled(true);
			} else if (calculatorState.Equals(CalcState.operatorEntered) )
			{
				calculatorState = CalcState.secondValueEntered;
			}
		}

		/// <summary>
		/// Handles the event of clicking a operator button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CalculatorOperandButton_Click(object sender, RoutedEventArgs e)
		{
			char operand = getSenderOperand(sender);

			int curr_num;
			//TODO: Check if entered value is out of range
			if (!int.TryParse(NumberTBlock.Text, out curr_num))
			{
				OperationErrorOccured("Number out of range.");
			}
			else
			{
				switch (calculatorState)
				{
					case CalcState.firstValueEntered:
					case CalcState.executedEquation:
					case CalcState.cleared:
						val1 = curr_num;
						EquationTBlock.Text = NumberTBlock.Text + " " + operand;
						op = operand;
						break;

					case CalcState.secondValueEntered:
						val2 = curr_num;
						int result;
						if (!TryExecuteEquation(val1, op, val2, out result))
						{
							return;
						}
						else
						{
							calculatorState = CalcState.operatorEntered; //Måste sättas pga if-sats i NumberTBlock_set.
							NumberTBlock_set(result);
							EquationTBlock.Text = result.ToString() + " " + operand;
							op = operand;
							val1 = result;
						}
						break;

					case CalcState.operatorEntered:
						EquationTBlock.Text = NumberTBlock.Text + " " + operand;
						op = operand;
						break;
				}
				calculatorState = CalcState.operatorEntered;
			}
		}
		/// <summary>
		/// Handles the event of clicking the = button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CalculatorEqualsButton_Click(object sender, RoutedEventArgs e)
		{
			int result;
			int curr_num;
			if (!int.TryParse(NumberTBlock.Text, out curr_num))
			{
				OperationErrorOccured("Number out of range.");
			} else
			{
				if (!calculatorState.Equals(CalcState.executedEquation))
				{
					val2 = curr_num;
				}
				if (!TryExecuteEquation(val1, op, val2, out result))
				{
					return;
				}
				calculatorState = CalcState.operatorEntered;
				NumberTBlock_set(result);
				EquationTBlock.Text = $"{val1.ToString()} {op} {val2.ToString()} =";
				val1 = result;
				calculatorState = CalcState.executedEquation;
			}
		}
		/// <summary>
		/// Handles the event of clicking the backspace <- button.
		/// Takes the length of the current number in NumberTBlock into account, to realize the deletion of the last digit.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CalculatorBackspaceButton_Click(object sender, RoutedEventArgs e)
		{
			if(calculatorState.Equals(CalcState.firstValueEntered) || calculatorState.Equals(CalcState.secondValueEntered))
			{
				var button = sender as Button;

				String sNum = NumberTBlock.Text;
				if (sNum.Length == 1)
				{
					NumberTBlock.Text = "0";
					ClearCalculator();
				} else if (sNum.Length > 1) { 
					sNum = sNum.Remove(sNum.Length - 1, 1);
					NumberTBlock.Text = sNum;
				}
			}
		}
		/// <summary>
		/// Clears the calculator when C button is pressed on the calcualtor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CalculatorClearButton_Click(object sender, RoutedEventArgs e)
		{
			ClearCalculator();
			NumberTBlock_set(0);
			EquationTBlock.Text = "";
		}

		/// <summary>
		/// Tries to execute the operation (x operand y) and returns the result.
		/// </summary>
		/// <param name="x">value 1</param>
		/// <param name="operand"></param>
		/// <param name="y">value 2</param>
		/// <param name="result"></param>
		/// <returns></returns>
		private Boolean TryExecuteEquation(int x, char operand, int y, out int result)
		{
			try
			{
				switch (operand)
				{
					case '+': result = Add(x, y); return true;
					case '-': result = Sub(x, y); return true;
					case 'x': result = Mult(x, y); return true;
					case '/': result = Div(x, y); return true;
					default: throw new CalcOpException("Could not recognize operator");
				}
			}
			catch (CalcOpException e)
			{
				OperationErrorOccured(e.Message);
				result = 0;
				return false;
			}
		}
		/// <summary>
		/// Displays the error message to notify the user of an error
		/// </summary>
		/// <param name="msg">Error message</param>
		private void OperationErrorOccured(String msg)
		{
			NumberTBlock.Text = $"E: {msg}";
			EquationTBlock.Text = "Try something else!";
			SetOperandButtonsEnabled(false);
			ClearCalculator();
		}

		/*------------- MATH FUNCTIONS -------------*/

		/// <summary>
		/// Division of x and y. Handles division by zero and fractions out of range of int.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>int fraction</returns>
		/// <exception cref="CalcOpException"></exception>
		private int Div(int x, int y)
		{

			if (y == 0)
			{
				throw new CalcOpException("Divison by zero is not allowed.");
			}
			long tRes = (long)x / y;

			if (TryOutOfIntRange(tRes))
			{
				throw new CalcOpException("Fraction not in range.");
			}
			return (int)tRes;
		}
		/// <summary>
		/// Multiplication of x and y. Handles products out of range of int.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>int product</returns>
		/// <exception cref="CalcOpException"></exception>
		private int Mult(int x, int y)
		{

			//TODO: Maybe check for
			//if x and y positive, result should be positive
			//if x is pos, y is neg, result should be negative
			//if both neg, then result should be positive
			long tRes = (long)x * y;
			if (TryOutOfIntRange(tRes))
			{
				throw new CalcOpException("Product not in range.");
			}
			return (int)tRes;
		}
		/// <summary>
		/// Difference of x - y. Handles difference out of range of int.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>int difference</returns>
		/// <exception cref="CalcOpException"></exception>
		private int Sub(int x, int y)
		{
			long tRes = (long)x - y;
			if (TryOutOfIntRange(tRes))
			{
				throw new CalcOpException("Difference not in range.");
			}
			return (int)tRes;
		}
		/// <summary>
		/// Sum of x + y. Handles difference out of range of int.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>int sum</returns>
		/// <exception cref="CalcOpException"></exception>
		private int Add(int x, int y)
		{
			long tRes = (long)x + y;
			if (TryOutOfIntRange(tRes))
			{
				throw new CalcOpException("Sum not in range.");
			} else {
				return (int)tRes;
			}
			
		}
		/// <summary>
		/// Control if x is out of int range.
		/// </summary>
		/// <param name="x"></param>
		/// <returns>true if x out of range of int, otherwise false</returns>
		private Boolean TryOutOfIntRange(long x)
		{
			if (x > int.MaxValue || x < int.MinValue)
			{
				return true;
			}
			return false;
		}

		/*------------- XAML-Object manipulation functions -------------*/

		/// <summary>
		/// Sets the text of TextBlock with name NumberTBlock in MainPage.xaml. Uses .ToString() to parse int.
		/// </summary>
		/// <param name="number"></param>
		private void NumberTBlock_set(int number)
		{
			if (calculatorState.Equals(CalcState.operatorEntered) || calculatorState.Equals(CalcState.cleared) || calculatorState.Equals(CalcState.executedEquation))
			{
				NumberTBlock.Text = number.ToString();
			}
			else if (calculatorState.Equals(CalcState.firstValueEntered) || calculatorState.Equals(CalcState.secondValueEntered))
			{
				NumberTBlock.Text = NumberTBlock.Text + number.ToString();
			}
		}

		/// <summary>
		/// Get the operand of object sender. + - x /
		/// </summary>
		/// <param name="sender"></param>
		/// <returns>char of the button content</returns>
		/// <exception cref="Exception"></exception>
		private char getSenderOperand(object sender)
		{
			var operandBtn = sender as Button;
			char operand = char.Parse(operandBtn.Content.ToString().Substring(0));
			if (operand.Equals('+') || operand.Equals('-') || operand.Equals('x') || operand.Equals('/'))
			{
				return operand;
			}
			else
			{
				throw new Exception($"Error: Operand {operand} not supported.");
			}
		}
		/// <summary>
		/// Clears the calculator values and sets state to cleared.
		/// </summary>
		private void ClearCalculator()
		{
			val1 = 0; val2 = 0;
			calculatorState = CalcState.cleared;

		}
		/// <summary>
		/// Enables/Disables the operand buttons in the calculator
		/// </summary>
		/// <param name="state">true if on, false if off</param>
		private void SetOperandButtonsEnabled(Boolean state)
		{
			OperandBtnAdd.IsEnabled = state;
			OperandBtnSub.IsEnabled = state;
			OperandBtnMult.IsEnabled = state;
			OperandBtnDiv.IsEnabled = state;
		}

		/*------------- EXCEPTION FUNCTIONS -------------*/

		/// <summary>
		/// Error function for calculator operation exceptions.
		/// </summary>
		public class CalcOpException : Exception
		{
			public CalcOpException(String msg) : base(msg)
			{
			}
		}
	}
}
