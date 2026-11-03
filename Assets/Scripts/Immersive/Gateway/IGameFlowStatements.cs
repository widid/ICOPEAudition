using System.Threading.Tasks;
using WiDiD.ImmersiveAPI.Networking;

namespace ICOPE.Immersive.Gateway
{
    /// <summary>Statement sends of the active Training.</summary>
    public interface IGameFlowStatements
    {
        Task<StatementSendResult> LaunchStrategy(string strategyId);
        Task<StatementSendResult> ExitStrategy(string strategyId);
        Task<StatementSendResult> LaunchStep(string stepId);
        Task<StatementSendResult> PassStep(string stepId, float score);
        Task<StatementSendResult> CompleteStep(string stepId, float score);
        Task<StatementSendResult> ExitStep(string stepId);
        Task<StatementSendResult> SendInteraction(string interactionId, bool success);
        Task<StatementSendResult> ScorePerformance(string performanceId);
        Task<StatementSendResult> ScoreSkill(string skillId);
        Task<StatementSendResult> ScoreReward(string rewardId);
    }
}
