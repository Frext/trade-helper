using System;
using System.Globalization;

namespace TradeHelper
{
    class DataModel
    {
        public string itemName = string.Empty;

        public string coinValueString = string.Empty;
        public string marketValueString = string.Empty;

        private float GetCoinValue()
        {
            try { return (float)Math.Round(float.Parse(coinValueString, CultureInfo.InvariantCulture), TheConstants.FRACTIONAL_DIGITS); }

            catch { return 0.0f; }
        }

        private float GetMarketValue()
        {
            try { return (float)Math.Round(float.Parse(marketValueString, CultureInfo.InvariantCulture), TheConstants.FRACTIONAL_DIGITS); }

            catch { return 0.0f; }
        }

        public float GetUsdEquivalentToCoinValue()
        {
            return (float)Math.Round(GetCoinValue() * TheConstants.USD_EQUIVALENT_TO_ONE_COIN,
TheConstants.FRACTIONAL_DIGITS);
        }

        public float GetMarketValueIncludingFee()
        {
            return (float)Math.Round(GetMarketValue() * TheConstants.MARKET_FEE,
TheConstants.FRACTIONAL_DIGITS);
        }

        public float GetProfitValue()
        {
            return (float)Math.Round(GetMarketValueIncludingFee() - GetUsdEquivalentToCoinValue(), TheConstants.FRACTIONAL_DIGITS);
        }
    }
}
