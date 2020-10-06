using System;
using Doozy.Engine;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Core
{
    [UsedImplicitly, Serializable]
    public class GameMode : IInitializable
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        [ShowInInspector] private int m_currentSceneIndex = -1;

        private MemoryLoop m_memoryLoop;
        
        private readonly Memory m_memoryA = new Memory();
        private readonly Memory m_memoryB = new Memory();
        private readonly Memory m_memoryC = new Memory();

        public Subject<int> m_loadSubject;

        private int m_moveLeft;

        private Subject<int> m_moveLeftSubject;
        public int MoveLeft => m_moveLeft;

        public bool m_initialized;
        
        public void DecrementMoveLeft()
        {
            m_moveLeft--;
            m_moveLeftSubject?.OnNext(m_moveLeft);
            Debug.Log($"move left : {m_moveLeft}");
        }

        public void AddResetMemoryMove(int memoryCount)
        {
            m_moveLeft += memoryCount;
            m_moveLeftSubject?.OnNext(m_moveLeft);
            Debug.Log($"move left : {m_moveLeft}");
        }

        public void InitializeMove()
        {
            if (m_initialized)
            {
                Debug.Log("Already init");
                return;
            }
            
            m_moveLeft = Object.FindObjectOfType<Controller>().AvailableMove;
            m_moveLeftSubject?.OnNext(m_moveLeft);
            Debug.Log($"move left : {m_moveLeft}");
        }
        
        public GameMode(ZenjectSceneLoader sceneLoader)
        {
            m_sceneLoader = sceneLoader;
        }

        void IInitializable.Initialize()
        {
            InitializeMemory();
        }

        private void InitializeMemory()
        {
            var charA = GameObject.FindWithTag("CharacterA")?.GetComponent<Character>();

            if (charA) charA.InitializeMemory(m_memoryA);
            
            var charB = GameObject.FindWithTag("CharacterB")?.GetComponent<Character>();
            
            if(charB) charB.InitializeMemory(m_memoryB);
            
            var charC = GameObject.FindWithTag("CharacterC")?.GetComponent<Character>();
            
            if(charC) charC.InitializeMemory(m_memoryC);
        }

        private void Load()
        {
            m_initialized = false;
            ResetMemory();
            m_loadSubject?.OnNext(m_currentSceneIndex);
            
            m_sceneLoader.LoadScene(m_currentSceneIndex, LoadSceneMode.Single, container =>
            {
                InitializeMemory();
                InitializeMove();
                m_initialized = true;
            });
        }

        public void LoadScene(int index)
        {
            m_currentSceneIndex = index;
            Load();
        }
        
        public void ReloadCurrentScene(int characterIndex)
        {
            var current = SceneManager.GetActiveScene().name;
            m_sceneLoader.LoadScene(current, LoadSceneMode.Single,
                container =>
                {
                    InitializeMemory();
                    Object.FindObjectOfType<Controller>().m_controlledCharacterIndex = characterIndex;
                });
        }

        public void LoadNextLevel()
        {
            m_currentSceneIndex++;
            if (m_currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                GameEventMessage.SendEvent("Game Over");
                return;
            }
            Load();
        }

        private void ResetMemory()
        {
            m_memoryA.ResetMemory();
            m_memoryB.ResetMemory();
            m_memoryC.ResetMemory();
        }

        public IObservable<int> GetCurrentSceneAsObservable() => m_loadSubject ?? (m_loadSubject = new Subject<int>());

        public void ResetLevel()
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            LoadScene(index);
        }

        public IObservable<int> GetMoveLeftAsObservable() =>
            m_moveLeftSubject ?? (m_moveLeftSubject = new Subject<int>());
    }
}