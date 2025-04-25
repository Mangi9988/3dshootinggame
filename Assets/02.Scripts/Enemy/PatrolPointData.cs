using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPointData", menuName = "Scriptable Objects/PatrolPointData")]
public class PatrolPointData : ScriptableObject
{
    public Vector3[] PatrolPositions;
}
