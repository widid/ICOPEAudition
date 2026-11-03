using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "CaseResult", menuName = "ICOPE/Immersive/Case Result")]
    public class CaseResultSO : ScriptableObject
    {
        [field: SerializeField] public CaseResultData Data { get; private set; } = new CaseResultData();
    }
}
