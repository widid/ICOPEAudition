using System.Linq;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Badges
{
    /// <summary>Writes which badge levels have every patient completed into the ProgressBoard the badge conditions read.</summary>
    public class LevelCompletionBoard
    {
        readonly IGameFlowProgress progress;
        readonly ProgressBoardSO board;

        public LevelCompletionBoard(IGameFlowProgress progress, ProgressBoardSO board)
        {
            this.progress = progress;
            this.board = board;
        }

        /// <param name="justCompletedStrategyId">A strategy whose completion the runtime may not have recorded yet.</param>
        public void Refresh(string justCompletedStrategyId = null)
        {
            board.Data.Resize(LevelBadgeCatalog.LevelKeys.Count);

            for (int i = 0; i < LevelBadgeCatalog.LevelKeys.Count; i++)
            {
                string levelKey = LevelBadgeCatalog.LevelKeys[i];
                string[] strategyIds = progress.StrategyIds.Where(id => FlowIds.LevelKey(id) == levelKey).ToArray();

                board.Data.levelsCompleted[i] = strategyIds.Length > 0
                    && strategyIds.All(id => id == justCompletedStrategyId || progress.IsStrategyCompleted(id));
            }
        }
    }
}
