 using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    [System.Serializable]
    public struct AudioInfo {
        [SerializeField]
        private AudioType audioType;

        [SerializeField]
        private AudioClip audioClip;

        public AudioType AudioType {
            get {
                return audioType;
            }
        }

        public AudioClip AudioClip {
            get {
                return audioClip;
            }
        }
    }

    [MustBeAssigned, SerializeField, Tooltip("Audio source for the sound effects")]
    private AudioSource soundEffectAudioSource;

    [SerializeField, Tooltip("The audios to play.")]
    private AudioInfo[] audioInfos;

    public void PlayAudioClipIfExists(AudioType audioType) {
        // Go through all the audio informations.
        foreach (var audioInfo in audioInfos) {
            // If we have the desired audio to play.
            if (audioInfo.AudioType == audioType) {
                soundEffectAudioSource.PlayOneShot(audioInfo.AudioClip);
                break;
            }
        }
    }
}
