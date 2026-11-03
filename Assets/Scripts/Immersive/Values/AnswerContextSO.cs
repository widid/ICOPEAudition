using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "AnswerContext", menuName = "ICOPE/Immersive/Answer Context")]
    public class AnswerContextSO : ScriptableObject
    {
        [field: SerializeField] public AnswerContextData Data { get; private set; } = new AnswerContextData();
    }
}
