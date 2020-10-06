using System.Collections;
using Core;
using Doozy.Engine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelButton : MonoBehaviour
    {
        private UIButton m_button;
        private TextMeshProUGUI m_buttonName;
        [ShowInInspector] private GameMode m_gameMode;

        private int m_levelIndex;

        [Inject]
        private void Construct(GameMode gameMode)
        {
            m_gameMode = gameMode;
        }
        
        private void Awake()
        {
            m_buttonName = GetComponentInChildren<TextMeshProUGUI>();
            m_button = GetComponent<UIButton>();

            m_levelIndex = transform.GetSiblingIndex() + 1;
            m_buttonName.SetText(m_levelIndex.ToString());

            m_button.OnClick.OnTrigger.Action = o => StartCoroutine(LoadLevel());
        }

        private IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(.5f);
            m_gameMode.LoadScene(m_levelIndex);
        }
    }
}