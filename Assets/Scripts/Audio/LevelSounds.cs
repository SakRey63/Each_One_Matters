using UnityEngine;

namespace EachOneMatters.Audio
{
    public class LevelSounds : MonoBehaviour
    {
        [Header("Основные треки")] [SerializeField]
        private AudioSource _backgroundMusic;

        [SerializeField] private AudioSource _menuPauseMusic;
        [SerializeField] private AudioSource _winMusic;
        [SerializeField] private AudioSource _gameOverMusic;

        [Header("SFX")] [SerializeField] private AudioSource _onClickedMusic;
        [SerializeField] private AudioSource _errorMusic;
        [SerializeField] private AudioSource _upgradeMusic;
        [SerializeField] private AudioSource _callHelpMusic;

        private void Awake()
        {
            ValidateAudioSources();
        }

        public void PlayBackgroundMusic()
        {
            StopRelevantTracks();
            _backgroundMusic?.Play();
        }

        public void PlayPauseMusic()
        {
            _backgroundMusic?.Pause();
            _menuPauseMusic?.Play();
        }

        public void PlayWinMusic()
        {
            StopRelevantTracks();
            _winMusic?.Play();
        }

        public void PlayGameOverMusic()
        {
            _backgroundMusic?.Pause();
            _gameOverMusic?.Play();
        }

        public void PlayButtonClick()
        {
            _onClickedMusic?.Play();
        }

        public void PlayErrorSound()
        {
            _errorMusic?.Play();
        }

        public void PlayUpgradeSound()
        {
            _upgradeMusic?.Play();
        }

        public void PlayCallHelpSound()
        {
            _callHelpMusic?.Play();
        }

        private void ValidateAudioSources()
        {
            if (_backgroundMusic == null) Debug.LogError("BackgroundMusic не назначен");
            if (_menuPauseMusic == null) Debug.LogError("MenuPauseMusic не назначен");
            if (_winMusic == null) Debug.LogError("WinMusic не назначен");
            if (_gameOverMusic == null) Debug.LogError("GameOverMusic не назначен");
            if (_onClickedMusic == null) Debug.LogError("OnClickedMusic не назначен");
        }

        private void StopRelevantTracks()
        {
            if (_backgroundMusic != null && _backgroundMusic.isPlaying)
                _backgroundMusic.Stop();

            if (_menuPauseMusic != null && _menuPauseMusic.isPlaying)
                _menuPauseMusic.Stop();

            if (_gameOverMusic != null && _gameOverMusic.isPlaying)
                _gameOverMusic.Stop();
        }
    }
}