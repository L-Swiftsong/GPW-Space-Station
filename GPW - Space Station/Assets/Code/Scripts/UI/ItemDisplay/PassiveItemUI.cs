using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using TMPro;
using Entities.Player;

public class PassiveItemUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI medkitCountText;
	[SerializeField] private TextMeshProUGUI keycardLevelText;

	private PlayerInventory _playerInventory;

	private void Awake()
	{
		if (_playerInventory == null)
		{
			_playerInventory = PlayerManager.Instance.Player.GetComponent<PlayerInventory>();
		}
	}

	private void Update()
	{
		if (_playerInventory != null)
		{
			UpdateMedkitCountUI();
			UpdateKeycardLevelUI();
		}
	}

	private void UpdateMedkitCountUI()
	{
		int medkitCount = _playerInventory.GetMedkitCount();
		medkitCountText.text = $"{medkitCount}";
	}

	private void UpdateKeycardLevelUI()
	{
		int keycardLevel = _playerInventory.GetDecoderSecurityLevel();
		keycardLevelText.text = $"{keycardLevel}";
	}
}
