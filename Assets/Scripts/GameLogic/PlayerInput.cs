using System;
using UnityEngine;
using YG;

public class PlayerInput : MonoBehaviour
{
    private const string HorizontalAxis = "Mouse X";
    
    [SerializeField] private float _swipeThreshold = 50f;    
    [SerializeField] private float _deadZone = 0.1f;
    [SerializeField] private float _direction = 1f;
    
    private Vector2 _startTouchPosition;
    private bool _isTouchStarted;
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
        
        if (Mathf.Abs(mouseX) > _deadZone)
        {
            DirectionChanged?.Invoke(mouseX);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            _isTouchActive = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _isTouchActive = true;
                _startTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:
                
                if (_isTouchActive)
                {
                    Vector2 currentPos = touch.position;
                    Vector2 delta = currentPos - _startTouchPosition;

                    if (Mathf.Abs(delta.x) > _swipeThreshold && Mathf.Abs(delta.y) < 30f)
                    {
                        if (delta.x > 0)
                        {
                            DirectionChanged?.Invoke(_direction); 
                        }
                        else
                        {
                            DirectionChanged?.Invoke(-_direction); 
                        }
                        
                        _startTouchPosition = currentPos;
                    }
                }
                
                break;

            case TouchPhase.Ended:
                
            case TouchPhase.Canceled:
                _isTouchActive = false;
                _startTouchPosition = Vector2.zero;
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