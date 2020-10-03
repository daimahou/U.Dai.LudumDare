using System;
using JetBrains.Annotations;
using ModestTree;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class MoveHandler
    {
        private readonly GridObject m_gridObject;
        private readonly GridMovement m_gridMovement;

        private readonly Settings m_settings;
        
        public MoveHandler(GridObject gridObject, GridMovement gridMovement, Settings settings)
        {
            m_gridObject = gridObject;
            m_gridMovement = gridMovement;
            m_settings = settings;
        }

        public MovementResult Move(Vector2Int direction)
        {
            Assert.That(direction != Vector2Int.zero);
            
            var targetPosition = m_gridObject.m_GridPosition + direction;
            return m_gridMovement.TryMoveToNeighborInPosition(targetPosition, m_settings.m_animateMovement, m_settings.m_rotateTowardsDirection);
        }

        [Serializable]
        public class Settings
        {
            public bool m_animateMovement;
            public bool m_rotateTowardsDirection;
        }
    }
}