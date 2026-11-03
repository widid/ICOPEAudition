namespace ICOPE.Immersive.Rewards
{
    public interface ICoinWallet
    {
        int Balance { get; }

        void Earn(int coins);

        /// <summary>Returns false, spending nothing, when the balance is too low.</summary>
        bool TrySpend(int coins);
    }
}
