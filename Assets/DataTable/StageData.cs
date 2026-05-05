using UnityEngine;

[CreateAssetMenu(fileName = "Stage_", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public int StageLevel;
    public int MaxSlotCount;
    public int ClearScore;
    public float TimeLimit;
}
