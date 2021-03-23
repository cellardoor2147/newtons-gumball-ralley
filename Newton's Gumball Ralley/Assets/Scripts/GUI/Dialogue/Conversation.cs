using UnityEngine;

namespace GUI.Dialogue
{
    public enum Expression
    {
        Neutral = 0,
        Happy = 1,
        Worried = 2,
        Skeptical = 3,
        Surprised = 4,
        Teaching = 5
    }

    public enum SpeakerDirection
    {
        Left = 0,
        Right = 1
    }

    [System.Serializable]
    public struct Line
    {
        public float secondDelayBetweenTypingEachChar;
        public SpeakerDirection activeSpeakerDirection;
        public Expression leftSpeakerExpression;
        public Expression rightSpeakerExpression;
        [TextArea(2, 5)]
        public string content;
    }

    [CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Conversation", order = 2)]
    public class Conversation : ScriptableObject
    {
        public CharacterMetaData leftSpeaker;
        public CharacterMetaData rightSpeaker;
        public Line[] lines;
    }
}
