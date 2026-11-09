using System;
using System.Linq;
using System.Threading.Tasks;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Values;
using UnityEngine;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>
    /// Adds each attempt, finished or left, to its patient's total minutes and handled count, then sends
    /// both Strategy-scoped Performances. Totals are restored from the server at startup.
    /// </summary>
    public class PatientHistoryReporter
    {
        const int SecondsPerMinute = 60;

        readonly IGameFlowStatements statements;
        readonly IGameFlowProgress progress;
        readonly PatientHistorySO history;
        int[] pendingSeconds = new int[0];

        public PatientHistoryReporter(IGameFlowStatements statements, IGameFlowProgress progress, PatientHistorySO history)
        {
            this.statements = statements;
            this.progress = progress;
            this.history = history;
        }

        public async Task ReportAttempt(string strategyId, TimeSpan duration)
        {
            int index = progress.StrategyIds.ToList().IndexOf(strategyId);
            if (index < 0)
                return;

            history.Data.Resize(progress.StrategyIds.Count);
            AddDuration(index, duration);
            history.Data.timesHandled[index]++;

            await statements.ScorePerformance(FlowIds.PatientTimeSpent(strategyId));
            await statements.ScorePerformance(FlowIds.PatientTimesHandled(strategyId));
        }

        // Sub-minute leftovers carry over to the patient's next attempt in this session.
        void AddDuration(int index, TimeSpan duration)
        {
            if (pendingSeconds.Length != history.Data.minutesSpent.Length)
                Array.Resize(ref pendingSeconds, history.Data.minutesSpent.Length);

            pendingSeconds[index] += Mathf.RoundToInt((float)duration.TotalSeconds);
            history.Data.minutesSpent[index] += pendingSeconds[index] / SecondsPerMinute;
            pendingSeconds[index] %= SecondsPerMinute;
        }
    }
}
