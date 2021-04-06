using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class PlacedObjectManager : MonoBehaviour
    {
        [SerializeField] public PlacedObjectMetaData metaData;
    }

    public static class PlacedObjectPrefabDictionary
    {
        private readonly static Dictionary<string, GameObject> dictionary;
        private readonly static string RESOURCES_PATH = "PlacedObjectMetaData";

        static PlacedObjectPrefabDictionary()
        {
            dictionary = new Dictionary<string, GameObject>();
            foreach (
                PlacedObjectMetaData metaData in 
                Resources.LoadAll<PlacedObjectMetaData>(RESOURCES_PATH)
            )
            {
                if (dictionary.ContainsKey(metaData.objectName))
                {
                    continue;
                }
                dictionary.Add(metaData.objectName, metaData.prefab);
            }
        }

        public static GameObject Get(string key)
        {
            return dictionary[key];
        }
    }
}
