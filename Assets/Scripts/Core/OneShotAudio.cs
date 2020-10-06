using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class OneShotAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip m_audioClip;

        [UsedImplicitly]
        public void Play()
        {
            AudioSource.PlayClipAtPoint(m_audioClip, Camera.main.transform.position);
        }
    }
}