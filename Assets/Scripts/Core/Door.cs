using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private bool m_opened;
        
        private Animator m_animator;
        private InteractOnGridObject m_interactOnGridObject;
        private MovementBlocker m_movementBlocker;
        private static readonly int s_open = Animator.StringToHash("Open");

        private bool m_occupied;
        private bool m_closing;
        
        private void Awake()
        {
            m_interactOnGridObject = GetComponent<InteractOnGridObject>();
            m_movementBlocker = GetComponent<MovementBlocker>();
            m_animator = GetComponentInChildren<Animator>();

            m_interactOnGridObject.m_OnEnterTile.AddListener(() => m_occupied = true);
            m_interactOnGridObject.m_OnExitTile.AddListener(() =>
            {
                m_occupied = false;
                if(m_closing) Close();
                
            });
            
            if(m_opened) Open();
        }

        [UsedImplicitly]
        public void Open()
        {
            m_animator.SetBool(s_open, true);
            m_movementBlocker.BlocksMovement = false;
        }

        [UsedImplicitly]
        public void Close()
        {
            if (m_occupied)
            {
                m_closing = true;
                return;
            }

            m_closing = false;
            m_animator.SetBool(s_open, false);
            m_movementBlocker.BlocksMovement = true;
        }
    }
}