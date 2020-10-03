using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Character : MonoBehaviour
    {
        private MoveHandler m_moveHandler;
        [ShowInInspector] private Memory m_memory;

        private bool m_memoryOverride;

        [Inject]
        private void Construct(MoveHandler moveHandler)
        {
            m_moveHandler = moveHandler;
        }

        public void InitializeMemory(Memory memory)
        {
            m_memory = memory;
        }

        public void OverrideMemory() => m_memoryOverride = true;

        public MovementResult Move(Vector2Int direction)
        {
            var moveResult = m_moveHandler.Move(direction);

            if (m_memoryOverride) ResetMemory();

            if(moveResult == MovementResult.Moved) Record(direction);

            return moveResult;
        }

        public void ExecuteMemoryStep()
        {
            if (m_memory == null || m_memory.IsEmpty) return;

            m_moveHandler.Move(m_memory.GetNextMove());
        }

        private void Record(Vector2Int actionDirection) => m_memory.Record(actionDirection);

        private void ResetMemory()
        {
            m_memoryOverride = false;
            m_memory.ResetMemory();
        }

        public void ResetPosition() => m_memory.ResetPosition();
    }
}