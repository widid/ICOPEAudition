using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Rewards
{
    /// <summary>
    /// Coin balance held in the CoinLedger the coins Performance reads. The package restores it from
    /// the server at startup and sends it before the session terminates; each change is scored at once too.
    /// </summary>
    public class SyncedCoinWallet : ICoinWallet
    {
        readonly IGameFlowStatements statements;
        readonly CoinLedgerSO ledger;

        public int Balance => ledger.Data.balance;

        public SyncedCoinWallet(IGameFlowStatements statements, CoinLedgerSO ledger)
        {
            this.statements = statements;
            this.ledger = ledger;
        }

        public void Earn(int coins)
        {
            SetBalance(Balance + coins);
        }

        public bool TrySpend(int coins)
        {
            if (coins > Balance)
                return false;

            SetBalance(Balance - coins);
            return true;
        }

        void SetBalance(int balance)
        {
            ledger.Data.balance = balance;
            _ = statements.ScorePerformance(FlowIds.CoinsPerformance);
        }
    }
}
