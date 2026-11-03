using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "PatientHistory", menuName = "ICOPE/Immersive/Patient History")]
    public class PatientHistorySO : ScriptableObject
    {
        [field: SerializeField] public PatientHistoryData Data { get; private set; } = new PatientHistoryData();
    }
}
