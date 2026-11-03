using System.Collections.Generic;
using System.Threading.Tasks;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Performances;
using ICOPE.Immersive.Rewards;
using ICOPE.Immersive.Skills;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>Sends the end-of-case Performances, the global Performances, the Skills the case exercised and the score Reward.</summary>
    public class CaseOutcomeReporter
    {
        readonly IGameFlowStatements statements;
        readonly SkillScoreCalculator skills;
        readonly AverageScoreReward scoreReward;
        readonly GlobalStatsReporter globalStats;
        readonly TrackingValues values;

        public CaseOutcomeReporter(IGameFlowStatements statements, SkillScoreCalculator skills, AverageScoreReward scoreReward, GlobalStatsReporter globalStats, TrackingValues values)
        {
            this.statements = statements;
            this.skills = skills;
            this.scoreReward = scoreReward;
            this.globalStats = globalStats;
            this.values = values;
        }

        public async Task Report(string strategyId, CaseResultData result, IReadOnlyCollection<string> stepTypes)
        {
            values.CaseResult.Data.CopyFrom(result);

            foreach (CasePerformanceDefinition performance in CasePerformanceCatalog.All)
                await statements.ScorePerformance(FlowIds.Performance(strategyId, performance.Key));

            await globalStats.ReportCaseFinished(strategyId, result.successRate);

            for (int i = 0; i < SkillCatalog.All.Count; i++)
            {
                SkillDefinition skill = SkillCatalog.All[i];
                if (!skill.CoversAny(stepTypes))
                    continue;

                values.SkillBoard.Data.Set(i, skills.Evaluate(skill));
                await statements.ScoreSkill(skill.Id);
            }

            values.RewardLedger.Data.points = scoreReward.Points(strategyId, result.successRate);
            await statements.ScoreReward(FlowIds.ScoreReward);
        }
    }
}
