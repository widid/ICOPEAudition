using System;
using UnityEngine;

namespace ICOPE.Immersive.Values
{
    /// <summary>The value assets the generated statementables read from at send time.</summary>
    public class TrackingValues
    {
        public AnswerContextSO AnswerContext { get; }
        public CaseResultSO CaseResult { get; }
        public SkillBoardSO SkillBoard { get; }
        public RewardLedgerSO RewardLedger { get; }
        public ProgressBoardSO ProgressBoard { get; }
        public CoinLedgerSO CoinLedger { get; }
        public GlobalStatsSO GlobalStats { get; }
        public PatientHistorySO PatientHistory { get; }

        TrackingValues(AnswerContextSO answerContext, CaseResultSO caseResult, SkillBoardSO skillBoard, RewardLedgerSO rewardLedger, ProgressBoardSO progressBoard, CoinLedgerSO coinLedger, GlobalStatsSO globalStats, PatientHistorySO patientHistory)
        {
            AnswerContext = answerContext;
            CaseResult = caseResult;
            SkillBoard = skillBoard;
            RewardLedger = rewardLedger;
            ProgressBoard = progressBoard;
            CoinLedger = coinLedger;
            GlobalStats = globalStats;
            PatientHistory = patientHistory;
        }

        /// <summary>Returns null and logs an error when any asset is missing.</summary>
        public static TrackingValues Load()
        {
            TrackingValues values = TryLoad();

            if (values == null)
                Debug.LogError($"[TrackingValues] Missing value assets under Resources/{TrackingValueAssets.ResourcesFolder}. Run ICOPE > ImmersiveAPI > Generate Training From Levels.");

            return values;
        }

        public static TrackingValues TryLoad()
        {
            AnswerContextSO answerContext = TrackingValueAssets.Load<AnswerContextSO>(TrackingValueAssets.AnswerContextName);
            CaseResultSO caseResult = TrackingValueAssets.Load<CaseResultSO>(TrackingValueAssets.CaseResultName);
            SkillBoardSO skillBoard = TrackingValueAssets.Load<SkillBoardSO>(TrackingValueAssets.SkillBoardName);
            RewardLedgerSO rewardLedger = TrackingValueAssets.Load<RewardLedgerSO>(TrackingValueAssets.RewardLedgerName);
            ProgressBoardSO progressBoard = TrackingValueAssets.Load<ProgressBoardSO>(TrackingValueAssets.ProgressBoardName);
            CoinLedgerSO coinLedger = TrackingValueAssets.Load<CoinLedgerSO>(TrackingValueAssets.CoinLedgerName);
            GlobalStatsSO globalStats = TrackingValueAssets.Load<GlobalStatsSO>(TrackingValueAssets.GlobalStatsName);
            PatientHistorySO patientHistory = TrackingValueAssets.Load<PatientHistorySO>(TrackingValueAssets.PatientHistoryName);

            if (answerContext == null || caseResult == null || skillBoard == null || rewardLedger == null || progressBoard == null || coinLedger == null || globalStats == null || patientHistory == null)
                return null;

            return new TrackingValues(answerContext, caseResult, skillBoard, rewardLedger, progressBoard, coinLedger, globalStats, patientHistory);
        }

        /// <summary>Clears every value, so nothing left in the assets from an earlier Editor play session is sent or restored over.</summary>
        public void Reset()
        {
            AnswerContext.Data.answerText = string.Empty;
            CaseResult.Data.CopyFrom(new CaseResultData());
            Array.Clear(SkillBoard.Data.scores, 0, SkillBoard.Data.scores.Length);
            RewardLedger.Data.points = 0;
            CoinLedger.Data.balance = 0;
            GlobalStats.Data.patientsHandled = 0;
            GlobalStats.Data.reminderOpenings = 0;
            GlobalStats.Data.successRatePercent = 0;
            GlobalStats.Data.successRate = 0f;
            Array.Clear(PatientHistory.Data.secondsSpent, 0, PatientHistory.Data.secondsSpent.Length);
            Array.Clear(PatientHistory.Data.timesHandled, 0, PatientHistory.Data.timesHandled.Length);
            Array.Clear(ProgressBoard.Data.levelsCompleted, 0, ProgressBoard.Data.levelsCompleted.Length);
        }
    }
}
