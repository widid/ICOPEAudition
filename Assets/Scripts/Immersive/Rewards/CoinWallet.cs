namespace ICOPE.Immersive.Rewards
{
    /// <summary>In-memory coin balance, kept for the session only.</summary>
    public class CoinWallet : ICoinWallet
    {
        public int Balance { get; private set; }

        public CoinWallet(int balance)
        {
            Balance = balance;
        }

        public void Earn(int coins)
        {
            Balance += coins;
        }

        public bool TrySpend(int coins)
        {
            if (coins > Balance)
                return false;

            Balance -= coins;
            return true;
        }
    }
}
