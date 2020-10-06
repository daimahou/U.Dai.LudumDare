using System;
using System.Collections;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Character : MonoBehaviour
    {
        private MoveHandler m_moveHandler;
        private GameMode m_gameMode;
        
        [ShowInInspector, ReadOnly] 
        private Memory m_memory;

        [SerializeField] private GameObject m_highlight;

        private bool m_memoryOverride;

        [SerializeField] private Material m_movementCompleteMaterial;
        [SerializeField] private Material m_movementFailedMaterial;
        private MeshRenderer m_meshRenderer;
        public bool IsDead { get; set; }
        public bool HasMemoryEmpty => !m_memoryOverride || m_memory.Count <= 0;

        [SerializeField] private AudioClip m_audioClip;
        
        [Inject]
        private void Construct(MoveHandler moveHandler, GameMode gameMode)
        {
            m_moveHandler = moveHandler;
            m_gameMode = gameMode;
        }

        private void Awake()
        {
            m_meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void InitializeMemory(Memory memory)
        {
            m_memory = memory;
        }

        public void SetHighlight(bool b) => m_highlight.SetActive(b);

        public void OverrideMemory() => m_memoryOverride = true;

        public MovementResult Move(Vector2Int direction)
        {
            var moveResult = m_moveHandler.Move(direction);

            if (m_memoryOverride) ResetMemory();

            if(moveResult == MovementResult.Moved || direction == Vector2Int.zero) Record(direction);

            return moveResult;
        }

        public void ExecuteMemoryStep()
        {
            if (m_memory.HasStopped)
            {
                m_meshRenderer.material = m_movementFailedMaterial;
                return;
            }
            
            if (m_memory == null || m_memory.IsEmpty)
            {
                m_meshRenderer.material = m_movementCompleteMaterial;
                return;
            }

            var moveDirection = m_memory.GetNextMove();

            if (moveDirection == Vector2Int.zero)
            {
                StartCoroutine(WaitMessage());
                return;
            }
            
            var movementResult = m_moveHandler.Move(moveDirection);
            
            if (movementResult != MovementResult.Moved) Kill("Movement Blocked");
        }

        private void Record(Vector2Int actionDirection) => m_memory.Record(actionDirection);

        private void ResetMemory()
        {
            m_memoryOverride = false;
            m_gameMode.AddResetMemoryMove(m_memory.Count);
            m_memory.ResetMemory();
        }

        public void ResetPosition() => m_memory.ResetPosition();

        private Subject<string> m_killSubject;

        private IEnumerator WaitMessage()
        {
            m_killSubject?.OnNext("Waiting");
            yield return new WaitForSeconds(.5f);
            m_killSubject?.OnNext("");
        }
        
        public void Kill(string message)
        {
            AudioSource.PlayClipAtPoint(m_audioClip, Camera.main.transform.position);
            m_meshRenderer.material = m_movementFailedMaterial;
            m_memory.Position = -1;
            IsDead = true;
            m_killSubject?.OnNext(message);
        }

        public IObservable<string> GetKillAsObservable() => m_killSubject ?? (m_killSubject = new Subject<string>());

        public bool CanMoveTo(Vector2Int actionDirection) => m_moveHandler.CanMove(actionDirection);
    }
}