using System;
using Core;
using TMPro;
using UniRx;
using UnityEngine;

namespace UI
{
    public class RanOutOfMoveDisplay : MonoBehaviour
    {
        private Character m_character;
        [SerializeField] private TextMeshPro m_text;

        private IDisposable m_disposable;
        
        private void Awake()
        {
            m_character = GetComponentInParent<Character>();
            m_disposable = m_character.GetKillAsObservable().Subscribe(msg =>
                {
                    m_text.gameObject.SetActive(true);
                    m_text.SetText(msg);
                });
        }

        private void OnDestroy()
        {
            m_disposable.Dispose();
        }
    }
}