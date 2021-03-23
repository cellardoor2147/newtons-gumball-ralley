using UnityEngine;

namespace GUI
{
    public class DialogueManager : MonoBehaviour
    {
        private readonly static string LEFT_SPEAKER_KEY = "Left Speaker";
        private readonly static string RIGHT_SPEAKER_KEY = "Right Speaker";
        private readonly static string DIALOGUE_BOX_KEY = "Dialogue Box";

        private Sprite leftSpeakerSprite;
        private Sprite rightSpeakerSprite;
        private GameObject dialogueBox;

        private void Awake()
        {
            //leftSpeakerSprite = GameObject.Find(LEFT_SPEAKER_KEY).GetComponent<Sprite>();
        }
    }
}
