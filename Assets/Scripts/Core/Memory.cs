using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class Memory
    {
        [ShowInInspector] private readonly List<Vector2Int> m_recordedMoves = new List<Vector2Int>();
        
        [ShowInInspector] private int i;
        public bool IsEmpty => i >= m_recordedMoves.Count;

        public void ResetPosition() => i = 0;
        
        public void ResetMemory()
        {
            ResetPosition();
            m_recordedMoves.Clear();
        }
        
        public void Record(Vector2Int actionDirection)
        {
            m_recordedMoves.Add(actionDirection);
        }

        public Vector2Int GetNextMove()
        {
            var move = m_recordedMoves[i];
            i++;
            return move;
        }
    }
}