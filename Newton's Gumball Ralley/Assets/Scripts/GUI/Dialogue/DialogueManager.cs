using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Core;
using Audio;

namespace GUI.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private readonly static string LEFT_SPEAKER_KEY = "Left Speaker";
        private readonly static string RIGHT_SPEAKER_KEY = "Right Speaker";
        private readonly static string DIALOGUE_BOX_KEY = "Dialogue Box";
        private readonly static string LEFT_SPEAKER_NAME_KEY = "Left Speaker Name";
        private readonly static string RIGHT_SPEAKER_NAME_KEY = "Right Speaker Name";
        private readonly static string SPEAKER_NAME_TEXT_KEY = "Speaker Name Text";
        private readonly static string DIALOGUE_BOX_TEXT_KEY = "Dialogue Box Content";
        private readonly static string TUTORIAL_BOX_KEY = "Tutorial Box";
        private readonly static string TUTORIAL_BOX_IMAGE_KEY = "Tutorial Box Image";
        private readonly static string TUTORIAL_BOX_TEXT_KEY = "Tutorial Box Conetent";

        [SerializeField] private List<Conversation> conversations;

        private Transform leftSpeakerTransform;
        private Transform rightSpeakerTransform;
        private Image leftSpeakerImage;
        private Image rightSpeakerImage;
        private GameObject leftSpeakerName;
        private GameObject rightSpeakerName;
        private TextMeshProUGUI leftSpeakerNameText;
        private TextMeshProUGUI rightSpeakerNameText;
        private TextMeshProUGUI dialogueBoxContent;
        private GameObject tutorialBox;
        private Image tutorialBoxImage;
        private TextMeshProUGUI tutorialBoxText;

        private void Awake()
        {
            leftSpeakerTransform = transform.Find(LEFT_SPEAKER_KEY);
            rightSpeakerTransform = transform.Find(RIGHT_SPEAKER_KEY);
            leftSpeakerImage = leftSpeakerTransform.GetComponent<Image>();
            rightSpeakerImage = rightSpeakerTransform.GetComponent<Image>();
            leftSpeakerName = transform.Find(DIALOGUE_BOX_KEY)
                .transform.Find(LEFT_SPEAKER_NAME_KEY).gameObject;
            rightSpeakerName = transform.Find(DIALOGUE_BOX_KEY)
                .transform.Find(RIGHT_SPEAKER_NAME_KEY).gameObject;
            leftSpeakerNameText = leftSpeakerName.transform
                .Find(SPEAKER_NAME_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            rightSpeakerNameText = rightSpeakerName.transform
                .Find(SPEAKER_NAME_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            dialogueBoxContent = transform.Find(DIALOGUE_BOX_KEY)
                .transform.Find(DIALOGUE_BOX_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            /*
             * TODO: implement tutorial box properly
            tutorialBox = transform.Find(TUTORIAL_BOX_KEY).gameObject;
            tutorialBoxImage = tutorialBox.transform.Find(TUTORIAL_BOX_TEXT_KEY)
                .GetComponent<Image>();
            tutorialBoxText = tutorialBox.transform.Find(TUTORIAL_BOX_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            */
        }

        private void OnEnable()
        {
            bool shouldPlayBeginningDialogue = !LevelManager.GetCurrentLevelIsComplete();
            foreach (Conversation conversation in conversations)
            {
                bool conversationShouldPlay =
                    conversation.worldIndex == LevelManager.GetCurrentWorldIndex()
                    && conversation.levelIndex == LevelManager.GetCurrentLevelIndex();
                if (conversationShouldPlay)
                {
                    StartConversation(conversation, shouldPlayBeginningDialogue);
                    return;
                }
            }
            GameStateManager.SetGameState(
                shouldPlayBeginningDialogue
                ? GameState.Editing
                : GameState.LevelCompleted
            ); // Couldn't find a conversation to play
        }

        public void StartConversation(Conversation conversation, bool shouldPlayBeginningDialogue)
        {
            StartCoroutine(TypeEachDialogueBoxContent(conversation, shouldPlayBeginningDialogue));
        }

        private IEnumerator TypeEachDialogueBoxContent(Conversation conversation, bool shouldPlayBeginningDialogue)
        {
            Line[] lines = shouldPlayBeginningDialogue
                ? conversation.linesAtBeginningOfLevel
                : conversation.linesAtEndOfLevel;
            foreach (Line line in lines)
            {
                SetSpeakerImage(SpeakerDirection.Left, conversation.leftSpeaker, line);
                SetSpeakerImage(SpeakerDirection.Right, conversation.rightSpeaker, line);
                bool leftSpeakerIsActive =
                    line.activeSpeakerDirection.Equals(SpeakerDirection.Left);
                if (leftSpeakerIsActive)
                {
                    SetActiveSpeakerName(
                        SpeakerDirection.Left,
                        conversation.leftSpeaker.fullName
                    );
                    /* TODO: fix sound for dialogue system
                    AudioManager.instance.PlaySound(
                        GetSpeakerSoundNameByExpression(conversation.leftSpeaker, line.leftSpeakerExpression)
                    );
                    */
                }
                else
                {
                    SetActiveSpeakerName(
                        SpeakerDirection.Right,
                        conversation.rightSpeaker.fullName
                    );
                    /* TODO: fix sound for dialogue system
                    AudioManager.instance.PlaySound(
                        GetSpeakerSoundNameByExpression(conversation.rightSpeaker, line.rightSpeakerExpression)
                    );
                    */
                }
                yield return TypeDialogueBoxContent(line);
                yield return new WaitUntil(() => Input.anyKeyDown);
            }
            GameStateManager.SetGameState(shouldPlayBeginningDialogue
                ? GameState.Editing
                : GameState.LevelCompleted
            );
        }

        private void SetSpeakerImage(SpeakerDirection direction, CharacterMetaData character, Line line)
        {
            Image characterImage;
            Expression characterExpression;
            if (direction.Equals(SpeakerDirection.Left))
            {
                characterImage = leftSpeakerImage;
                characterExpression = line.leftSpeakerExpression;
            }
            else
            {
                characterImage = rightSpeakerImage;
                characterExpression = line.rightSpeakerExpression;
            }
            characterImage.sprite = GetSpeakerSpriteByExpression(
                character,
                characterExpression
            );
            characterImage.color = new Color(
                characterImage.color.r,
                characterImage.color.g,
                characterImage.color.b,
                direction.Equals(line.activeSpeakerDirection) ? 1.0f : 0.5f
            );
        }

        private Sprite GetSpeakerSpriteByExpression(CharacterMetaData characterMetaData, Expression expression)
        {
            switch (expression)
            {
                case Expression.Neutral:
                    return characterMetaData.neutralPortrait;
                case Expression.Happy:
                    return characterMetaData.happyPortrait;
                case Expression.Oops:
                    return characterMetaData.oopsPortrait;
                case Expression.Thinking:
                    return characterMetaData.thinkingPortrait;
                case Expression.Explaining:
                    return characterMetaData.explainingPortrait;
                default:
                    Debug.LogError($"Tried setting invalid character expression: {expression}");
                    return null;
            }
        }

        private void SetActiveSpeakerName(SpeakerDirection direction, string name)
        {
            if (direction.Equals(SpeakerDirection.Left))
            {
                leftSpeakerName.SetActive(true);
                rightSpeakerName.SetActive(false);
                leftSpeakerNameText.text = name;
            }
            else
            {
                leftSpeakerName.SetActive(false);
                rightSpeakerName.SetActive(true);
                rightSpeakerNameText.text = name;
            }
        }

        private string GetSpeakerSoundNameByExpression(
            CharacterMetaData activeSpeaker,
            Expression expression
        )
        {
            switch (expression)
            {
                case Expression.Neutral:
                    return activeSpeaker.neutralSound.name;
                case Expression.Happy:
                    return activeSpeaker.happySound.name;
                case Expression.Oops:
                    return activeSpeaker.oopsSound.name;
                case Expression.Thinking:
                    return activeSpeaker.thinkingSound.name;
                case Expression.Explaining:
                    return activeSpeaker.explainingSound.name;
                default:
                    Debug.LogError($"Tried playing sound for invalid character expression: {expression}");
                    return null;
            }

        }
             
        private IEnumerator TypeDialogueBoxContent(Line line)
        {
            dialogueBoxContent.text = "";
            foreach (char character in line.content)
            {
                yield return new WaitForSecondsRealtime(line.secondDelayBetweenTypingEachChar);
                dialogueBoxContent.text += character;
            }
            dialogueBoxContent.text = line.content;
        }
    }
}
