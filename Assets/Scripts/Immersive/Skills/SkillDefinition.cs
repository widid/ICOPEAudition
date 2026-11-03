using System;
using System.Collections.Generic;
using System.Linq;
using ICOPE.Immersive.Identity;

namespace ICOPE.Immersive.Skills
{
    /// <summary>A clinical competence and the step types that exercise it.</summary>
    public class SkillDefinition
    {
        public string Key { get; }
        public string NameFr { get; }
        public string NameEn { get; }
        public string Id => FlowIds.Skill(Key);

        readonly HashSet<string> stepTypes;

        public SkillDefinition(string key, string nameFr, string nameEn, params string[] stepTypes)
        {
            Key = key;
            NameFr = nameFr;
            NameEn = nameEn;
            this.stepTypes = new HashSet<string>(stepTypes, StringComparer.OrdinalIgnoreCase);
        }

        public bool Covers(string stepType) => stepType != null && stepTypes.Contains(stepType);

        public bool CoversAny(IEnumerable<string> types) => types.Any(Covers);
    }
}
