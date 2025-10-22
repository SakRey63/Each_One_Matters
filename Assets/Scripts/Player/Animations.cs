using UnityEngine;

public class Animations : MonoBehaviour
{
    private const string IsRun = "IsRun";
    private const string IsFalling = "IsFalling";
    private const string IsWin = "IsWin";
    
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

    public void MoveWinAnimation()
    {
        _animator.SetTrigger(IsWin);
    }
}
