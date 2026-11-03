using System.Collections.Generic;

namespace ICOPE.Immersive.Editor.Catalog
{
    /// <summary>One patient as declared in StreamingAssets/Levels.</summary>
    public class CaseDefinition
    {
        /// <summary>Patient folder name, e.g. 1_3_Denise.</summary>
        public string CaseId { get; }
        public string LevelName { get; }
        public string FirstName { get; }
        public IReadOnlyList<CaseStepDefinition> Steps { get; }

        public CaseDefinition(string caseId, string levelName, string firstName, IReadOnlyList<CaseStepDefinition> steps)
        {
            CaseId = caseId;
            LevelName = levelName;
            FirstName = firstName;
            Steps = steps;
        }
    }
}
