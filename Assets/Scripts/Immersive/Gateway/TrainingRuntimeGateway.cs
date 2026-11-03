using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiDiD.ImmersiveAPI.Networking;
using WiDiD.ImmersiveAPI.Trainings.GameFlow;

namespace ICOPE.Immersive.Gateway
{
    public class TrainingRuntimeGateway : IGameFlowStatements, IGameFlowProgress
    {
        public IReadOnlyList<string> StrategyIds { get; }
        public IReadOnlyList<string> StepIds { get; }

        public TrainingRuntimeGateway()
        {
            List<TrainingStrategyData> strategies = TrainingRuntime.ActiveTraining.Data.GameFlow.Strategies;

            StrategyIds = strategies.Select(strategy => strategy.Id).ToArray();
            StepIds = strategies
                .SelectMany(strategy => strategy.Steps)
                .Select(step => step.Id)
                .ToArray();
        }

        public Task<StatementSendResult> LaunchStrategy(string strategyId) => TrainingRuntime.LaunchStrategy(strategyId);
        public Task<StatementSendResult> ExitStrategy(string strategyId) => TrainingRuntime.ExitStrategy(strategyId);
        public Task<StatementSendResult> LaunchStep(string stepId) => TrainingRuntime.LaunchStep(stepId);
        public Task<StatementSendResult> PassStep(string stepId, float score) => TrainingRuntime.PassStep(stepId, score);
        public Task<StatementSendResult> CompleteStep(string stepId, float score) => TrainingRuntime.CompleteStep(stepId, score);
        public Task<StatementSendResult> ExitStep(string stepId) => TrainingRuntime.ExitStep(stepId);
        public Task<StatementSendResult> SendInteraction(string interactionId, bool success) => TrainingRuntime.SendInteraction(interactionId, success);
        public Task<StatementSendResult> ScorePerformance(string performanceId) => TrainingRuntime.ScorePerformance(performanceId);
        public Task<StatementSendResult> ScoreSkill(string skillId) => TrainingRuntime.ScoreSkill(skillId);
        public Task<StatementSendResult> ScoreReward(string rewardId) => TrainingRuntime.ScoreReward(rewardId);

        public bool IsStrategyUnlocked(string strategyId) => TrainingRuntime.IsStrategyUnlocked(strategyId);
        public bool IsStrategyCompleted(string strategyId) => TrainingRuntime.IsStrategyCompleted(strategyId);
        public float? GetStrategyBestScore(string strategyId) => TrainingRuntime.GetStrategyBestScore(strategyId);
        public float? GetStepBestScore(string stepId) => TrainingRuntime.GetStepBestScore(stepId);
    }
}
