using UnityEngine;

public class PoliceOfficerSound : MonoBehaviour
{
    [SerializeField] private AudioSource _scream;
    [SerializeField] private AudioSource _shoot;

    public void PlayScream()
    {
        _scream.Play();
    }

    public void PlayShoot()
    {
        _shoot.Play();
    }
}