using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "GlobalStats", menuName = "ICOPE/Immersive/Global Stats")]
    public class GlobalStatsSO : ScriptableObject
    {
        [field: SerializeField] public GlobalStatsData Data { get; private set; } = new GlobalStatsData();
    }
}
