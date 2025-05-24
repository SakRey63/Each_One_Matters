using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const string Horizontal = "Horizontal";
    
    public event Action<float> DirectionChanged;
    private void Update()
    {
        if (Input.GetAxis(Horizontal) != 0)
        {
            DirectionChanged?.Invoke(Input.GetAxis(Horizontal));
        }
    }
}