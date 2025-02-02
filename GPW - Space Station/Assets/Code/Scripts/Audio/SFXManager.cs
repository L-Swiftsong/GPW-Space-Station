using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class SFXManager
    {
        private static AudioSource s_oneShotAudioSource;
        private static AudioSource OneShotAudioSource
        {
            get
            {
                if (s_oneShotAudioSource == null)
                {
                    GameObject soundGameObject = new GameObject("Sound");
                    s_oneShotAudioSource = soundGameObject.AddComponent<AudioSource>();
                    s_oneShotAudioSource.spatialBlend = 1.0f; // Use 3D Audio.
                }

                return s_oneShotAudioSource;
            }
        }
    
        public class DetectableClipParameters : System.EventArgs
        {
            public Vector3 Origin;
            public float DetectableRadius;
        }
        public static event System.Action<DetectableClipParameters> OnDetectableSoundTriggered;


        public static void PlayClipAtPosition(AudioClip clip, Vector3 origin, AudioValues audioSettings)
        {
            // Cache the OneShotAudioSource to prevent repeated null checks.
            AudioSource audioSource = OneShotAudioSource;

            // Move the OneShotSource to our origin positon.
            audioSource.transform.position = origin;

            // Setup OneShotSource Values.
            audioSource.pitch = audioSettings.Pitch;
            audioSource.volume = audioSettings.Volume;
            audioSource.dopplerLevel = audioSettings.DopplerLevel;
            audioSource.spread = audioSettings.Spread;
            audioSource.minDistance = audioSettings.MinDistance;
            audioSource.maxDistance = audioSettings.MaxDistance;

            // Play our Audio Clip.
            audioSource.PlayOneShot(clip);
        }
        public static void PlayDetectableClipAtPosition(AudioClip clip, Vector3 origin, AudioValues audioSettings, float detectableRadius)
        {
            PlayClipAtPosition(clip, origin, audioSettings);

            OnDetectableSoundTriggered?.Invoke(new DetectableClipParameters() {
                Origin = origin,
                DetectableRadius = detectableRadius,
            });
        }


        [System.Serializable]
        public struct AudioValues
        {
            public float Pitch;
            public float Volume;

            public float DopplerLevel;
            public float Spread;
            public float MinDistance;
            public float MaxDistance;


            public AudioValues(float pitch = 1.0f, float volume = 1.0f,
                float dopplerLevel = 1.0f, float spread = 0.0f, float minDistance = 1.0f, float maxDistance = 500.0f)
            {
                this.Pitch = pitch;
                this.Volume = volume;

                this.DopplerLevel = dopplerLevel;
                this.Spread = spread;
                this.MinDistance = minDistance;
                this.MaxDistance = maxDistance;
            }

            public static AudioValues Default = new AudioValues();
        }
    }
}