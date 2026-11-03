using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "ProgressBoard", menuName = "ICOPE/Immersive/Progress Board")]
    public class ProgressBoardSO : ScriptableObject
    {
        [field: SerializeField] public ProgressBoardData Data { get; private set; } = new ProgressBoardData();
    }
}
