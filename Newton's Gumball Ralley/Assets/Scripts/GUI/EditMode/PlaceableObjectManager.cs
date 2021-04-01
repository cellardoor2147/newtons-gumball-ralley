using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GUI.EditMode
{
    public class PlaceableObjectManager : MonoBehaviour, IPointerClickHandler
    {
        private static readonly string PLACEABLE_OBJECT_IMAGE_PREFIX =
            "Placeable Object Image";

        [SerializeField] GameObject placeableObjectPrefab;

        private void Awake()
        {
            transform.Find(PLACEABLE_OBJECT_IMAGE_PREFIX).GetComponent<Image>().sprite =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite;
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            Debug.Log("GOT HERE");
            Instantiate(placeableObjectPrefab);
        }
    }
}
