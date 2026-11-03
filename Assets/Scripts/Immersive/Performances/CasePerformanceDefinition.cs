namespace ICOPE.Immersive.Performances
{
    /// <summary>One Strategy-scoped Performance reported at the end of every case.</summary>
    public class CasePerformanceDefinition
    {
        public string Key { get; }
        public string NameFr { get; }
        public string NameEn { get; }
        public string Unit { get; }

        /// <summary>CaseResultData int field sent as Raw.</summary>
        public string RawField { get; }

        /// <summary>CaseResultData float field sent as Scaled, or null.</summary>
        public string ScaledField { get; }

        /// <summary>Fixed Max, or null. Ignored when MaxIsStepCount.</summary>
        public int? FixedMax { get; }

        public bool MaxIsStepCount { get; }

        public CasePerformanceDefinition(string key, string nameFr, string nameEn, string unit, string rawField, string scaledField = null, int? fixedMax = null, bool maxIsStepCount = false)
        {
            Key = key;
            NameFr = nameFr;
            NameEn = nameEn;
            Unit = unit;
            RawField = rawField;
            ScaledField = scaledField;
            FixedMax = fixedMax;
            MaxIsStepCount = maxIsStepCount;
        }
    }
}
