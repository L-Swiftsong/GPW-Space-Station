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

        [Header("Sound Settings")]
        [SerializeField] private float _volume = 1.0f;
        [SerializeField] private float _minPitch = 0.95f;
        [SerializeField] private float _maxPitch = 1.05f;

        [Space(10)]
        [SerializeField] private float _minDistance = 3.0f;
        [SerializeField] private float _maxDistance = 40.0f;

        [Space(5)]
        [SerializeField] private bool _useCustomCurve;
        [SerializeField] private AnimationCurve _falloffCurve;


        private void Awake() => _door = GetComponent<Door>();
        private void OnEnable() => _door.OnOpenStateChanged += Door_OnOpenStateChanged;
        private void OnDisable() => _door.OnOpenStateChanged -= Door_OnOpenStateChanged;


        private void Door_OnOpenStateChanged(bool isNowOpen)
        {
            SFXManager.Instance.PlayClipAtPosition(isNowOpen ? _doorOpenClip : _doorCloseClip, transform.position, minPitch: _minPitch, maxPitch: _maxPitch, volume: _volume, minDistance: _minDistance, maxDistance: _maxDistance, falloffCurve: _useCustomCurve ? _falloffCurve : null);
        }
    }
}