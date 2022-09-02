using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TradeHelper
{
    class ViewModel : INotifyPropertyChanged
    {
        DataModel dataModel = new DataModel();

        public RelayCommand ResetAllTextBoxesButtonCommand { get; private set; }

        public RelayCommand SaveCurrentItemButtonCommand { get; private set; }

        public ViewModel()
        {
            ResetAllTextBoxesButtonCommand = new RelayCommand(ResetAllTextBoxes, CanResetAllTextBoxes);
            SaveCurrentItemButtonCommand = new RelayCommand(SaveCurrentItem, CanSaveCurrentItem);
        }

        #region Helper Methods
        private string ReplaceDecimalCommaWithDecimalDot(string str)
        {
            return str.Replace(",", TheConstants.DECIMAL_POINT_STRING);
        }
        #endregion

        #region Button Commands

        private void ResetAllTextBoxes(object obj)
        {
            ItemName = string.Empty;
            CoinValueString = string.Empty;
            MarketValueString = string.Empty;
        }

        private bool CanResetAllTextBoxes(object obj)
        {
            if (string.IsNullOrEmpty(ItemName) && string.IsNullOrEmpty(CoinValueString) && string.IsNullOrEmpty(MarketValueString))
            {
                return false;
            }

            return true;
        }

        private void SaveCurrentItem(object obj)
        {
            ItemName = ItemName.Trim();

            Properties.Settings.Default.AllSavedItems.Add(ItemName + TheConstants.SEPERATION_STRING + ReplaceDecimalCommaWithDecimalDot(ProfitValue.ToString()) + TheConstants.SEPERATION_STRING + ReplaceDecimalCommaWithDecimalDot(UsdEquivalentToCoinValue.ToString()) + TheConstants.SEPERATION_STRING + ReplaceDecimalCommaWithDecimalDot(MarketValueString.ToString()) + TheConstants.SEPERATION_STRING + ReplaceDecimalCommaWithDecimalDot(MarketValueWithoutFee.ToString()) + TheConstants.SEPERATION_STRING + DateTime.Today.ToString("d"));

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private bool CanSaveCurrentItem(object obj)
        {
            if (string.IsNullOrEmpty(ItemName) || UsdEquivalentToCoinValue.Equals(0.0f) || MarketValueWithoutFee.Equals(0.0f))
            {
                return false;
            }

            return true;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of properties.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void INotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        public string ItemName
        {
            get => dataModel.itemName;
            set
            {
                if (value != dataModel.itemName)
                {
                    dataModel.itemName = value;
                    INotifyPropertyChanged();
                }
            }
        }

        public string CoinValueString
        {
            get => dataModel.coinValueString;
            set
            {
                if (value != dataModel.coinValueString)
                {
                    dataModel.coinValueString = value;

                    INotifyPropertyChanged();
                    INotifyPropertyChanged(nameof(UsdEquivalentToCoinValue));
                    INotifyPropertyChanged(nameof(ProfitValue));
                }
            }
        }

        public float UsdEquivalentToCoinValue => dataModel.GetUsdEquivalentToCoinValue();

        public string MarketValueString
        {
            get => dataModel.marketValueString;
            set
            {
                if (value != dataModel.marketValueString)
                {
                    dataModel.marketValueString = value;

                    INotifyPropertyChanged();
                    INotifyPropertyChanged(nameof(MarketValueWithoutFee));
                    INotifyPropertyChanged(nameof(ProfitValue));
                }
            }
        }

        public float MarketValueWithoutFee => dataModel.GetMarketValueIncludingFee();

        public float ProfitValue => dataModel.GetProfitValue();
        #endregion
    }
}
