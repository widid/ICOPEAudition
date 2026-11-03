using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ICOPE.Immersive.Editor.Catalog
{
    /// <summary>Reads the patients from StreamingAssets/Levels the same way PatientDataJsonLoader does.</summary>
    public static class LevelCatalogReader
    {
        const string LevelsFolder = "Levels";
        const string ManifestFile = "manifest.json";

        public static List<CaseDefinition> Read()
        {
            string levelsRoot = Path.Combine(Application.streamingAssetsPath, LevelsFolder);
            LevelManifestDto manifest = JsonUtility.FromJson<LevelManifestDto>(ReadJson(Path.Combine(levelsRoot, ManifestFile)));
            List<CaseDefinition> cases = new List<CaseDefinition>();

            foreach (LevelEntryDto level in manifest.levels)
            {
                foreach (string patientFolder in level.patients)
                {
                    string patientPath = Path.Combine(levelsRoot, level.name, patientFolder, ExtractName(patientFolder) + ".json");

                    if (!File.Exists(patientPath))
                    {
                        Debug.LogError($"[LevelCatalogReader] Missing patient file {patientPath}.");
                        continue;
                    }

                    PatientFileDto patient = JsonUtility.FromJson<PatientFileDto>(ReadJson(patientPath));
                    cases.Add(new CaseDefinition(patientFolder, ExtractName(level.name), patient.firstName, ReadSteps(patient)));
                }
            }

            return cases;
        }

        static List<CaseStepDefinition> ReadSteps(PatientFileDto patient)
        {
            List<CaseStepDefinition> steps = new List<CaseStepDefinition>();

            for (int i = 0; i < patient.steps.Count; i++)
            {
                PatientStepDto step = patient.steps[i];
                steps.Add(new CaseStepDefinition(i, step.type, step.diagnosticPhase is { Count: > 0 }, step.actionPhase is { Count: > 0 }));
            }

            return steps;
        }

        static string ReadJson(string path) => File.ReadAllText(path).TrimStart('﻿');

        static string ExtractName(string raw)
        {
            int index = raw.LastIndexOf('_');
            return index >= 0 ? raw.Substring(index + 1) : raw;
        }
    }
}
