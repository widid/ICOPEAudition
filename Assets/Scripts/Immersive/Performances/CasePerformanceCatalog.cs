using System.Collections.Generic;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Performances
{
    public static class CasePerformanceCatalog
    {
        public static IReadOnlyList<CasePerformanceDefinition> All { get; } = new[]
        {
            new CasePerformanceDefinition("diagnostic_errors", "D'analyse", "In the analysis", "erreurs", nameof(CaseResultData.diagnosticErrors)),
            new CasePerformanceDefinition("action_errors", "Sur la conduite à tenir", "In the course of action", "erreurs", nameof(CaseResultData.actionErrors)),
            new CasePerformanceDefinition("steps_first_try", "Réussies du premier coup", "Right first time", "étapes", nameof(CaseResultData.stepsFirstTry), maxIsStepCount: true),
            new CasePerformanceDefinition("success_rate", "De réussite sur ce cas", "Success rate on this case", "%", nameof(CaseResultData.successRatePercent), nameof(CaseResultData.successRate), fixedMax: 100),
            new CasePerformanceDefinition("duration", "Pour terminer ce cas", "To complete this case", "secondes", nameof(CaseResultData.durationSeconds)),
        };
    }
}
