using System;
using Core;
using Doozy.Engine.UI;
using UnityEngine;
using Zenject;

namespace UI
{
    public class RestartButton : MonoBehaviour
    {
        private GameMode m_gameMode;
        private UIButton m_button;

        [Inject]
        public void Construct(GameMode gameMode)
        {
            m_gameMode = gameMode;
        }

        private void Awake()
        {
            m_button = GetComponent<UIButton>();

            m_button.OnClick.OnTrigger.Action = o => m_gameMode.ResetLevel();
        }
    }
}