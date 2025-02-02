using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Environment.Doors
{
    [RequireComponent(typeof(Door))]
    public class DoorAudio : MonoBehaviour
    {
        private Door _door;


        [Header("Sound Clips")]
        [SerializeField] private AudioClip _doorOpenClip;
        [SerializeField] private AudioClip _doorCloseClip;

        [Space(5)]
        [SerializeField] private SFXManager.AudioValues _audioValues = new SFXManager.AudioValues(minDistance: 3.0f, maxDistance: 40.0f);
        [SerializeField] private float _pitchVariance = 0.05f;


        private void Awake() => _door = GetComponent<Door>();
        private void OnEnable() => _door.OnOpenStateChanged += Door_OnOpenStateChanged;
        private void OnDisable() => _door.OnOpenStateChanged -= Door_OnOpenStateChanged;


        private void Door_OnOpenStateChanged(bool isNowOpen)
        {
            SFXManager.AudioValues audioValues = _audioValues;
            audioValues.Pitch += Random.Range(-_pitchVariance, _pitchVariance);

            SFXManager.PlayClipAtPosition(isNowOpen ? _doorOpenClip : _doorCloseClip, transform.position, audioValues);
        }
    }
}