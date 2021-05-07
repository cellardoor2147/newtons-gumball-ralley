using UnityEngine;

namespace GUI.Dialogue
{
    public enum Expression
    {
        Neutral = 0,
        Happy = 1,
        Oops = 2,
        Thinking = 3,
        Explaining = 4
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
        [TextArea(2, 5)]
        public string englishContent;
        [TextArea(2, 5)]
        public string spanishContent;
    }

    [CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Conversation", order = 2)]
    public class Conversation : ScriptableObject
    {
        public int worldIndex;
        public int levelIndex;
        public CharacterMetaData leftSpeaker;
        public CharacterMetaData rightSpeaker;
        public Line[] linesAtBeginningOfLevel;
        public Line[] linesAtEndOfLevel;
    }
}
