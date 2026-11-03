using System.Threading.Tasks;
using ICOPE.Immersive.Badges;
using ICOPE.Immersive.Gateway;
using ICOPE.Immersive.Performances;
using ICOPE.Immersive.Progress;
using ICOPE.Immersive.Rewards;
using ICOPE.Immersive.Scoring;
using ICOPE.Immersive.Skills;
using ICOPE.Immersive.Tracking;
using ICOPE.Immersive.Values;
using UnityEngine;
using WiDiD.ImmersiveAPI.Trainings.GameFlow;

namespace ICOPE.Immersive.Bootstrap
{
    /// <summary>
    /// Composition root of the integration. Completes once TrainingRuntime has restored the stored
    /// progress. Outside the Editor a missing session never gets here: SessionGate closes the app.
    /// </summary>
    public static class ImmersiveServicesFactory
    {
        public static async Task<ImmersiveServices> CreateAsync(bool trackingEnabled)
        {
#if UNITY_EDITOR
            if (!TrainingRuntime.IsReady && !TrainingRuntime.IsInitializing)
                return Offline("No Immersive debug session in the Editor");
#endif
            await TrainingRuntime.WhenReady;

            if (!TrainingRuntime.IsReady)
                return Offline("TrainingRuntime failed to initialize");

            TrainingRuntimeGateway gateway = new TrainingRuntimeGateway();
            IPatientProgressProvider progress = new ImmersiveProgressProvider(gateway);
            TrackingValues values = TrackingValues.Load();

            if (values == null)
                return new ImmersiveServices(new NullCaseTracker(), progress, new CoinWallet(0));

            LevelCompletionBoard levels = new LevelCompletionBoard(gateway, values.ProgressBoard);
            levels.Refresh();

            GlobalStatsReporter globalStats = new GlobalStatsReporter(gateway, new GlobalSuccessRate(gateway), values.GlobalStats);
            globalStats.RefreshSuccessRate();

            if (!trackingEnabled)
            {
                Debug.LogWarning("[ImmersiveServices] Tracking disabled by a debug option (alwaysRight / canAccessAllLevel): no statement is sent.");
                return new ImmersiveServices(new NullCaseTracker(), progress, new CoinWallet(values.CoinLedger.Data.balance));
            }

            CaseOutcomeReporter outcomes = new CaseOutcomeReporter(gateway, new SkillScoreCalculator(gateway), new AverageScoreReward(gateway), globalStats, values);
            ImmersiveCaseTracker tracker = new ImmersiveCaseTracker(gateway, new BinaryStepScoringPolicy(), outcomes, levels, globalStats,
                new PatientHistoryReporter(gateway, gateway, values.PatientHistory), values.AnswerContext);

            return new ImmersiveServices(tracker, progress, new SyncedCoinWallet(gateway, values.CoinLedger));
        }

        static ImmersiveServices Offline(string reason)
        {
            Debug.LogWarning($"[ImmersiveServices] {reason}: every patient is open and nothing is tracked.");
            return new ImmersiveServices(new NullCaseTracker(), new UnrestrictedProgressProvider(), new CoinWallet(0));
        }
    }
}
