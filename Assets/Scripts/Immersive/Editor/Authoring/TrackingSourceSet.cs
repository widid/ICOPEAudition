using System.Collections.Generic;
using WiDiD.Reflection.Tracking;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>Reflected sources pointing at the tracking value assets.</summary>
    public class TrackingSourceSet
    {
        public ReflectedStringSourceSO AnswerText { get; set; }

        /// <summary>CaseResultData int fields, keyed by field name.</summary>
        public Dictionary<string, ReflectedIntSourceSO> CaseResultInts { get; } = new();

        /// <summary>CaseResultData float fields, keyed by field name.</summary>
        public Dictionary<string, ReflectedFloatSourceSO> CaseResultFloats { get; } = new();

        /// <summary>Indexed like SkillCatalog.All.</summary>
        public List<ReflectedFloatSourceSO> SkillScores { get; } = new();

        /// <summary>Indexed like LevelBadgeCatalog.LevelKeys.</summary>
        public List<ReflectedBoolSourceSO> LevelsCompleted { get; } = new();

        public ReflectedIntSourceSO RewardPoints { get; set; }
        public ReflectedIntSourceSO CoinBalance { get; set; }
        public ReflectedIntSourceSO PatientsHandled { get; set; }
        public ReflectedIntSourceSO ReminderOpenings { get; set; }
        public ReflectedIntSourceSO SuccessRatePercent { get; set; }
        public ReflectedFloatSourceSO SuccessRate { get; set; }

        /// <summary>Indexed like the generated cases.</summary>
        public List<ReflectedIntSourceSO> PatientSecondsSpent { get; } = new();

        /// <summary>Indexed like the generated cases.</summary>
        public List<ReflectedIntSourceSO> PatientTimesHandled { get; } = new();
    }
}
