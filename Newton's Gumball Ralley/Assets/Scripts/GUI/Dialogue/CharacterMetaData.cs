using UnityEngine;

namespace GUI.Dialogue
{
    [CreateAssetMenu(fileName = "CharacterMetaData", menuName = "ScriptableObjects/CharacterMetaData", order = 3)]
    public class CharacterMetaData : ScriptableObject
    {
        public string fullName;
        public Sprite neutralPortrait;
        public Sprite happyPortrait;
        public Sprite worriedPortrait;
        public Sprite skepticalPortrait;
        public Sprite surprisedPortrait;
        public Sprite teachingPortrait;
    }
}
