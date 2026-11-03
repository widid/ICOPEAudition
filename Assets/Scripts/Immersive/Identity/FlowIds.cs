using ICOPE.Immersive.Tracking;

namespace ICOPE.Immersive.Identity
{
    /// <summary>Builds the GameFlow ids shared by the generated Training and the runtime tracker.</summary>
    public static class FlowIds
    {
        public const string TrainingId = "ICOPE_Audition";
        public const string ScoreReward = "reward.score";
        public const string CoinsPerformance = "coins";
        public const string PatientsHandledPerformance = "patients_handled";
        public const string ReminderOpeningsPerformance = "reminder_openings";
        public const string GlobalSuccessRatePerformance = "global_success_rate";

        const char Separator = '.';
        const char StepPrefix = 's';
        const char StepTypeSeparator = '_';

        public static string Strategy(string caseId) => caseId.ToLowerInvariant();

        /// <summary>Level prefix of a case or strategy id, e.g. "1" for 1_3_denise.</summary>
        public static string LevelKey(string caseOrStrategyId)
        {
            int separator = caseOrStrategyId.IndexOf(StepTypeSeparator);
            return separator > 0 ? caseOrStrategyId.Substring(0, separator) : caseOrStrategyId;
        }

        public static string Step(string caseId, int index, string stepType)
            => $"{Strategy(caseId)}{Separator}{StepPrefix}{index:00}{StepTypeSeparator}{stepType.ToLowerInvariant()}";

        public static string Interaction(string stepId, AnswerPhase phase) => $"{stepId}{Separator}{phase.ToString().ToLowerInvariant()}";

        public static string Performance(string strategyId, string performanceKey) => $"{strategyId}{Separator}{performanceKey}";

        public static string PatientTimeSpent(string strategyId) => Performance(strategyId, "time_spent");

        public static string PatientTimesHandled(string strategyId) => Performance(strategyId, "times_handled");

        public static string Skill(string skillKey) => $"skill{Separator}{skillKey}";

        /// <summary>Reads back the lowercased step type of an id built by Step.</summary>
        public static bool TryGetStepType(string stepId, out string stepType)
        {
            stepType = null;

            if (string.IsNullOrEmpty(stepId))
                return false;

            string segment = stepId.Substring(stepId.LastIndexOf(Separator) + 1);
            int typeStart = segment.IndexOf(StepTypeSeparator);

            if (segment.Length == 0 || segment[0] != StepPrefix || typeStart < 0)
                return false;

            stepType = segment.Substring(typeStart + 1);
            return stepType.Length > 0;
        }
    }
}
