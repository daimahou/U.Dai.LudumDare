using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine;
using ModestTree;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class Controller : MonoBehaviour
    {
        [SerializeField] private List<Character> m_controllableCharacters;
        private List<Spikes> m_spikes;

        private Vector2Int m_currentInput = Vector2Int.zero;

        private Settings m_settings;
        private GameMode m_gameMode;
        
        private Character m_possessedCharacter;
        public int m_controlledCharacterIndex;

        private float m_lastProcessedTime;
        private static readonly int s_active = Animator.StringToHash("Active");

        [SerializeField] private int m_availableMove;
        public int AvailableMove => m_availableMove;

        [SerializeField] private bool m_allowNegative;

        [SerializeField] private AudioClip m_nullMoveClip;
        [SerializeField] private AudioClip m_moveClip;
        
        [Inject]
        private void Construct(Settings settings, GameMode gameMode)
        {
            m_settings = settings;
            m_gameMode = gameMode;
        }
        
        private void Start()
        {
            Possess(m_controlledCharacterIndex);
        }
        
        public void Possess(int characterIndex)
        {
            FindCharactersInScene();
            m_spikes = FindObjectsOfType<Spikes>().ToList();
            
            Assert.That(characterIndex < m_controllableCharacters.Count);
            
            if (m_possessedCharacter)
            {
                m_possessedCharacter.GetComponentInChildren<Animator>().SetBool(s_active, false);
                m_possessedCharacter.GetComponentInChildren<MeshRenderer>().material = m_settings.m_idleMaterial;
            }
            
            m_possessedCharacter = m_controllableCharacters[characterIndex];
            m_possessedCharacter.GetComponentInChildren<Animator>().SetBool(s_active, true);
            
            // reset memory
            m_possessedCharacter.OverrideMemory();
            m_controllableCharacters.Where(x => x != m_possessedCharacter).ForEach(x => x.ResetPosition());
            m_possessedCharacter.GetComponentInChildren<MeshRenderer>().material = m_settings.m_selectedMaterial;
        }

        private void FindCharactersInScene()
        {
            if (m_controllableCharacters.Count > 0) return;

            m_controllableCharacters = new List<Character>();
            
            var charA = GameObject.FindWithTag("CharacterA")?.GetComponent<Character>();

            if (charA) m_controllableCharacters.Add(charA);
            
            var charB = GameObject.FindWithTag("CharacterB")?.GetComponent<Character>();
            
            if(charB) m_controllableCharacters.Add(charB);
            
            var charC = GameObject.FindWithTag("CharacterC")?.GetComponent<Character>();
            
            if(charC) m_controllableCharacters.Add(charC);

            Assert.That(m_controllableCharacters.Count > 0, "no playable character in the scene !!! ");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameEventMessage.SendEvent("Menu");
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                m_gameMode.ResetLevel();
                return;
            }
            if (SwitchCharacter()) return;
            
            if(GetKeyboardInput()) ExecuteInput();
        }

        private bool SwitchCharacter()
        {
            var characterIndex = -1;
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                characterIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                characterIndex = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {                
                characterIndex = 2;
            }
            
            FindCharactersInScene();
            
            if (characterIndex < 0 || characterIndex >= m_controllableCharacters.Count) return false;
            
            m_gameMode.ReloadCurrentScene(characterIndex);
            
            return true;
        }

        private bool GetKeyboardInput() 
        {
            var invertX = m_settings.m_invertXAxis ? -1 : 1;
            var invertY = m_settings.m_invertYAxis ? -1 : 1;
            
            var input = m_settings.m_swapAxis
                ? new Vector2(Input.GetAxisRaw("Vertical") * invertX, Input.GetAxisRaw("Horizontal") * invertY)
                : new Vector2(Input.GetAxisRaw("Horizontal") * invertX, Input.GetAxisRaw("Vertical") * invertY);

            var flag = Input.GetButtonDown("Up") || 
                       Input.GetButtonDown("Down") || 
                       Input.GetButtonDown("Left") ||
                       Input.GetButtonDown("Right") || 
                       Input.GetKeyDown(KeyCode.Space);
            
            m_currentInput = flag ? input.ToVector2Int() : Vector2Int.zero;

            return flag;
        }
        
        private void ExecuteInput(Vector2Int? direction = null)
        {
            if (!m_allowNegative && m_gameMode.MoveLeft <= 0 && m_possessedCharacter.HasMemoryEmpty)
            {
                m_possessedCharacter.Kill("Out of Move");
                GameEventMessage.SendEvent("No more Moves");
                return;
            }
            
            if (m_possessedCharacter.IsDead) return;
            
            var actionDirection = direction ?? m_currentInput;
            
            var time = Time.time;
            if (time - m_lastProcessedTime < .15f) return;
            m_lastProcessedTime = time;

            if (!m_possessedCharacter.CanMoveTo(actionDirection) && actionDirection != Vector2Int.zero) return;

            m_gameMode.DecrementMoveLeft();
            
            var result = m_possessedCharacter.Move(actionDirection);
            
            m_controllableCharacters.Where(x => x != m_possessedCharacter).ForEach(x => x.ExecuteMemoryStep());
            
            if(result != MovementResult.Moved && actionDirection != Vector2Int.zero)
            {
                result = m_possessedCharacter.Move(actionDirection);
                if (result != MovementResult.Moved) m_possessedCharacter.Move(Vector2Int.zero);
            }
            
            m_spikes.ForEach(x => x.Toggle());

            if (actionDirection == Vector2Int.zero)
            {
                AudioSource.PlayClipAtPoint(m_nullMoveClip, Camera.main.transform.position);
            }
            else
            {
                AudioSource.PlayClipAtPoint(m_moveClip, Camera.main.transform.position);
            }
            // If movement was successful or failed for some other reason we remove the current input
            m_currentInput = Vector2Int.zero;
        }
        
        [Serializable]
        public class Settings
        {
            public bool m_swapAxis;
            public bool m_invertXAxis;
            public bool m_invertYAxis;

            public Material m_idleMaterial;
            public Material m_selectedMaterial;
            public Material m_stuckMaterial;
        }
    }
}