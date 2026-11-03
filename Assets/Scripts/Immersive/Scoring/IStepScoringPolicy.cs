using ICOPE.Immersive.Tracking;

namespace ICOPE.Immersive.Scoring
{
    public interface IStepScoringPolicy
    {
        /// <summary>Scaled score in [0,1].</summary>
        float Score(StepAttempts attempts);
    }
}
