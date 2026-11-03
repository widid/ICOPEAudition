using System;
using System.Collections.Generic;

namespace ICOPE.Immersive.Editor.Authoring
{
    /// <summary>English names of the StreamingAssets/Levels folders, which are named in French.</summary>
    public static class LevelNameLabels
    {
        static readonly Dictionary<string, string> english = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Tutoriel", "Tutorial" },
            { "Facile", "Easy" },
            { "Moyen", "Medium" },
            { "Difficile", "Hard" },
        };

        public static string Fr(string levelName) => levelName;
        public static string En(string levelName) => english.TryGetValue(levelName, out string label) ? label : levelName;
    }
}
