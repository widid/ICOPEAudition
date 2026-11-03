using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "CoinLedger", menuName = "ICOPE/Immersive/Coin Ledger")]
    public class CoinLedgerSO : ScriptableObject
    {
        [field: SerializeField] public CoinLedgerData Data { get; private set; } = new CoinLedgerData();
    }
}
