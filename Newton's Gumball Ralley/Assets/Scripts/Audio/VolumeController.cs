using UnityEngine;

namespace Audio
{
    public class VolumeController : MonoBehaviour
    {
        public void ChangeUniversalVolume(float universalVolume)
        {
            AudioManager.instance.SetUniversalVolume(universalVolume);
        }
    }
}
