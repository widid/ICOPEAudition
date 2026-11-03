using System.Collections.Generic;
using System.Linq;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Identity;

namespace ICOPE.Immersive.Skills
{
    /// <summary>
    /// Scores a skill as the average best score of every Training step it covers, unplayed steps
    /// counting as 0, so the skill grows with coverage and reaches 1 only when all are perfect.
    /// </summary>
    public class SkillScoreCalculator
    {
        readonly IGameFlowProgress progress;
        readonly Dictionary<string, string[]> stepIdsBySkill = new();

        public SkillScoreCalculator(IGameFlowProgress progress)
        {
            this.progress = progress;
        }

        public float Evaluate(SkillDefinition skill)
        {
            string[] stepIds = CoveredSteps(skill);

            if (stepIds.Length == 0)
                return 0f;

            return stepIds.Sum(stepId => progress.GetStepBestScore(stepId) ?? 0f) / stepIds.Length;
        }

        string[] CoveredSteps(SkillDefinition skill)
        {
            if (!stepIdsBySkill.TryGetValue(skill.Key, out string[] stepIds))
            {
                stepIds = progress.StepIds
                    .Where(stepId => FlowIds.TryGetStepType(stepId, out string stepType) && skill.Covers(stepType))
                    .ToArray();

                stepIdsBySkill[skill.Key] = stepIds;
            }

            return stepIds;
        }
    }
}
