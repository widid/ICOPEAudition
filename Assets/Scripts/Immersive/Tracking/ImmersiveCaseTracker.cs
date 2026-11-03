using System;
using System.Collections.Generic;
using ICOPE.Immersive.Badges;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Scoring;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>
    /// Turns the case lifecycle into GameFlow statements: patient = Strategy, AlgoStep = Step,
    /// answer phase = Interaction. Strategy and Simulation outcomes follow automatically from the
    /// last CompleteStep. Sends run through one SerialTaskQueue so they leave in call order.
    /// </summary>
    public class ImmersiveCaseTracker : IClinicalCaseTracker
    {
        readonly IGameFlowStatements statements;
        readonly IStepScoringPolicy scoring;
        readonly CaseOutcomeReporter outcomes;
        readonly LevelCompletionBoard levels;
        readonly GlobalStatsReporter globalStats;
        readonly PatientHistoryReporter history;
        readonly AnswerContextSO answerContext;
        readonly CaseAttemptLog attempts = new();
        readonly SerialTaskQueue queue = new();

        string activeStrategyId;
        string activeStepId;
        DateTime caseStartedAt;
        HashSet<string> caseStepTypes = new();

        public ImmersiveCaseTracker(IGameFlowStatements statements, IStepScoringPolicy scoring, CaseOutcomeReporter outcomes, LevelCompletionBoard levels, GlobalStatsReporter globalStats, PatientHistoryReporter history, AnswerContextSO answerContext)
        {
            this.statements = statements;
            this.scoring = scoring;
            this.outcomes = outcomes;
            this.levels = levels;
            this.globalStats = globalStats;
            this.history = history;
            this.answerContext = answerContext;
        }

        public void CaseStarted(string caseId)
        {
            CaseAbandoned();

            string strategyId = FlowIds.Strategy(caseId);
            activeStrategyId = strategyId;
            caseStartedAt = DateTime.UtcNow;
            caseStepTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            attempts.Begin();

            queue.Enqueue(() => statements.LaunchStrategy(strategyId));
        }

        public void StepStarted(StepRef step)
        {
            if (activeStrategyId == null)
                return;

            string stepId = FlowIds.Step(step.CaseId, step.Index, step.StepType);
            activeStepId = stepId;
            caseStepTypes.Add(step.StepType);
            attempts.BeginStep();

            queue.Enqueue(() => statements.LaunchStep(stepId));
        }

        public void AnswerSubmitted(StepRef step, AnswerPhase phase, string answerText, bool isCorrect)
        {
            if (activeStepId == null)
                return;

            attempts.RecordAnswer(phase, isCorrect);
            string interactionId = FlowIds.Interaction(activeStepId, phase);

            queue.Enqueue(() =>
            {
                answerContext.Data.answerText = answerText;
                return statements.SendInteraction(interactionId, isCorrect);
            });
        }

        public void StepFinished(StepRef step)
        {
            if (activeStepId == null)
                return;

            string stepId = activeStepId;
            float score = scoring.Score(attempts.CloseStep());
            activeStepId = null;

            // TEMPORARY-STORED-PROGRESS: the backend marks a step succeeded, and so restores it, only on Passed.
            queue.Enqueue(async () =>
            {
                await statements.PassStep(stepId, score);
                await statements.CompleteStep(stepId, score);
            });

            if (step.IsLast)
                FinishCase();
        }

        public void CaseAbandoned()
        {
            if (activeStrategyId == null)
                return;

            string strategyId = activeStrategyId;
            string stepId = activeStepId;
            TimeSpan duration = DateTime.UtcNow - caseStartedAt;
            activeStrategyId = null;
            activeStepId = null;

            queue.Enqueue(async () =>
            {
                if (stepId != null)
                    await statements.ExitStep(stepId);

                await statements.ExitStrategy(strategyId);
                await history.ReportAttempt(strategyId, duration);
            });
        }

        public void ReminderOpened()
        {
            queue.Enqueue(globalStats.ReportReminderOpened);
        }

        void FinishCase()
        {
            string strategyId = activeStrategyId;
            TimeSpan duration = DateTime.UtcNow - caseStartedAt;
            CaseResultData result = attempts.ToResult(duration);
            IReadOnlyCollection<string> stepTypes = caseStepTypes;
            activeStrategyId = null;

            queue.Enqueue(async () =>
            {
                await outcomes.Report(strategyId, result, stepTypes);
                await history.ReportAttempt(strategyId, duration);
                levels.Refresh(strategyId);
            });
        }
    }
}
