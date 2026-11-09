using System;
using ICOPE.Immersive.Values;
using UnityEngine;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>Counts wrong answers per step over one case.</summary>
    public class CaseAttemptLog
    {
        StepAttempts current;
        int diagnosticErrors;
        int actionErrors;
        int stepsFirstTry;
        int stepCount;

        public void Begin()
        {
            current = default;
            diagnosticErrors = 0;
            actionErrors = 0;
            stepsFirstTry = 0;
            stepCount = 0;
        }

        public void BeginStep()
        {
            current = default;
        }

        public void RecordAnswer(AnswerPhase phase, bool isCorrect)
        {
            if (!isCorrect)
                current = current.WithError(phase);
        }

        public StepAttempts CloseStep()
        {
            StepAttempts closed = current;

            stepCount++;
            diagnosticErrors += closed.DiagnosticErrors;
            actionErrors += closed.ActionErrors;
            if (closed.IsFirstTry)
                stepsFirstTry++;

            current = default;
            return closed;
        }

        public CaseResultData ToResult(TimeSpan duration)
        {
            float successRate = stepCount == 0 ? 0f : (float)stepsFirstTry / stepCount;

            return new CaseResultData
            {
                diagnosticErrors = diagnosticErrors,
                actionErrors = actionErrors,
                stepsFirstTry = stepsFirstTry,
                stepCount = stepCount,
                successRate = successRate,
                successRatePercent = Mathf.RoundToInt(successRate * 100f),
                durationMinutes = Mathf.RoundToInt((float)duration.TotalMinutes)
            };
        }
    }
}
