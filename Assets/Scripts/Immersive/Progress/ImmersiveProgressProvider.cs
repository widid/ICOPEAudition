using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;

namespace ICOPE.Immersive.Progress
{
    /// <summary>Patient progress read from the Training, seeded with the server's stored progress.</summary>
    public class ImmersiveProgressProvider : IPatientProgressProvider
    {
        readonly IGameFlowProgress progress;

        public ImmersiveProgressProvider(IGameFlowProgress progress)
        {
            this.progress = progress;
        }

        public bool IsUnlocked(string caseId) => progress.IsStrategyUnlocked(FlowIds.Strategy(caseId));
        public bool IsCompleted(string caseId) => progress.IsStrategyCompleted(FlowIds.Strategy(caseId));
        public float? BestScore(string caseId) => progress.GetStrategyBestScore(FlowIds.Strategy(caseId));
    }
}
