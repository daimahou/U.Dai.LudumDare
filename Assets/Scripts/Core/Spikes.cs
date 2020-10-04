using UnityEngine;

namespace Core
{
    public class Spikes : MonoBehaviour
    {
        private Animator m_animator;
        private GridObject m_gridObject;

        [SerializeField] private int m_frequency = 3;
        private int m_currentFrequency;
        
        [SerializeField] private bool m_active;
        private bool m_stopped;
        
        private static readonly int s_pop = Animator.StringToHash("Pop");
        private static readonly int s_ready = Animator.StringToHash("Ready");

        private Character m_occupyingCharacter;

        private void Awake()
        {
            m_gridObject = GetComponent<GridObject>();
            m_animator = GetComponentInChildren<Animator>();
            m_animator.SetBool(s_pop, m_active);

            m_gridObject.m_CurrentGridTile.OnGridObjectEnter += (gridObject, tile) =>
            {
                m_occupyingCharacter = gridObject.GetComponent<Character>();
                
            };
            
            m_gridObject.m_CurrentGridTile.OnGridObjectExit += (gridObject, tile) =>
            {
                m_occupyingCharacter = null;
            };
        }

        public void Toggle()
        {
            if (m_stopped) return;
            
            m_currentFrequency++;

            m_active = m_currentFrequency % m_frequency == 0;
            
            if(m_frequency - m_currentFrequency % m_frequency  == 1)
            {
                m_animator.SetTrigger(s_ready);
            }
            else
            {
                m_animator.SetBool(s_pop, m_active);
            }

            if (m_active && m_occupyingCharacter)
            {
                m_occupyingCharacter.Kill();
                m_stopped = true;
            }
        }
    }
}