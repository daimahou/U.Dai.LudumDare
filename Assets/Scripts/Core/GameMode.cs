using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core
{
    [UsedImplicitly]
    public class GameMode : IInitializable
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        private int m_currentSceneIndex = -1;

        private MemoryLoop m_memoryLoop;
        
        private readonly Memory m_memoryA = new Memory();
        private readonly Memory m_memoryB = new Memory();
        private readonly Memory m_memoryC = new Memory();

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
            ResetMemory();
            m_sceneLoader.LoadScene(m_currentSceneIndex, LoadSceneMode.Single, container => { InitializeMemory(); });
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
            Load();
        }

        private void ResetMemory()
        {
            m_memoryA.ResetMemory();
            m_memoryB.ResetMemory();
            m_memoryC.ResetMemory();
        }
    }
}