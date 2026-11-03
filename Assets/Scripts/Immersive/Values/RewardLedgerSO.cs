using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "RewardLedger", menuName = "ICOPE/Immersive/Reward Ledger")]
    public class RewardLedgerSO : ScriptableObject
    {
        [field: SerializeField] public RewardLedgerData Data { get; private set; } = new RewardLedgerData();
    }
}
