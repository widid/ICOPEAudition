using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Scaled score of each skill, indexed like SkillCatalog.All.</summary>
    [Serializable]
    public class SkillBoardData
    {
        public float[] scores = new float[0];

        public void Resize(int count)
        {
            if (scores.Length != count)
                Array.Resize(ref scores, count);
        }

        public void Set(int index, float score)
        {
            if (index >= scores.Length)
                Resize(index + 1);

            scores[index] = score;
        }
    }
}
