namespace ICOPE.Immersive.Progress
{
    public interface IPatientProgressProvider
    {
        bool IsUnlocked(string caseId);
        bool IsCompleted(string caseId);

        /// <summary>Best scaled score in [0,1], or null when never completed.</summary>
        float? BestScore(string caseId);
    }
}
