using UnityEngine;

namespace ICOPE.Immersive.Values
{
    [CreateAssetMenu(fileName = "SkillBoard", menuName = "ICOPE/Immersive/Skill Board")]
    public class SkillBoardSO : ScriptableObject
    {
        [field: SerializeField] public SkillBoardData Data { get; private set; } = new SkillBoardData();
    }
}
