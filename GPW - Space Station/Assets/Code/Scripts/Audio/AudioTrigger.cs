using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class AudioTrigger : MonoBehaviour
{
	[SerializeField] private AudioClip _audioClip;

	[Space(5)]
	[SerializeField] private float _volume = 1.0f;
	[SerializeField] private float _minPitch = 1.0f;
	[SerializeField] private float _maxPitch = 1.0f;

	public void PlaySound()
	{
		SFXManager.Instance.PlayClipAtPosition(_audioClip, transform.position, minPitch: _minPitch, maxPitch: _maxPitch, volume: _volume);
	}
}
