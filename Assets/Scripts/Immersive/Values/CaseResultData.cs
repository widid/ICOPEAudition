using System;

namespace ICOPE.Immersive.Values
{
    /// <summary>Results of the last finished case, read by every case Performance.</summary>
    [Serializable]
    public class CaseResultData
    {
        public int diagnosticErrors;
        public int actionErrors;
        public int stepsFirstTry;
        public int stepCount;
        public int successRatePercent;
        public float successRate;
        public int durationSeconds;

        public void CopyFrom(CaseResultData other)
        {
            diagnosticErrors = other.diagnosticErrors;
            actionErrors = other.actionErrors;
            stepsFirstTry = other.stepsFirstTry;
            stepCount = other.stepCount;
            successRatePercent = other.successRatePercent;
            successRate = other.successRate;
            durationSeconds = other.durationSeconds;
        }
    }
}
