using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Core;

namespace GUI.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private readonly static string LEFT_SPEAKER_KEY = "Left Speaker";
        private readonly static string RIGHT_SPEAKER_KEY = "Right Speaker";
        private readonly static string DIALOGUE_BOX_KEY = "Dialogue Box";
        private readonly static string SPEAKER_NAME_KEY = "Speaker Name";
        private readonly static string SPEAKER_NAME_TEXT_KEY = "Speaker Name Text";
        private readonly static string DIALOGUE_BOX_TEXT_KEY = "Dialogue Box Content";

        private Image leftSpeakerImage;
        private Image rightSpeakerImage;
        private TextMeshProUGUI speakerName;
        private TextMeshProUGUI dialogueBoxContent;
        private bool anyKeyWasPressed;

        private void Awake()
        {
            leftSpeakerImage = transform.Find(LEFT_SPEAKER_KEY).GetComponent<Image>();
            rightSpeakerImage = transform.Find(RIGHT_SPEAKER_KEY).GetComponent<Image>();
            speakerName = transform.Find(DIALOGUE_BOX_KEY)
                .transform.Find(SPEAKER_NAME_KEY)
                .transform.Find(SPEAKER_NAME_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            dialogueBoxContent = transform.Find(DIALOGUE_BOX_KEY)
                .transform.Find(DIALOGUE_BOX_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
        }

        public void StartConversation(Conversation conversation)
        {
            StartCoroutine(TypeEachDialogueBoxContent(conversation));
        }

        private IEnumerator TypeEachDialogueBoxContent(Conversation conversation)
        {
            foreach (Line line in conversation.lines)
            {
                SetSpeakerImages(conversation, line);
                SetSpeakerName(conversation, line);
                yield return TypeDialogueBoxContent(line);
                yield return new WaitUntil(() => Input.anyKeyDown);
            }
            GameStateManager.SetGameState(GameState.Playing);
        }

        private void SetSpeakerImages(Conversation conversation, Line line)
        {
            bool shouldSetLeftSpeakerImage = conversation.leftSpeaker != null;
            if (shouldSetLeftSpeakerImage)
            {
                leftSpeakerImage.sprite = GetSpeakerSpriteByExpression(
                    conversation.leftSpeaker,
                    line.leftSpeakerExpression
                );
                leftSpeakerImage.color = GetSpeakerColorByActiveSpeakerDirection(
                    SpeakerDirection.Left,
                    line.activeSpeakerDirection
                );
            }
            bool shouldSetRightSpeakerImage = conversation.rightSpeaker != null;
            if (shouldSetRightSpeakerImage)
            {
                rightSpeakerImage.sprite = GetSpeakerSpriteByExpression(
                    conversation.rightSpeaker,
                    line.rightSpeakerExpression
                );
                rightSpeakerImage.color = GetSpeakerColorByActiveSpeakerDirection(
                    SpeakerDirection.Right,
                    line.activeSpeakerDirection
                );
            }
        }

        private Sprite GetSpeakerSpriteByExpression(CharacterMetaData characterMetaData, Expression expression)
        {
            switch (expression)
            {
                case Expression.Neutral:
                    return characterMetaData.neutralPortrait;
                case Expression.Happy:
                    return characterMetaData.happyPortrait;
                case Expression.Worried:
                    return characterMetaData.worriedPortrait;
                case Expression.Skeptical:
                    return characterMetaData.skepticalPortrait;
                case Expression.Surprised:
                    return characterMetaData.surprisedPortrait;
                case Expression.Teaching:
                    return characterMetaData.teachingPortrait;
                default:
                    Debug.LogError($"Tried setting invalid character expression: {expression}");
                    return null;
            }
        }

        private Color GetSpeakerColorByActiveSpeakerDirection(
            SpeakerDirection speakerToColorDirection,
            SpeakerDirection activeSpeakerDirection
        )
        {
            if (speakerToColorDirection.Equals(SpeakerDirection.Left))
            {
                return new Color(
                    leftSpeakerImage.color.r,
                    leftSpeakerImage.color.g,
                    leftSpeakerImage.color.b,
                    activeSpeakerDirection.Equals(SpeakerDirection.Left)
                        ? 1.0f : 0.5f
                );
            }
            return new Color(
                rightSpeakerImage.color.r,
                rightSpeakerImage.color.g,
                rightSpeakerImage.color.b,
                activeSpeakerDirection.Equals(SpeakerDirection.Right)
                    ? 1.0f : 0.5f
            );
        }

        private void SetSpeakerName(Conversation conversation, Line line)
        {
            bool shouldWriteLeftSpeakerName =
                line.activeSpeakerDirection.Equals(SpeakerDirection.Left) &&
                conversation.leftSpeaker != null;
            if (shouldWriteLeftSpeakerName)
            {
                speakerName.text = conversation.leftSpeaker.fullName;
            }
            bool shouldWriteRightSpeakerName =
                line.activeSpeakerDirection.Equals(SpeakerDirection.Right) &&
                conversation.rightSpeaker != null;
            if (shouldWriteRightSpeakerName)
            {
                speakerName.text = conversation.rightSpeaker.fullName;
            }
        }

        private IEnumerator TypeDialogueBoxContent(Line line)
        {
            dialogueBoxContent.text = "";
            foreach (char character in line.content)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                dialogueBoxContent.text += character;
            }
            dialogueBoxContent.text = line.content;
        }
    }
}
