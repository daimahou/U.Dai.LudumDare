using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class Elevator : MonoBehaviour
    {
        private GridObject m_gridObject;
        private GridTile m_gridTile;

        private GridObject m_occupyingObject;
        
        [SerializeField] private int m_lowHeight = 0;
        [SerializeField] private int m_heightHeight = 1;
        [SerializeField] private int m_offset = -1;

        private void Awake()
        {
            m_gridObject = GetComponent<GridObject>();
            m_gridTile = m_gridObject.m_CurrentGridTile;
            m_gridTile.OnGridObjectEnter += (gridObject, tile) => { m_occupyingObject = gridObject; };
            m_gridTile.OnGridObjectExit += (gridObject, tile) => { m_occupyingObject = null; };
        }

        [UsedImplicitly]
        public void Open()
        {
            m_gridTile.m_TileHeight = m_heightHeight;

            var tileTransform = m_gridTile.transform;
            var newPos = tileTransform.position;
            newPos.y = m_offset + (float) m_heightHeight / 2;

            tileTransform.position = newPos;

            transform.position = newPos - Vector3.up * m_offset;
            if(m_occupyingObject) m_occupyingObject.transform.position = newPos - Vector3.up * m_offset;
        }

        [UsedImplicitly]
        public void Close()
        {
            m_gridTile.m_TileHeight = m_lowHeight;

            var tileTransform = m_gridTile.transform;
            var newPos = tileTransform.position;
            newPos.y = m_offset + (float) m_lowHeight / 2;

            tileTransform.position = newPos;

            transform.position = newPos - Vector3.up * m_offset;
            if(m_occupyingObject)  m_occupyingObject.transform.position = newPos - Vector3.up * m_offset;
        }
    }
}