using System.Collections.Generic;
using ICOPE.Immersive.Editor.Catalog;
using ICOPE.Immersive.Identity;
using WiDiD.ImmersiveAPI.Trainings.GameFlow.Building;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>
    /// One Strategy per patient chained in manifest order, one Step per AlgoStep chained in case
    /// order. The last step completes its Strategy, the last patient completes the Simulation.
    /// </summary>
    public static class GameFlowStructureBuilder
    {
        public static GameFlowBuildResult Build(IReadOnlyList<CaseDefinition> cases)
        {
            IGameFlowBuilderService builder = GameFlowBuilderService.Create()
                .SimulationPassingScore(FlowRules.SimulationPassingScore);

            string previousStrategyId = null;

            for (int i = 0; i < cases.Count; i++)
            {
                CaseDefinition patient = cases[i];
                string strategyId = FlowIds.Strategy(patient.CaseId);
                string after = previousStrategyId;
                bool completesSimulation = i == cases.Count - 1;

                builder.Strategy(strategyId, strategy => ConfigureStrategy(strategy, patient, after, completesSimulation));
                previousStrategyId = strategyId;
            }

            return builder.Build();
        }

        static void ConfigureStrategy(IStrategyBuilder strategy, CaseDefinition patient, string after, bool completesSimulation)
        {
            strategy
                .Named("fr", $"{LevelNameLabels.Fr(patient.LevelName)} - {patient.FirstName}")
                .Named("en", $"{LevelNameLabels.En(patient.LevelName)} - {patient.FirstName}")
                .PassingScore(FlowRules.StrategyPassingScore)
                .CompletesSimulation(completesSimulation);

            if (after != null)
                strategy.After(after);

            string previousStepId = null;

            foreach (CaseStepDefinition step in patient.Steps)
            {
                string stepId = FlowIds.Step(patient.CaseId, step.Index, step.StepType);
                string stepAfter = previousStepId;
                bool completesStrategy = step.Index == patient.Steps.Count - 1;

                strategy.Step(stepId, stepBuilder => ConfigureStep(stepBuilder, step, stepAfter, completesStrategy));
                previousStepId = stepId;
            }
        }

        static void ConfigureStep(IStepBuilder stepBuilder, CaseStepDefinition step, string after, bool completesStrategy)
        {
            stepBuilder
                .Named("fr", $"{step.Index + 1}. {StepTypeLabels.Fr(step.StepType)}")
                .Named("en", $"{step.Index + 1}. {StepTypeLabels.En(step.StepType)}")
                .PassingScore(FlowRules.StepPassingScore)
                .CompletesStrategy(completesStrategy);

            if (after != null)
                stepBuilder.After(after);
        }
    }
}
