using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using ICOPE.Immersive.Identity;
using UnityEditor;
using UnityEngine;
using WiDiD.ImmersiveAPI.Editor.Trainings.GameFlow;
using WiDiD.ImmersiveAPI.Trainings;
using WiDiD.ImmersiveAPI.Trainings.GameFlow;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>
    /// Rewrites a Training's Editor graph (GameFlowGraph and its nested StepGraphs) from a built
    /// GameFlow. The graph's own Sync then rebuilds Training.Strategies, so the graph and the data
    /// always match and later graph edits keep what was generated.
    /// </summary>
    public static class GameFlowGraphWriter
    {
        const string PreviousPort = nameof(StrategyNode.previous);
        const string NextPort = nameof(StrategyNode.next);
        static readonly Vector2 StrategySpacing = new(320f, 220f);
        const float StepSpacing = 300f;

        public static void Write(TrainingSO training, TrainingGameFlowData gameFlow)
        {
            GameFlowGraph graph = TrainingGraphAssets.FindOrCreate(training);
            Clear(graph);

            Dictionary<string, StrategyNode> strategyNodes = new Dictionary<string, StrategyNode>();
            Dictionary<string, int> strategiesPerLevel = new Dictionary<string, int>();

            foreach (TrainingStrategyData strategy in gameFlow.Strategies)
            {
                string levelKey = FlowIds.LevelKey(strategy.Id);
                strategiesPerLevel.TryGetValue(levelKey, out int column);
                strategiesPerLevel[levelKey] = column + 1;
                int row = strategiesPerLevel.Count - 1;

                StrategyNode node = BaseNode.CreateFromType<StrategyNode>(new Vector2(column * StrategySpacing.x, row * StrategySpacing.y));
                node.customId = strategy.Id;
                node.strategyName = strategy.Name;
                node.strategyDescription = strategy.Description;
                node.passingScore = strategy.PassingScore;
                node.requireAllPreviousCompleted = strategy.RequireAllPreviousCompleted;
                node.requireAllPreviousPassed = strategy.RequireAllPreviousPassed;
                node.completeSimulation = strategy.CompleteSimulation;

                // GameFlowGraph creates the nested StepGraph (with one default step) on AddNode.
                graph.AddNode(node);
                WriteSteps(node.stepGraph, strategy.Steps);
                strategyNodes[strategy.Id] = node;
            }

            foreach (TrainingStrategyData strategy in gameFlow.Strategies)
            {
                foreach (string previousId in strategy.Previous)
                    Link(graph, strategyNodes[previousId], strategyNodes[strategy.Id]);
            }

            graph.Sync();
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
        }

        static void WriteSteps(StepGraph stepGraph, List<TrainingStepData> steps)
        {
            foreach (BaseNode defaultNode in stepGraph.nodes.ToList())
                stepGraph.RemoveNode(defaultNode);

            Dictionary<string, StepNode> stepNodes = new Dictionary<string, StepNode>();

            for (int i = 0; i < steps.Count; i++)
            {
                TrainingStepData step = steps[i];

                StepNode node = BaseNode.CreateFromType<StepNode>(new Vector2(i * StepSpacing, 0f));
                node.customId = step.Id;
                node.stepName = step.Name;
                node.stepDescription = step.Description;
                node.passingScore = step.PassingScore;
                node.requireAllPreviousCompleted = step.RequireAllPreviousCompleted;
                node.requireAllPreviousPassed = step.RequireAllPreviousPassed;
                node.triggersStrategyCompletion = step.TriggersStrategyCompletion;

                stepGraph.AddNode(node);
                stepNodes[step.Id] = node;
            }

            foreach (TrainingStepData step in steps)
            {
                foreach (string previousId in step.Previous)
                    Link(stepGraph, stepNodes[previousId], stepNodes[step.Id]);
            }

            EditorUtility.SetDirty(stepGraph);
        }

        static void Link(BaseGraph graph, BaseNode from, BaseNode to)
        {
            graph.Connect(to.GetPort(PreviousPort, null), from.GetPort(NextPort, null), false);
        }

        // Removing a StrategyNode also deletes its nested StepGraph sub-asset (see GameFlowGraph).
        static void Clear(GameFlowGraph graph)
        {
            foreach (SerializableEdge edge in graph.edges.ToList())
                graph.Disconnect(edge);

            foreach (BaseNode node in graph.nodes.ToList())
                graph.RemoveNode(node);
        }
    }
}
