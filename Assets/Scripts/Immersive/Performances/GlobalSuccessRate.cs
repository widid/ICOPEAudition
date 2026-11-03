using System.Linq;
using ICOPE.Immersive.Gateway;
using UnityEngine;

namespace ICOPE.Immersive.Performances
{
    /// <summary>Average best score of every completed patient, in [0,1]; 0 when none is completed.</summary>
    public class GlobalSuccessRate
    {
        readonly IGameFlowProgress progress;

        public GlobalSuccessRate(IGameFlowProgress progress)
        {
            this.progress = progress;
        }

        /// <param name="justCompletedStrategyId">A strategy whose completion and score the runtime may not have recorded yet.</param>
        /// <param name="justCompletedScore">Score of that strategy, in [0,1].</param>
        public float Evaluate(string justCompletedStrategyId = null, float justCompletedScore = 0f)
        {
            float[] scores = progress.StrategyIds
                .Where(id => id == justCompletedStrategyId || progress.IsStrategyCompleted(id))
                .Select(id => BestScore(id, justCompletedStrategyId, justCompletedScore))
                .ToArray();

            return scores.Length == 0 ? 0f : Mathf.Clamp01(scores.Average());
        }

        float BestScore(string strategyId, string justCompletedStrategyId, float justCompletedScore)
        {
            float best = progress.GetStrategyBestScore(strategyId) ?? 0f;
            return strategyId == justCompletedStrategyId ? Mathf.Max(best, justCompletedScore) : best;
        }
    }
}
