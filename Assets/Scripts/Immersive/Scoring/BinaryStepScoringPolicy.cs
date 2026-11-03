using ICOPE.Immersive.Tracking;

namespace ICOPE.Immersive.Scoring
{
    /// <summary>1 when every phase was answered right first time, 0 otherwise.</summary>
    public class BinaryStepScoringPolicy : IStepScoringPolicy
    {
        public float Score(StepAttempts attempts) => attempts.IsFirstTry ? 1f : 0f;
    }
}
