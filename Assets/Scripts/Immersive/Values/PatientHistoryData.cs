using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Per-patient totals over every attempt, indexed like the Training's strategies. Restored from the server at startup.</summary>
    [Serializable]
    public class PatientHistoryData
    {
        public int[] secondsSpent = new int[0];
        public int[] timesHandled = new int[0];

        public void Resize(int count)
        {
            if (secondsSpent.Length != count)
                Array.Resize(ref secondsSpent, count);

            if (timesHandled.Length != count)
                Array.Resize(ref timesHandled, count);
        }
    }
}
