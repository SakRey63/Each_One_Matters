using System;
using UnityEngine;
using YG;

namespace EachOneMatters.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        private const string HorizontalAxis = "Mouse X";

        [Header("Desktop")] [SerializeField] private float _mouseDeadZone = 0.1f;

        [Header("Mobile")] [SerializeField] private float _minSwipeDelta = 5f;
        [SerializeField] private float _sensitivity = 1f;
        [SerializeField] private float _maxOutput = 2f;

        private Vector2 _previousTouchPosition;
        private bool _isTouchActive;

        public event Action<float> DirectionChanged;
        public event Action OnEscapePressed;

        private void Update()
        {
            if (YG2.envir.isDesktop == false)
            {
                HandleTouchInput();
            }
            else
            {
                HandleEscapeInput();
                HandleMouseInput();
            }
        }

        private void HandleMouseInput()
        {
            float mouseX = Input.GetAxis(HorizontalAxis);

            if (Mathf.Abs(mouseX) > _mouseDeadZone)
            {
                DirectionChanged?.Invoke(Mathf.Clamp(mouseX, -_maxOutput, _maxOutput));
            }
            else
            {
                DirectionChanged?.Invoke(0f);
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                if (_isTouchActive)
                {
                    _isTouchActive = false;

                    DirectionChanged?.Invoke(0f);
                }

                return;
            }

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _isTouchActive = true;
                    _previousTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (_isTouchActive)
                    {
                        Vector2 currentPos = touch.position;
                        Vector2 delta = currentPos - _previousTouchPosition;

                        if (Mathf.Abs(delta.x) > _minSwipeDelta && Mathf.Abs(delta.y) < 30f)
                        {
                            float direction = delta.x * _sensitivity / Screen.width;
                            direction = Mathf.Clamp(direction * 10f, -_maxOutput, _maxOutput);

                            DirectionChanged?.Invoke(direction);
                        }

                        _previousTouchPosition = currentPos;
                    }

                    break;

                case TouchPhase.Ended:

                case TouchPhase.Canceled:

                    if (_isTouchActive)
                    {
                        _isTouchActive = false;

                        DirectionChanged?.Invoke(0f);
                    }

                    break;
            }
        }

        private void HandleEscapeInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscapePressed?.Invoke();
            }
        }
    }
}