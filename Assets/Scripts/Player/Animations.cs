using UnityEngine;

public class Animations : MonoBehaviour
{
    private const string IsRun = "IsRun";
    private const string IsFalling = "IsFalling";
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void MoveRunAnimation(bool isRun)
    {
        _animator.SetBool(IsRun, isRun);
    }

    public void MoveFallingAnimation()
    {
        _animator.SetTrigger(IsFalling);
    }
}
