namespace ICOPE.Immersive.Editor.Catalog
{
    public class CaseStepDefinition
    {
        public int Index { get; }
        public string StepType { get; }
        public bool HasDiagnostic { get; }
        public bool HasAction { get; }

        public CaseStepDefinition(int index, string stepType, bool hasDiagnostic, bool hasAction)
        {
            Index = index;
            StepType = stepType;
            HasDiagnostic = hasDiagnostic;
            HasAction = hasAction;
        }
    }
}
