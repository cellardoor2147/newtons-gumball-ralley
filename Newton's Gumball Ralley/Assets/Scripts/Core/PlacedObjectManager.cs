using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class PlacedObjectManager : MonoBehaviour
    {
        [SerializeField] public PlacedObjectMetaData metaData;
        private Rigidbody2D placedObjectRB;
        private Vector2 lastValidPosition;
        private Quaternion lastValidRotation;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;

        private void Awake()
        {
            if (!metaData.objectName.Contains("Pulley")
                && !metaData.objectName.Equals("LeverWeightContraption"))
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                defaultColor = spriteRenderer.color;
            }
            placedObjectRB = GetComponent<Rigidbody2D>();
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
        }

        public void UnfreezeRigidbody()
        {
            bool canNotUnfreezeRigidBody = placedObjectRB == null;
            if (canNotUnfreezeRigidBody)
            {
                return;
            }
            placedObjectRB.constraints = RigidbodyConstraints2D.None;
        }

        public void FreezeRigidbody()
        {
            bool canNotFreezeRigidBody = placedObjectRB == null;
            if (canNotFreezeRigidBody)
            {
                return;
            }
            placedObjectRB.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void ResetTransform()
        {
            transform.position = lastValidPosition;
            transform.rotation = lastValidRotation;
        }

        public void SetLastValidPosition(Vector2 position)
        {
            lastValidPosition = position;
        }

        public void SetLastValidRotation(Quaternion rotation)
        {
            lastValidRotation = rotation;
        }
        public void GrayOut()
        {
            if (gameObject.activeSelf && !metaData.objectName.Contains("Pulley")
                                      && !metaData.objectName.Contains("SpikeWedge")) {
                spriteRenderer.color = Color.gray;
            }    
        }

        public void RevertFromGray()
        {
            if (gameObject.activeSelf && !metaData.objectName.Contains("Pulley") 
                                      && !metaData.objectName.Contains("SpikeWedge")) {
                spriteRenderer.color = defaultColor;
            }
        }
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
