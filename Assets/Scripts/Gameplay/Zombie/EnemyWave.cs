using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    private const string IsRun = "IsRun";
    
    [SerializeField] private Animator _animator;

    public void PauseAnimation()
    {
        _animator.SetBool(IsRun, false);
    }

    public void ResumeAnimation()
    {
        _animator.SetBool(IsRun, true);
    }
}
