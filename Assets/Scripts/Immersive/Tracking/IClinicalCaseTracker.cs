namespace ICOPE.Immersive.Tracking
{
    /// <summary>Lifecycle of a clinical case as the game plays it.</summary>
    public interface IClinicalCaseTracker
    {
        void CaseStarted(string caseId);
        void StepStarted(StepRef step);
        void AnswerSubmitted(StepRef step, AnswerPhase phase, string answerText, bool isCorrect);
        void StepFinished(StepRef step);

        /// <summary>Leaves the case in progress, if any.</summary>
        void CaseAbandoned();

        /// <summary>The reminder ("Rappel") panel was opened.</summary>
        void ReminderOpened();
    }
}
