using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "PlacedObjectMetaData", menuName = "ScriptableObjects/PlacedObjectMetaData", order = 4)]
    public class PlacedObjectMetaData : ScriptableObject
    {
        public string objectName;
        public GameObject prefab;
        public bool canSnap;
    }
}
