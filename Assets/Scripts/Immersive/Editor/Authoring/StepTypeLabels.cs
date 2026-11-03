using System;
using System.Collections.Generic;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>Display names of the PatientData.AlgoData.Step types.</summary>
    public static class StepTypeLabels
    {
        static readonly Dictionary<string, (string Fr, string En)> labels = new(StringComparer.OrdinalIgnoreCase)
        {
            { "CasePresentation", ("Présentation du cas", "Case presentation") },
            { "WisperTest", ("Test de la voix chuchotée", "Whisper test") },
            { "GoNoGo", ("Questionnaire de repérage", "Screening questionnaire") },
            { "Otoscopy", ("Otoscopie", "Otoscopy") },
            { "Hhies", ("Questionnaire HHIE-S", "HHIE-S questionnaire") },
            { "WeberTest", ("Test de Weber", "Weber test") },
            { "Audiometry", ("Audiométrie", "Audiometry") },
            { "Telephone", ("Appel téléphonique", "Phone call") },
            { "HhiesWeber", ("HHIE-S et Weber", "HHIE-S and Weber") },
            { "AudiometryVocale", ("Audiométrie vocale", "Speech audiometry") },
            { "GoNoGoOtoscopy", ("Repérage et otoscopie", "Screening and otoscopy") },
        };

        public static string Fr(string stepType) => labels.TryGetValue(stepType, out var label) ? label.Fr : stepType;
        public static string En(string stepType) => labels.TryGetValue(stepType, out var label) ? label.En : stepType;
    }
}
