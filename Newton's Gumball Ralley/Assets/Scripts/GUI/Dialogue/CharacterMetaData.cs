using UnityEngine;
using Audio;

namespace GUI.Dialogue
{
    [CreateAssetMenu(fileName = "CharacterMetaData", menuName = "ScriptableObjects/CharacterMetaData", order = 3)]
    public class CharacterMetaData : ScriptableObject
    {
        public string fullName;
        public Sprite neutralPortrait;
        public Sprite happyPortrait;
        public Sprite oopsPortrait;
        public Sprite thinkingPortrait;
        public Sprite explainingPortrait;
        public SoundMetaData neutralSound;
        public SoundMetaData happySound;
        public SoundMetaData oopsSound;
        public SoundMetaData thinkingSound;
        public SoundMetaData explainingSound;
    }
}
