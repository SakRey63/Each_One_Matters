using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    private bool _audioInitialized;

    private void Update()
    {
        if (_audioInitialized || !Input.anyKeyDown)
            return;
        
        AudioListener.volume = 1f;
        _audioInitialized = true;
    }
}