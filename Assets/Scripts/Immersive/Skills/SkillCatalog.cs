using System.Collections.Generic;

namespace ICOPE.Immersive.Skills
{
    /// <summary>Skills reported by the Training. Step types are the PatientData.AlgoData.Step names.</summary>
    public static class SkillCatalog
    {
        public static IReadOnlyList<SkillDefinition> All { get; } = new[]
        {
            new SkillDefinition("screening", "Repérage ICOPE", "ICOPE screening", "CasePresentation", "GoNoGo", "GoNoGoOtoscopy", "Telephone"),
            new SkillDefinition("whisper", "Test de la voix chuchotée", "Whisper test", "WisperTest"),
            new SkillDefinition("otoscopy", "Otoscopie", "Otoscopy", "Otoscopy", "GoNoGoOtoscopy"),
            new SkillDefinition("hhies", "Questionnaire HHIE-S", "HHIE-S questionnaire", "Hhies", "HhiesWeber"),
            new SkillDefinition("acoumetry", "Acoumétrie (Weber)", "Tuning fork test (Weber)", "WeberTest", "HhiesWeber"),
            new SkillDefinition("audiometry", "Audiométrie", "Audiometry", "Audiometry", "AudiometryVocale"),
        };
    }
}
