namespace ICOPE.Immersive.Tracking
{
    /// <summary>Identifies one step of a clinical case by its position in the case.</summary>
    public readonly struct StepRef
    {
        public string CaseId { get; }
        public int Index { get; }
        public string StepType { get; }
        public bool IsLast { get; }

        public StepRef(string caseId, int index, string stepType, bool isLast)
        {
            CaseId = caseId;
            Index = index;
            StepType = stepType;
            IsLast = isLast;
        }
    }
}
