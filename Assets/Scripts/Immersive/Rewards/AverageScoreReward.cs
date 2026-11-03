using System.Linq;
using ICOPE.Immersive.Gateway;
using UnityEngine;

namespace ICOPE.Immersive.Rewards
{
    /// <summary>
    /// Points of the score Reward: the average best score of every patient, unplayed patients
    /// counting as 0, scaled to MaxPoints.
    /// </summary>
    public class AverageScoreReward
    {
        public const int MaxPoints = 1000;

        readonly IGameFlowProgress progress;

        public AverageScoreReward(IGameFlowProgress progress)
        {
            this.progress = progress;
        }

        /// <param name="justCompletedStrategyId">A strategy whose score the runtime may not have recorded yet.</param>
        /// <param name="justCompletedScore">Score of that strategy, in [0,1].</param>
        public int Points(string justCompletedStrategyId, float justCompletedScore)
        {
            if (progress.StrategyIds.Count == 0)
                return 0;

            float average = progress.StrategyIds.Average(id => BestScore(id, justCompletedStrategyId, justCompletedScore));
            return Mathf.RoundToInt(Mathf.Clamp01(average) * MaxPoints);
        }

        float BestScore(string strategyId, string justCompletedStrategyId, float justCompletedScore)
        {
            float best = progress.GetStrategyBestScore(strategyId) ?? 0f;
            return strategyId == justCompletedStrategyId ? Mathf.Max(best, justCompletedScore) : best;
        }
    }
}
