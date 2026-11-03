using System.Collections.Generic;

namespace ICOPE.Immersive.Badges
{
    /// <summary>
    /// Levels rewarded by a badge, in badge slot order (ImmersiveAPI allows 3). A key is the level
    /// prefix of the folders in StreamingAssets/Levels, e.g. "1" for 1_Facile and 1_3_Denise.
    /// </summary>
    public static class LevelBadgeCatalog
    {
        public static IReadOnlyList<string> LevelKeys { get; } = new[] { "1", "2", "3" };
    }
}
