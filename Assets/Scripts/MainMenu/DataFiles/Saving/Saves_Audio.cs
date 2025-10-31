using System;

namespace YG
{
    [Serializable]
    public class AudioData
    {
        public float VolumeMusic = 1f;
        public float VolumeSFX = 1f;
        public float VolumeUi = 1f;
        public float SpeedSideMovement = 50f;
    }

    public partial class SavesYG
    {
        public AudioData audio = new AudioData();
    }
}