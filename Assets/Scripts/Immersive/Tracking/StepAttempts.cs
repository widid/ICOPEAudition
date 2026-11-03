namespace ICOPE.Immersive.Tracking
{
    /// <summary>Wrong answers given on one step, per phase.</summary>
    public readonly struct StepAttempts
    {
        public int DiagnosticErrors { get; }
        public int ActionErrors { get; }

        public bool IsFirstTry => DiagnosticErrors == 0 && ActionErrors == 0;

        public StepAttempts(int diagnosticErrors, int actionErrors)
        {
            DiagnosticErrors = diagnosticErrors;
            ActionErrors = actionErrors;
        }

        public StepAttempts WithError(AnswerPhase phase) => phase == AnswerPhase.Diagnostic
            ? new StepAttempts(DiagnosticErrors + 1, ActionErrors)
            : new StepAttempts(DiagnosticErrors, ActionErrors + 1);
    }
}
