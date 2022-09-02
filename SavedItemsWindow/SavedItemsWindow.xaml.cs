using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace TradeHelper
{
    /// <summary>
    /// Interaction logic for audioSourceWindow.xaml
    /// </summary>
    public partial class SavedItemsWindow : Window
    {
        // The whitespaces are defined by the maximum length properties of the text boxes in the main window.

        #region Main Methods
        private void AddListBoxHeader()
        {
            SavedItemsListBox.Items.Add(new Item() { ItemName = TheConstants.ITEM_NAME_STRING, PurchaseValue = TheConstants.PURCHASE_VALUE_STRING, PurchaseDateString = TheConstants.PURCHASE_DATE_STRING });
        }

        private void AddListBoxItems()
        {
            List<string> ItemsFailedToConvert = new List<string>();
            // This list stores the items failed to convert into the right view and deletes them after the rest of the items converted successfully.

            // Because the older saved items are in smaller indices, reverse the for loop to show the saved items from newer to older.
            for (int index = Properties.Settings.Default.AllSavedItems.Count - 1; index >= 0; index--)
            {
                string savedItem = Properties.Settings.Default.AllSavedItems[index];

                try
                {
                    string[] savedItemSplit = savedItem.Split(char.Parse(TheConstants.SEPERATION_STRING));

                    savedItemSplit[1] = TheConstants.DOLLAR_SIGN + savedItemSplit[1];

                    // If the date is today or yesterday, change it to be more legible.
                    savedItemSplit[2] = ChangeDateToLegibleString(DateTime.Parse(savedItemSplit[2]));

                    SavedItemsListBox.Items.Add(new Item() { ItemName = savedItemSplit[0], PurchaseValue = savedItemSplit[1], PurchaseDateString = savedItemSplit[2] });
                }

                catch
                {
                    ItemsFailedToConvert.Add(savedItem);
                    // If converting fails, don't add the newly created item instance to the list and remove it from the default variable.
                }
            }

            foreach (string failedItemName in ItemsFailedToConvert)
            {
                Properties.Settings.Default.AllSavedItems.Remove(failedItemName);

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
        }

        private void DeleteSelectedItems()
        {
            while (SavedItemsListBox.SelectedItems.Count > 0)
            {
                if (IsSelectedIndexTheHeader())
                {
                    SavedItemsListBox.SelectedItems.RemoveAt(SavedItemsListBox.SelectedIndex);
                    // Remove the header from the selection

                    continue;
                }

                Properties.Settings.Default.AllSavedItems.RemoveAt(Properties.Settings.Default.AllSavedItems.Count - SavedItemsListBox.SelectedIndex);
                // Because we reversed the for loop for adding items chronologically to the list box, we can't just get the selected index because it will refer to a different index in the default variable.
                // Instead, we get the index from the end.
                // For example, you want to delete the item below the header in the list. The indexing in the list starts from 1 because there is the header at the index 0.
                // And there are 6 items in the list box.
                // Because the for loop was reversed to show the items in the list box chronologically, the data for the item isn't actually at index 2.
                // It's in index 4. The data's stored from older to newer in the default variable.

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                SavedItemsListBox.Items.RemoveAt(SavedItemsListBox.SelectedIndex);
            }
        }

        private float SumUsdEquivalentCoinValuesOfSelectedItems()
        {
            if (SavedItemsListBox.SelectedItems.Count > 0)
            {
                float sum = 0.0f;

                foreach (Item item in SavedItemsListBox.SelectedItems)
                {
                    if (IsItemTheHeader(item))
                    {
                        continue;
                    }

                    sum += float.Parse(item.PurchaseValue.Replace(TheConstants.DOLLAR_SIGN, string.Empty), CultureInfo.InvariantCulture);
                }

                return sum;
            }

            return 0.0f;
        }
        #endregion

        #region Helper Methods
        private string ChangeDateToLegibleString(DateTime currentDate)
        {
            if (DateTime.Today - currentDate == TimeSpan.FromDays(0))
            {
                return TheConstants.TODAY_STRING;
            }

            else if (DateTime.Today - currentDate == TimeSpan.FromDays(1))
            {
                return TheConstants.YESTERDAY_STRING;
            }

            else
            {
                return currentDate.ToString("d");
            }
        }

        private string ReplaceDecimalCommaWithDecimalDot(string str)
        {
            return str.Replace(",", TheConstants.DECIMAL_POINT_STRING);
        }

        private bool IsItemTheHeader(Item item)
        {
            if (item.ItemName == TheConstants.ITEM_NAME_STRING)
            {
                return true;
            }

            return false;
        }

        private bool IsSelectedIndexTheHeader()
        {
            if (SavedItemsListBox.SelectedIndex == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        private class Item
        {
            public string ItemName { get; set; }

            public string PurchaseValue { get; set; }

            public string PurchaseDateString { get; set; }
        }

        public SavedItemsWindow()
        {
            InitializeComponent();

            AddListBoxHeader();

            AddListBoxItems();
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        private void CalculateProfitValuesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(TheConstants.SUM_OF_USD_EQUIVALENT_COIN_VALUES_STRING + " : " + TheConstants.DOLLAR_SIGN + ReplaceDecimalCommaWithDecimalDot(SumUsdEquivalentCoinValuesOfSelectedItems().ToString()), Application.Current.MainWindow.Title);
        }
    }
}
