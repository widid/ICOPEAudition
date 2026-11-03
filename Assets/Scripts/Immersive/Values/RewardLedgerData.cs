using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Points of the score Reward: average best score of every patient, from 0 to AverageScoreReward.MaxPoints.</summary>
    [Serializable]
    public class RewardLedgerData
    {
        public int points;
    }
}
