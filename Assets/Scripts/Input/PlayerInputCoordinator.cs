using System;
using UnityEngine;
using YG;

namespace EachOneMatters.Inputs
{
    public class PlayerInputCoordinator : MonoBehaviour
    {
        private PlayerInput _playerInput;
    
        public event Action OnLevelMenuClicked;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            _playerInput.OnEscapePressed += OnEscapePressed;
        }

        private void OnDisable()
        {
            _playerInput.OnEscapePressed -= OnEscapePressed;
        }

        private void OnEscapePressed()
        {
            SetCursorState(false, true); 
            OnLevelMenuClicked?.Invoke();
        }
    
        private void SetCursorState(bool isLocked, bool isVisible)
        {
            if (YG2.envir.isDesktop)
            {
                Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
                Cursor.visible = isVisible;
            }
        }
    }
}