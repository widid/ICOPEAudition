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
    /// Adds each attempt, finished or left, to its patient's total time and handled count, then sends
    /// both Strategy-scoped Performances. Totals are restored from the server at startup.
    /// </summary>
    public class PatientHistoryReporter
    {
        readonly IGameFlowStatements statements;
        readonly IGameFlowProgress progress;
        readonly PatientHistorySO history;

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
            history.Data.secondsSpent[index] += Mathf.RoundToInt((float)duration.TotalSeconds);
            history.Data.timesHandled[index]++;

            await statements.ScorePerformance(FlowIds.PatientTimeSpent(strategyId));
            await statements.ScorePerformance(FlowIds.PatientTimesHandled(strategyId));
        }
    }
}
