using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class Controller : MonoBehaviour
    {
        [SerializeField] private List<Character> m_controllableCharacters;

        private Vector2Int m_currentInput = Vector2Int.zero;
        private Vector2Int m_queuedInput = Vector2Int.zero;

        private Settings m_settings;
        private GameMode m_gameMode;
        
        private Character m_possessedCharacter;

        private float m_lastProcessedTime;
        
        [Inject]
        private void Construct(Settings settings, GameMode gameMode)
        {
            m_settings = settings;
            m_gameMode = gameMode;
        }
        
        public void Possess(int characterIndex)
        {
            FindCharactersInScene();
            
            Assert.That(characterIndex < m_controllableCharacters.Count);
            
            if (m_possessedCharacter) m_possessedCharacter.GetComponent<GridMovement>().OnMovementEnd -= MovementEnded;
            m_possessedCharacter = m_controllableCharacters[characterIndex];
            m_possessedCharacter.GetComponent<GridMovement>().OnMovementEnd += MovementEnded;
            
            // reset memory
            m_possessedCharacter.OverrideMemory();
            m_controllableCharacters.Where(x => x != m_possessedCharacter).ForEach(x => x.ResetPosition());
        }

        private void FindCharactersInScene()
        {
            if (m_controllableCharacters.Count > 0) return;
            
            m_controllableCharacters = FindObjectsOfType<Character>().ToList();
         
            Assert.That(m_controllableCharacters.Count > 0, "no playable character in the scene !!! ");
        }

        private void Update()
        {
            if (SwitchCharacter()) return;
            
            m_currentInput = GetKeyboardInput();
            ExecuteInput();
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

        private Vector2Int GetKeyboardInput() 
        {
            var invertX = m_settings.m_invertXAxis ? -1 : 1;
            var invertY = m_settings.m_invertYAxis ? -1 : 1;
            
            var input = m_settings.m_swapAxis
                ? new Vector2(Input.GetAxisRaw("Vertical") * invertX, Input.GetAxisRaw("Horizontal") * invertY)
                : new Vector2(Input.GetAxisRaw("Horizontal") * invertX, Input.GetAxisRaw("Vertical") * invertY);

            var flag = Input.GetButtonDown("Up") || 
                       Input.GetButtonDown("Down") || 
                       Input.GetButtonDown("Left") ||
                       Input.GetButtonDown("Right");
            
            return flag ? input.ToVector2Int() : Vector2Int.zero;
        }
        
        private void ExecuteInput(Vector2Int? direction = null)
        {
            var actionDirection = direction ?? m_currentInput;
            
            if (actionDirection == Vector2Int.zero)
                return;
            
            var time = Time.time;

            Debug.Log($"t:{time} :: {m_lastProcessedTime} :: {time - m_lastProcessedTime}");
            if (time - m_lastProcessedTime < .15f) return;
            
            m_lastProcessedTime = time;

            var movementResult = m_possessedCharacter.Move(actionDirection);

            Debug.Log($"{movementResult}");
            // Queue the desired input if the movement is currently in cooldown
            if (movementResult == MovementResult.Cooldown) {
                m_queuedInput = m_currentInput;
                return;
            }

            // If movement was successful or failed for some other reason we remove the current input
            m_currentInput = Vector2Int.zero;
            
            m_controllableCharacters.Where(x => x != m_possessedCharacter).ForEach(x => x.ExecuteMemoryStep());
        }

        private void ExecuteQueuedInput()
        {
            // If there is not queued input direction
            if (m_queuedInput == Vector2Int.zero) return;

            var queuedInput = m_queuedInput;
            ClearQueuedInput();
            ExecuteInput(queuedInput);
        }

        private void ClearQueuedInput() => m_queuedInput = Vector2Int.zero;

        // Callback for the movement ended on GridMovement, used to execute queued input
        private void MovementEnded(GridMovement movement, GridTile fromGridPos, GridTile toGridPos) 
        {
            ExecuteQueuedInput();
        }
        
        [Serializable]
        public class Settings
        {
            public bool m_swapAxis;
            public bool m_invertXAxis;
            public bool m_invertYAxis;
        }
    }
}