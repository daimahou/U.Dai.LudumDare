using UnityEngine;

namespace Core
{
    public class Spikes : MonoBehaviour
    {
        private Animator m_animator;
        private GridObject m_gridObject;

        [SerializeField, Range(0, 10)] private int m_frequency = 3;
        [SerializeField, Range(0, 10)] private int m_currentFrequency;
        
        private bool m_active;
        private bool m_stopped;
        
        private static readonly int s_pop = Animator.StringToHash("Pop");
        private static readonly int s_ready = Animator.StringToHash("Ready");

        private Character m_occupyingCharacter;

        private void Awake()
        {
            m_gridObject = GetComponent<GridObject>();
            m_animator = GetComponentInChildren<Animator>();

            m_currentFrequency %= m_frequency;
            m_active = m_currentFrequency == 0;
            
            UpdateAnimation();

            m_gridObject.m_CurrentGridTile.OnGridObjectEnter += (gridObject, tile) =>
            {
                m_occupyingCharacter = gridObject.GetComponent<Character>();
            };
            
            m_gridObject.m_CurrentGridTile.OnGridObjectExit += (gridObject, tile) =>
            {
                m_occupyingCharacter = null;
            };
        }

        private void UpdateAnimation()
        {
            if (m_frequency - m_currentFrequency == 1)
            {
                m_animator.SetTrigger(s_ready);
            }
            else
            {
                m_animator.SetBool(s_pop, m_active);
            }
        }

        public void Toggle()
        {
            if (m_stopped) return;
            
            m_currentFrequency++;
            m_currentFrequency %= m_frequency;

            m_active = m_currentFrequency == 0;

            UpdateAnimation();

            if (m_active && m_occupyingCharacter)
            {
                m_occupyingCharacter.Kill("");
                m_stopped = true;
            }
        }
    }
}