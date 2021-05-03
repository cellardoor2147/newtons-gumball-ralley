using UnityEngine;

namespace Core.PlacedObjects
{
    [CreateAssetMenu(fileName = "PlacedObjectMetaData", menuName = "ScriptableObjects/PlacedObjectMetaData", order = 4)]
    public class PlacedObjectMetaData : ScriptableObject
    {
        public string objectName;
        public GameObject prefab;
        public bool canSnap;
        public bool isSimpleMachine;
        public float amountOfScrap;
    }
}
