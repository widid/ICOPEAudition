using System;
using System.Collections.Generic;

namespace ICOPE.Immersive.Editor.Catalog
{
    /// <summary>The parts of a patient JSON the GameFlow is generated from.</summary>
    [Serializable]
    public class PatientFileDto
    {
        public string firstName;
        public string lastName;
        public List<PatientStepDto> steps;
    }
}
