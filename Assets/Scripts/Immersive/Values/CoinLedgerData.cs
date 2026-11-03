using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Coin balance read by the coins Performance, restored from the server at startup.</summary>
    [Serializable]
    public class CoinLedgerData
    {
        public int balance;
    }
}
