using UnityEngine;

[CreateAssetMenu(fileName = "MachineType", menuName = "ScriptableObjects/MachineType")]
public class MachineTypeSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
}
