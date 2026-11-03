namespace ICOPE.Immersive.Progress
{
    /// <summary>Every patient open, nothing completed - used when no Immersive session exists (Editor only).</summary>
    public class UnrestrictedProgressProvider : IPatientProgressProvider
    {
        public bool IsUnlocked(string caseId) => true;
        public bool IsCompleted(string caseId) => false;
        public float? BestScore(string caseId) => null;
    }
}
