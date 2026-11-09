using System.Collections.Generic;
using ICOPE.Immersive.Values;

namespace ICOPE.Immersive.Performances
{
    public static class CasePerformanceCatalog
    {
        public static IReadOnlyList<CasePerformanceDefinition> All { get; } = new[]
        {
            new CasePerformanceDefinition("diagnostic_errors", "Erreurs d'analyse", "Analysis errors", "", nameof(CaseResultData.diagnosticErrors)),
            new CasePerformanceDefinition("action_errors", "Erreurs sur la conduite à tenir", "Errors in the course of action", "", nameof(CaseResultData.actionErrors)),
            new CasePerformanceDefinition("steps_first_try", "Étapes réussies du premier coup", "Steps right first time", "", nameof(CaseResultData.stepsFirstTry), maxIsStepCount: true),
            new CasePerformanceDefinition("success_rate", "De réussite sur ce cas", "Success rate on this case", "%", nameof(CaseResultData.successRatePercent), nameof(CaseResultData.successRate), fixedMax: 100),
            new CasePerformanceDefinition("duration", "Pour terminer ce cas", "To complete this case", "min", nameof(CaseResultData.durationMinutes)),
        };
    }
}
