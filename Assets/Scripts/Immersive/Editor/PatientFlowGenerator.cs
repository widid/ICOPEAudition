using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICOPE.Immersive.Editor.Authoring;
using ICOPE.Immersive.Editor.Catalog;
using ICOPE.Immersive.Identity;
using UnityEditor;
using UnityEngine;
using WiDiD.ImmersiveAPI.Trainings;
using WiDiD.ImmersiveAPI.Trainings.GameFlow.Building;
using WiDiD.ImmersiveAPI.Trainings.GameFlow.Json;

namespace ICOPE.Immersive.Editor
{
    /// <summary>
    /// Regenerates the ICOPE_Audition Training and its Editor graph from StreamingAssets/Levels,
    /// and exports its gameflow JSON for the backend. Run after any change to the patient files.
    /// </summary>
    public static class PatientFlowGenerator
    {
        const string ExportPath = "Docs/ImmersiveAPI/ICOPE_Audition.gameflow.json";

        [MenuItem("ICOPE/ImmersiveAPI/Generate Training From Levels")]
        static void Generate()
        {
            TrainingSO training = FindTraining();
            if (training == null)
            {
                Debug.LogError($"[PatientFlowGenerator] No TrainingSO with Id '{FlowIds.TrainingId}' found.");
                return;
            }

            List<CaseDefinition> cases = LevelCatalogReader.Read();
            GameFlowBuildResult result = GameFlowStructureBuilder.Build(cases);

            if (!result)
            {
                Debug.LogError($"[PatientFlowGenerator] GameFlow build failed:\n{result}");
                return;
            }

            foreach (GameFlowBuildIssue warning in result.Warnings)
                Debug.LogWarning($"[PatientFlowGenerator] {warning}");

            GameFlowGraphWriter.Write(training, result.GameFlow);
            training.Data.GameFlow.SimulationPassingScore = result.GameFlow.SimulationPassingScore;

            StatementableAuthoring.Apply(training.Data.GameFlow, cases, TrackingAssetFactory.EnsureAll(cases));

            EditorUtility.SetDirty(training);
            AssetDatabase.SaveAssets();
            Export(training);

            int stepCount = cases.Sum(patient => patient.Steps.Count);
            Debug.Log($"[PatientFlowGenerator] '{training.name}' generated: {cases.Count} strategies, {stepCount} steps. GameFlow JSON written to {ExportPath}.");
        }

        static TrainingSO FindTraining() => AssetDatabase.FindAssets($"t:{nameof(TrainingSO)}")
            .Select(guid => AssetDatabase.LoadAssetAtPath<TrainingSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .FirstOrDefault(training => training != null && training.Data.Id == FlowIds.TrainingId);

        static void Export(TrainingSO training)
        {
            string fullPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ExportPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, training.Data.GameFlow.ToJson());
        }
    }
}
