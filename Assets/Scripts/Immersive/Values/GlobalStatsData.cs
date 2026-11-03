using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Values read by the Simulation-scoped global Performances.</summary>
    [Serializable]
    public class GlobalStatsData
    {
        public int patientsHandled;
        public int reminderOpenings;
        public int successRatePercent;
        public float successRate;
    }
}
