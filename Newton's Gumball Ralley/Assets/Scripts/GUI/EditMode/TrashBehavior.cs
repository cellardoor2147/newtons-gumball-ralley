using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Audio;

namespace GUI.EditMode
{
    public class TrashBehavior :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private Sprite trashClosedSprite;
        [SerializeField] private Sprite trashOpenSprite;
        [SerializeField] private SoundMetaData trashOpenSound;
        [SerializeField] private SoundMetaData trashCloseSound;

        private RectTransform rectTransform;
        private float originalRectTransformHeight;
        private Image trashImage;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalRectTransformHeight = rectTransform.sizeDelta.y;
            trashImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            SetSprite(trashClosedSprite);
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            if (!EditModeManager.SomeMachineIsSelected())
            {
                return;
            }
            AudioManager.instance.PlaySound(trashOpenSound.name);
            SetSprite(trashOpenSprite);
            EditModeManager.SetSelectedMachineVisibility(false);
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            AudioManager.instance.PlaySound(trashCloseSound.name);
            SetSprite(trashClosedSprite);
            EditModeManager.SetSelectedMachineVisibility(true);
        }

        private void SetSprite(Sprite sprite)
        {
            float spriteSizeRatio = sprite.rect.height / sprite.rect.width;
            float adjustedSpriteHeight = spriteSizeRatio * originalRectTransformHeight;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, adjustedSpriteHeight);
            trashImage.sprite = sprite;
        }
    }
}
