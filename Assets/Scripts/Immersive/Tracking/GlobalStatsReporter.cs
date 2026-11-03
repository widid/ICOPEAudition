using System.Threading.Tasks;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Performances;
using ICOPE.Immersive.Values;
using UnityEngine;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>
    /// Updates and sends the Simulation-scoped global Performances. Patients handled and reminder
    /// openings are restored from the server at startup; the success rate is derived from progress.
    /// </summary>
    public class GlobalStatsReporter
    {
        readonly IGameFlowStatements statements;
        readonly GlobalSuccessRate successRate;
        readonly GlobalStatsSO stats;

        public GlobalStatsReporter(IGameFlowStatements statements, GlobalSuccessRate successRate, GlobalStatsSO stats)
        {
            this.statements = statements;
            this.successRate = successRate;
            this.stats = stats;
        }

        /// <param name="justCompletedStrategyId">A strategy whose completion and score the runtime may not have recorded yet.</param>
        /// <param name="justCompletedScore">Score of that strategy, in [0,1].</param>
        public void RefreshSuccessRate(string justCompletedStrategyId = null, float justCompletedScore = 0f)
        {
            float rate = successRate.Evaluate(justCompletedStrategyId, justCompletedScore);
            stats.Data.successRate = rate;
            stats.Data.successRatePercent = Mathf.RoundToInt(rate * 100f);
        }

        public async Task ReportCaseFinished(string strategyId, float score)
        {
            stats.Data.patientsHandled++;
            RefreshSuccessRate(strategyId, score);

            await statements.ScorePerformance(FlowIds.PatientsHandledPerformance);
            await statements.ScorePerformance(FlowIds.GlobalSuccessRatePerformance);
        }

        public Task ReportReminderOpened()
        {
            stats.Data.reminderOpenings++;
            return statements.ScorePerformance(FlowIds.ReminderOpeningsPerformance);
        }
    }
}
