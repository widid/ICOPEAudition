using System.Collections.Generic;
using System.Linq;
using ICOPE.Immersive.Badges;
using ICOPE.Immersive.Editor.Catalog;
using ICOPE.Immersive.Identity;
using ICOPE.Immersive.Performances;
using ICOPE.Immersive.Rewards;
using ICOPE.Immersive.Skills;
using ICOPE.Immersive.Tracking;
using UnityEngine;
using WiDiD.ImmersiveAPI.Trainings.GameFlow;
using WiDiD.Reflection.Tracking;
using WiDiD.XAPI;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>Adds the Interactions, Performances, Skills, Reward and level Badges to an installed GameFlow.</summary>
    public static class StatementableAuthoring
    {
        const string ChoiceInteractionType = "Choice";
        const string FallbackInteractionType = "Other";

        public static void Apply(TrainingGameFlowData gameFlow, IReadOnlyList<CaseDefinition> cases, TrackingSourceSet sources)
        {
            for (int i = 0; i < cases.Count; i++)
            {
                CaseDefinition patient = cases[i];
                TrainingStrategyData strategy = gameFlow.Strategies.First(s => s.Id == FlowIds.Strategy(patient.CaseId));
                strategy.Performances = BuildPerformances(strategy.Id, patient, sources);
                strategy.Performances.Add(BuildTimeSpentPerformance(strategy.Id, sources.PatientMinutesSpent[i]));
                strategy.Performances.Add(BuildTimesHandledPerformance(strategy.Id, sources.PatientTimesHandled[i]));

                foreach (CaseStepDefinition step in patient.Steps)
                {
                    TrainingStepData stepData = strategy.Steps.First(s => s.Id == FlowIds.Step(patient.CaseId, step.Index, step.StepType));
                    stepData.Interactions = BuildInteractions(stepData.Id, patient, step, sources);
                }
            }

            gameFlow.Performances = new List<TrainingPerformanceData>
            {
                BuildCoinsPerformance(sources),
                BuildPatientsHandledPerformance(sources),
                BuildReminderOpeningsPerformance(sources),
                BuildGlobalSuccessRatePerformance(sources)
            };
            gameFlow.Skills = BuildSkills(sources);
            gameFlow.Rewards = new List<TrainingRewardData> { BuildScoreReward(sources) };
            gameFlow.Badges = BuildLevelBadges(cases, sources);
        }

        static List<TrainingBadgeData> BuildLevelBadges(IReadOnlyList<CaseDefinition> cases, TrackingSourceSet sources)
        {
            List<TrainingBadgeData> badges = new List<TrainingBadgeData>();

            for (int i = 0; i < LevelBadgeCatalog.LevelKeys.Count; i++)
            {
                string levelKey = LevelBadgeCatalog.LevelKeys[i];
                CaseDefinition firstCase = cases.FirstOrDefault(patient => FlowIds.LevelKey(patient.CaseId) == levelKey);

                if (firstCase == null)
                {
                    Debug.LogWarning($"[StatementableAuthoring] No patient in level '{levelKey}': its badge never unlocks.");
                    continue;
                }

                string levelFr = LevelNameLabels.Fr(firstCase.LevelName);
                string levelEn = LevelNameLabels.En(firstCase.LevelName);

                badges.Add(new TrainingBadgeData
                {
                    Name = LocalizedText.Of($"Niveau {levelFr} terminé", $"{levelEn} level completed"),
                    Description = LocalizedText.Of($"Tous les cas du niveau {levelFr} sont terminés.", $"Every case of the {levelEn} level is completed."),
                    UnlockCondition = IsTrue(sources.LevelsCompleted[i])
                });
            }

            return badges;
        }

        static ConditionChain IsTrue(ReflectedBoolSourceSO source) => new ConditionChain
        {
            Entries = new List<ConditionChainEntry>
            {
                new ConditionChainEntry
                {
                    Condition = new Condition
                    {
                        ValueType = ConditionValueType.Bool,
                        Operator = ComparisonOperator.Equals,
                        BoolLeft = new PerformanceBoolValueSource { Enabled = true, ReflectedSource = source },
                        BoolRight = new PerformanceBoolValueSource { Enabled = true, FixedValue = true }
                    }
                }
            }
        };

        static List<TrainingInteractionData> BuildInteractions(string stepId, CaseDefinition patient, CaseStepDefinition step, TrackingSourceSet sources)
        {
            List<TrainingInteractionData> interactions = new List<TrainingInteractionData>();

            if (step.HasDiagnostic)
                interactions.Add(BuildInteraction(stepId, AnswerPhase.Diagnostic, patient, step, "Analyse", "Analysis", sources));

            if (step.HasAction)
                interactions.Add(BuildInteraction(stepId, AnswerPhase.Action, patient, step, "Conduite à tenir", "Action", sources));

            return interactions;
        }

        static TrainingInteractionData BuildInteraction(string stepId, AnswerPhase phase, CaseDefinition patient, CaseStepDefinition step, string phaseFr, string phaseEn, TrackingSourceSet sources) => new TrainingInteractionData
        {
            Id = FlowIds.Interaction(stepId, phase),
            Name = LocalizedText.Of(
                $"{patient.FirstName} - {StepTypeLabels.Fr(step.StepType)} - {phaseFr}",
                $"{patient.FirstName} - {StepTypeLabels.En(step.StepType)} - {phaseEn}"),
            Kind = InteractionKind.AnsweredQuestion,
            InteractionTypeKey = InteractionTypes.All.ContainsKey(ChoiceInteractionType) ? ChoiceInteractionType : FallbackInteractionType,
            Response = new PerformanceStringValueSource { Enabled = true, ReflectedSource = sources.AnswerText }
        };

        static List<TrainingPerformanceData> BuildPerformances(string strategyId, CaseDefinition patient, TrackingSourceSet sources)
        {
            List<TrainingPerformanceData> performances = new List<TrainingPerformanceData>();

            foreach (CasePerformanceDefinition definition in CasePerformanceCatalog.All)
            {
                int? max = definition.MaxIsStepCount ? patient.Steps.Count : definition.FixedMax;

                performances.Add(new TrainingPerformanceData
                {
                    Id = FlowIds.Performance(strategyId, definition.Key),
                    Name = LocalizedText.Of(definition.NameFr, definition.NameEn),
                    Unit = definition.Unit,
                    Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.CaseResultInts[definition.RawField] },
                    Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 },
                    Max = new PerformanceIntValueSource { Enabled = max.HasValue, FixedValue = max ?? 0 },
                    Scaled = definition.ScaledField == null
                        ? new PerformanceValueSource()
                        : new PerformanceValueSource { Enabled = true, ReflectedSource = sources.CaseResultFloats[definition.ScaledField] }
                });
            }

            return performances;
        }

        // Synced with the server: the stored total is applied at startup and sent again before the session terminates.
        static TrainingPerformanceData BuildTimeSpentPerformance(string strategyId, ReflectedIntSourceSO minutesSpent) => new TrainingPerformanceData
        {
            Id = FlowIds.PatientTimeSpent(strategyId),
            Name = LocalizedText.Of("Passées au total sur ce patient", "Spent on this patient in total"),
            Unit = "min",
            SyncWithServer = true,
            ApplyServerValue = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = minutesSpent },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 }
        };

        // Synced with the server: the stored count is applied at startup and sent again before the session terminates.
        static TrainingPerformanceData BuildTimesHandledPerformance(string strategyId, ReflectedIntSourceSO timesHandled) => new TrainingPerformanceData
        {
            Id = FlowIds.PatientTimesHandled(strategyId),
            Name = LocalizedText.Of("Pris en charge au total", "Handled in total"),
            Unit = "x",
            SyncWithServer = true,
            ApplyServerValue = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = timesHandled },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 }
        };

        static List<TrainingSkillData> BuildSkills(TrackingSourceSet sources)
        {
            List<TrainingSkillData> skills = new List<TrainingSkillData>();

            for (int i = 0; i < SkillCatalog.All.Count; i++)
            {
                SkillDefinition skill = SkillCatalog.All[i];

                skills.Add(new TrainingSkillData
                {
                    Id = skill.Id,
                    Name = LocalizedText.Of(skill.NameFr, skill.NameEn),
                    MainCategoryKey = nameof(SkillCategories.HardSkill),
                    SubCategoryId = skill.Key,
                    SubCategoryName = LocalizedText.Of(skill.NameFr, skill.NameEn),
                    Scaled = new PerformanceValueSource { Enabled = true, ReflectedSource = sources.SkillScores[i] }
                });
            }

            return skills;
        }

        // Synced with the server: the stored balance is applied at startup and sent again before the session terminates.
        static TrainingPerformanceData BuildCoinsPerformance(TrackingSourceSet sources) => new TrainingPerformanceData
        {
            Id = FlowIds.CoinsPerformance,
            Name = LocalizedText.Of("Dans votre porte-monnaie", "In your wallet"),
            Unit = "pièces",
            SyncWithServer = true,
            ApplyServerValue = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.CoinBalance },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 }
        };

        // Synced with the server: the stored count is applied at startup and sent again before the session terminates.
        static TrainingPerformanceData BuildPatientsHandledPerformance(TrackingSourceSet sources) => new TrainingPerformanceData
        {
            Id = FlowIds.PatientsHandledPerformance,
            Name = LocalizedText.Of("Pris en charge au total", "Handled in total"),
            Unit = "patients",
            SyncWithServer = true,
            ApplyServerValue = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.PatientsHandled },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 }
        };

        // Synced with the server: the stored count is applied at startup and sent again before the session terminates.
        static TrainingPerformanceData BuildReminderOpeningsPerformance(TrackingSourceSet sources) => new TrainingPerformanceData
        {
            Id = FlowIds.ReminderOpeningsPerformance,
            Name = LocalizedText.Of("Sur le bouton Rappel", "On the Reminder button"),
            Unit = "appuis",
            SyncWithServer = true,
            ApplyServerValue = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.ReminderOpenings },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 }
        };

        // Derived from restored progress, so the stored value is not applied; sent again before the session terminates.
        static TrainingPerformanceData BuildGlobalSuccessRatePerformance(TrackingSourceSet sources) => new TrainingPerformanceData
        {
            Id = FlowIds.GlobalSuccessRatePerformance,
            Name = LocalizedText.Of("De réussite globale", "Overall success rate"),
            Description = LocalizedText.Of("Meilleur score moyen de tous les patients terminés.", "Average best score of every completed patient."),
            Unit = "%",
            SyncWithServer = true,
            SendOnClose = true,
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.SuccessRatePercent },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 },
            Max = new PerformanceIntValueSource { Enabled = true, FixedValue = 100 },
            Scaled = new PerformanceValueSource { Enabled = true, ReflectedSource = sources.SuccessRate }
        };

        static TrainingRewardData BuildScoreReward(TrackingSourceSet sources) => new TrainingRewardData
        {
            Id = FlowIds.ScoreReward,
            Name = LocalizedText.Of("Score moyen", "Average score"),
            Description = LocalizedText.Of("Score moyen de tous les patients, les patients non joués comptant 0.", "Average score of every patient, unplayed patients counting as 0."),
            Raw = new PerformanceIntValueSource { Enabled = true, ReflectedSource = sources.RewardPoints },
            Min = new PerformanceIntValueSource { Enabled = true, FixedValue = 0 },
            Max = new PerformanceIntValueSource { Enabled = true, FixedValue = AverageScoreReward.MaxPoints }
        };
    }
}
