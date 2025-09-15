using UnityEngine;

public class LevelSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _backgroundMusic;
    [SerializeField] private AudioSource _menuPauseMusic;
    [SerializeField] private AudioSource _winMusic;
    [SerializeField] private AudioSource _gameOverMusic;
    [SerializeField] private AudioSource _onClickedMusic;
    [SerializeField] private AudioSource _errorMusic;
    [SerializeField] private AudioSource _upgradeMusic;
    [SerializeField] private AudioSource _callHelpMusic;
    
    public void CreateBackgroundMusic()
    {
        _menuPauseMusic.Stop();
        _gameOverMusic.Stop();
        _backgroundMusic.Play();
    }

    public void CreateMusicMenuPause()
    {
        _backgroundMusic.Pause();
        _menuPauseMusic.Play();
    }

    public void CreateWinMusic()
    {
        _backgroundMusic.Stop();
        _winMusic.Play();
    }

    public void CreateGameOverMusic()
    {
        _backgroundMusic.Pause();
        _gameOverMusic.Play();
    }

    public void CreateOnClickedMusic()
    {
        _onClickedMusic.Play();
    }

    public void CreateErrorMusic()
    {
        _errorMusic.Play();
    }

    public void CreateUpgradeMusic()
    {
        _upgradeMusic.Play();
    }

    public void CreateCallHelpMusic()
    {
        _callHelpMusic.Play();
    }
}
