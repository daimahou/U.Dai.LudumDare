using Core;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class MoveLeftDisplay : MonoBehaviour
    {
        private GameMode m_gameMode;
        private TextMeshProUGUI m_text;

        [Inject]
        private void Construct(GameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
            m_gameMode.GetMoveLeftAsObservable().Subscribe(x => m_text.SetText($"Move{System.Environment.NewLine}{x}"));
        }
    }
}