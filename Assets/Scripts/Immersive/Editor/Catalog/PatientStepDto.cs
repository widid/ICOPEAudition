using System;
using System.Collections.Generic;

namespace ICOPE.Immersive.Editor.Catalog
{
    [Serializable]
    public class PatientStepDto
    {
        public string type;
        public List<PatientAnswerDto> diagnosticPhase;
        public List<PatientAnswerDto> actionPhase;
    }
}
