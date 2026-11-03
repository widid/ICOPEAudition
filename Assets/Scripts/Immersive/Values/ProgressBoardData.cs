using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Whether each badge level is fully completed, indexed like LevelBadgeCatalog.LevelKeys. Read by the badge conditions.</summary>
    [Serializable]
    public class ProgressBoardData
    {
        public bool[] levelsCompleted = new bool[0];

        public void Resize(int count)
        {
            if (levelsCompleted.Length != count)
                Array.Resize(ref levelsCompleted, count);
        }
    }
}
