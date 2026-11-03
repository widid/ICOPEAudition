using UnityEngine;

namespace ICOPE.Immersive.Rewards
{
    /// <summary>Coins earned at the end of a case.</summary>
    public static class CoinReward
    {
        public static int ForSuccessRate(float successRatePercent) => Mathf.RoundToInt(successRatePercent / 4f);
    }
}
