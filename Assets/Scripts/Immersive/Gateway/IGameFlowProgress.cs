using System.Collections.Generic;

namespace ICOPE.Immersive.Gateway
{
    /// <summary>Progress of the active Training, restored server progress included.</summary>
    public interface IGameFlowProgress
    {
        IReadOnlyList<string> StrategyIds { get; }
        IReadOnlyList<string> StepIds { get; }

        bool IsStrategyUnlocked(string strategyId);
        bool IsStrategyCompleted(string strategyId);
        float? GetStrategyBestScore(string strategyId);
        float? GetStepBestScore(string stepId);
    }
}
