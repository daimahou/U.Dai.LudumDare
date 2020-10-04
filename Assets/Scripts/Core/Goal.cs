using UnityEngine;
using Zenject;

namespace Core
{
    public class Goal : MonoBehaviour
    {
        private GridObject m_gridObject;
        private GridTile m_gridTile;

        private GameMode m_gameMode;
        
        private void Awake()
        {
            m_gridObject = GetComponent<GridObject>();
            m_gridTile = m_gridObject.m_CurrentGridTile;
            
            m_gridTile.OnGridObjectEnter += (gridObject, tile) => { m_gameMode.LoadNextLevel(); };
        }

        [Inject]
        private void Construct(GameMode gameMode)
        {
            m_gameMode = gameMode;
        }
    }
}