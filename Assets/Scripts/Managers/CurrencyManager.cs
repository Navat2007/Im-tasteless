using System;

namespace Managers
{
    public static class CurrencyManager
    {
        public static event Action<int> OnGoldChange;
        public static event Action<int> OnLevelGoldChange;
        public static event Action<int> OnGemsChange;
        
        private static int _gold;
        private static int _levelGold;
        private static int _gems;

        public static int GetCurrency(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLD:
                    return _gold;
                case CurrencyType.LEVEL_GOLD:
                    return _levelGold;
                case CurrencyType.GEMS:
                    return _gems;
                default:
                    return 0;
            }
        }
        
        public static void AddCurrency(CurrencyType currencyType, int amount)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLD:
                    _gold += amount;
                    OnGoldChange?.Invoke(_gold);
                    break;
                case CurrencyType.LEVEL_GOLD:
                    _levelGold += amount;
                    OnLevelGoldChange?.Invoke(_levelGold);
                    break;
                case CurrencyType.GEMS:
                    _gems += amount;
                    OnGemsChange?.Invoke(_gold);
                    break;
            }
        }
        
        public static void SetCurrency(CurrencyType currencyType, int value)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLD:
                    _gold = value;
                    OnGoldChange?.Invoke(_gold);
                    break;
                case CurrencyType.LEVEL_GOLD:
                    _levelGold = value;
                    OnLevelGoldChange?.Invoke(_levelGold);
                    break;
                case CurrencyType.GEMS:
                    _gems = value;
                    OnGemsChange?.Invoke(_gold);
                    break;
            }
        }
    }

    public enum CurrencyType
    {
        GOLD,
        LEVEL_GOLD,
        GEMS
    }
}