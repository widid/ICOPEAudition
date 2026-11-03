using System.Collections.Generic;
using ICOPE.Immersive.Badges;
using ICOPE.Immersive.Editor.Catalog;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Performances;
using ICOPE.Immersive.Skills;
using ICOPE.Immersive.Values;
using UnityEditor;
using UnityEngine;
using WiDiD.Reflection.Tracking;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>Creates or updates the value assets and the reflected sources that read them.</summary>
    public static class TrackingAssetFactory
    {
        const string ResourcesRoot = "Assets/Resources";
        const string ValuesFolder = ResourcesRoot + "/" + TrackingValueAssets.ResourcesFolder;
        const string SourcesFolder = ValuesFolder + "/Sources";

        public static TrackingSourceSet EnsureAll(IReadOnlyList<CaseDefinition> cases)
        {
            EnsureFolder(SourcesFolder);

            AnswerContextSO answerContext = EnsureAsset<AnswerContextSO>($"{ValuesFolder}/{TrackingValueAssets.AnswerContextName}.asset");
            CaseResultSO caseResult = EnsureAsset<CaseResultSO>($"{ValuesFolder}/{TrackingValueAssets.CaseResultName}.asset");
            SkillBoardSO skillBoard = EnsureAsset<SkillBoardSO>($"{ValuesFolder}/{TrackingValueAssets.SkillBoardName}.asset");
            RewardLedgerSO rewardLedger = EnsureAsset<RewardLedgerSO>($"{ValuesFolder}/{TrackingValueAssets.RewardLedgerName}.asset");
            CoinLedgerSO coinLedger = EnsureAsset<CoinLedgerSO>($"{ValuesFolder}/{TrackingValueAssets.CoinLedgerName}.asset");
            GlobalStatsSO globalStats = EnsureAsset<GlobalStatsSO>($"{ValuesFolder}/{TrackingValueAssets.GlobalStatsName}.asset");
            PatientHistorySO patientHistory = EnsureAsset<PatientHistorySO>($"{ValuesFolder}/{TrackingValueAssets.PatientHistoryName}.asset");
            ProgressBoardSO progressBoard = EnsureAsset<ProgressBoardSO>($"{ValuesFolder}/{TrackingValueAssets.ProgressBoardName}.asset");

            skillBoard.Data.Resize(SkillCatalog.All.Count);
            progressBoard.Data.Resize(LevelBadgeCatalog.LevelKeys.Count);
            patientHistory.Data.Resize(cases.Count);
            EditorUtility.SetDirty(skillBoard);
            EditorUtility.SetDirty(progressBoard);
            EditorUtility.SetDirty(patientHistory);

            TrackingSourceSet sources = new TrackingSourceSet
            {
                AnswerText = EnsureSource<ReflectedStringSourceSO>(TrackingValueAssets.AnswerContextName, nameof(AnswerContextData.answerText), answerContext, TrackingValueAssets.Member(nameof(AnswerContextData.answerText))),
                RewardPoints = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.RewardLedgerName, nameof(RewardLedgerData.points), rewardLedger, TrackingValueAssets.Member(nameof(RewardLedgerData.points))),
                CoinBalance = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.CoinLedgerName, nameof(CoinLedgerData.balance), coinLedger, TrackingValueAssets.Member(nameof(CoinLedgerData.balance))),
                PatientsHandled = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.GlobalStatsName, nameof(GlobalStatsData.patientsHandled), globalStats, TrackingValueAssets.Member(nameof(GlobalStatsData.patientsHandled))),
                ReminderOpenings = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.GlobalStatsName, nameof(GlobalStatsData.reminderOpenings), globalStats, TrackingValueAssets.Member(nameof(GlobalStatsData.reminderOpenings))),
                SuccessRatePercent = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.GlobalStatsName, nameof(GlobalStatsData.successRatePercent), globalStats, TrackingValueAssets.Member(nameof(GlobalStatsData.successRatePercent))),
                SuccessRate = EnsureSource<ReflectedFloatSourceSO>(TrackingValueAssets.GlobalStatsName, nameof(GlobalStatsData.successRate), globalStats, TrackingValueAssets.Member(nameof(GlobalStatsData.successRate)))
            };

            foreach (CasePerformanceDefinition performance in CasePerformanceCatalog.All)
            {
                sources.CaseResultInts[performance.RawField] = EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.CaseResultName, performance.RawField, caseResult, TrackingValueAssets.Member(performance.RawField));

                if (performance.ScaledField != null)
                    sources.CaseResultFloats[performance.ScaledField] = EnsureSource<ReflectedFloatSourceSO>(TrackingValueAssets.CaseResultName, performance.ScaledField, caseResult, TrackingValueAssets.Member(performance.ScaledField));
            }

            for (int i = 0; i < SkillCatalog.All.Count; i++)
                sources.SkillScores.Add(EnsureSource<ReflectedFloatSourceSO>(TrackingValueAssets.SkillBoardName, SkillCatalog.All[i].Key, skillBoard, TrackingValueAssets.SkillScore(i)));

            for (int i = 0; i < LevelBadgeCatalog.LevelKeys.Count; i++)
                sources.LevelsCompleted.Add(EnsureSource<ReflectedBoolSourceSO>(TrackingValueAssets.ProgressBoardName, $"level{LevelBadgeCatalog.LevelKeys[i]}", progressBoard, TrackingValueAssets.LevelCompleted(i)));

            for (int i = 0; i < cases.Count; i++)
            {
                string strategyId = FlowIds.Strategy(cases[i].CaseId);
                sources.PatientSecondsSpent.Add(EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.PatientHistoryName, $"{strategyId}.{nameof(PatientHistoryData.secondsSpent)}", patientHistory, TrackingValueAssets.PatientSecondsSpent(i)));
                sources.PatientTimesHandled.Add(EnsureSource<ReflectedIntSourceSO>(TrackingValueAssets.PatientHistoryName, $"{strategyId}.{nameof(PatientHistoryData.timesHandled)}", patientHistory, TrackingValueAssets.PatientTimesHandled(i)));
            }

            return sources;
        }

        static TSource EnsureSource<TSource>(string ownerName, string memberName, Object target, string memberPath) where TSource : ScriptableObject, IReflectedValueSource
        {
            TSource source = EnsureAsset<TSource>($"{SourcesFolder}/{ownerName}.{memberName}.asset");
            source.Data.DirectAsset = target;
            source.Data.MemberPath = memberPath;
            EditorUtility.SetDirty(source);
            return source;
        }

        static T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = path.Substring(0, path.LastIndexOf('/'));
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, path.Substring(parent.Length + 1));
        }
    }
}
