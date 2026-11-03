namespace ICOPE.Immersive.Tracking
{
    public class NullCaseTracker : IClinicalCaseTracker
    {
        public void CaseStarted(string caseId) { }
        public void StepStarted(StepRef step) { }
        public void AnswerSubmitted(StepRef step, AnswerPhase phase, string answerText, bool isCorrect) { }
        public void StepFinished(StepRef step) { }
        public void CaseAbandoned() { }
        public void ReminderOpened() { }
    }
}
