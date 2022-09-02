using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TradeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        #region Helper Methods

        private void ReplaceSeperationStringWithEmpty(TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;

            textBox.Text = textBox.Text.Replace(TheConstants.SEPERATION_STRING, string.Empty);

            textBox.CaretIndex = caretIndex;
        }
        
        private void ReplaceNewlineStringWithEmpty(TextBox textBox)
        {
            int caretIndex = textBox.CaretIndex;

            textBox.Text = textBox.Text.Replace(TheConstants.NEWLINE_STRING, string.Empty);

            textBox.CaretIndex = caretIndex;
        }

        private void FocusOnTextBox(TextBox textBox)
        {
            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void HideMainWindow()
        {
            Hide();
        }

        private void ShowMainWindow(object sender, EventArgs e)
        {
            Show();
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel();

            // Go into the UI thread and focus on the textbox at the top at startup.

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                FocusOnTextBox(ItemNameTextBox);
            }));
        }

        #region Item Name Input
        private void ItemNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusOnTextBox(CoinValueTextBox);

                e.Handled = true;
            }
        }

        private void ItemNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ReplaceSeperationStringWithEmpty(ItemNameTextBox);
            ReplaceNewlineStringWithEmpty(ItemNameTextBox);
        }

        #endregion

        #region Coin Value Input

        private void CoinValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(TheConstants.DECIMAL_POINT_STRING) && CoinValueTextBox.Text.Contains(TheConstants.DECIMAL_POINT_STRING))    // Allow entering only one decimal point.
            {
                e.Handled = true;
            }
            else
            {
                Regex regex = new Regex(@"[^0-9:.]+$");

                e.Handled = regex.IsMatch(e.Text);
            }
        }

        private void CoinValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusOnTextBox(MarketValueTextBox);

                e.Handled = true;
            }
        }

        #endregion

        #region Market Value Input

        private void MarketValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(TheConstants.DECIMAL_POINT_STRING) && MarketValueTextBox.Text.Contains(TheConstants.DECIMAL_POINT_STRING))    // Enter only one dot.
            {
                e.Handled = true;
            }
            else
            {
                Regex regex = new Regex(@"[^0-9:.]+$");

                e.Handled = regex.IsMatch(e.Text);
            }
        }

        #endregion

        #region Button Click Events

        private void ResetAllTextBoxesButton_Click(object sender, RoutedEventArgs e)
        {
            FocusOnTextBox(ItemNameTextBox);
        }

        private void SaveItemButton_Click(object sender, RoutedEventArgs e)
        {
            FocusOnTextBox(ItemNameTextBox);
        }

        private void ShowSavedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            SavedItemsWindow savedItemsWindow = new SavedItemsWindow();
            savedItemsWindow.Show();

            HideMainWindow();

            savedItemsWindow.Closed += ShowMainWindow;
        }
        #endregion


    }
}
