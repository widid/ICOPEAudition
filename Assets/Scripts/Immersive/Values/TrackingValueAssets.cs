using UnityEngine;

namespace ICOPE.Immersive.Values
{
    /// <summary>Resources location and reflected member paths of the tracking value assets.</summary>
    public static class TrackingValueAssets
    {
        public const string ResourcesFolder = "ImmersiveAPI/Values";
        public const string AnswerContextName = "AnswerContext";
        public const string CaseResultName = "CaseResult";
        public const string SkillBoardName = "SkillBoard";
        public const string RewardLedgerName = "RewardLedger";
        public const string CoinLedgerName = "CoinLedger";
        public const string GlobalStatsName = "GlobalStats";
        public const string PatientHistoryName = "PatientHistory";
        public const string ProgressBoardName = "ProgressBoard";

        const string DataMember = "Data";

        public static string Member(string fieldName) => $"{DataMember}/{fieldName}";

        public static string SkillScore(int index) => $"{DataMember}/{nameof(SkillBoardData.scores)}[{index}]";

        public static string PatientSecondsSpent(int index) => $"{DataMember}/{nameof(PatientHistoryData.secondsSpent)}[{index}]";

        public static string PatientTimesHandled(int index) => $"{DataMember}/{nameof(PatientHistoryData.timesHandled)}[{index}]";

        public static string LevelCompleted(int index) => $"{DataMember}/{nameof(ProgressBoardData.levelsCompleted)}[{index}]";

        public static T Load<T>(string assetName) where T : ScriptableObject => Resources.Load<T>($"{ResourcesFolder}/{assetName}");
    }
}
