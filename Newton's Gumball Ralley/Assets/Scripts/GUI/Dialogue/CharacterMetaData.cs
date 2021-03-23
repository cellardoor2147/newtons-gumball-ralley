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
        public Sprite worriedPortrait;
        public Sprite skepticalPortrait;
        public Sprite surprisedPortrait;
        public Sprite teachingPortrait;
        public SoundMetaData neutralSound;
        public SoundMetaData happySound;
        public SoundMetaData worriedSound;
        public SoundMetaData skepticalSound;
        public SoundMetaData surprisedSound;
        public SoundMetaData teachingSound;
    }
}
