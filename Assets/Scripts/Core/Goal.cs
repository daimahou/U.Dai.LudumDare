using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Goal : MonoBehaviour
    {
        private GameMode m_gameMode;
        private bool m_loading;

        [SerializeField] private AudioClip m_audioClip;
        
        [Inject]
        private void Construct(GameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        public void LoadNext() => StartCoroutine(Load());

        private IEnumerator Load()
        {
            AudioSource.PlayClipAtPoint(m_audioClip, Camera.main.transform.position);
            if(m_loading) yield break;

            m_loading = true;
            
            yield return new WaitForSeconds(.5f);
            m_gameMode.LoadNextLevel();
        }
    }
}