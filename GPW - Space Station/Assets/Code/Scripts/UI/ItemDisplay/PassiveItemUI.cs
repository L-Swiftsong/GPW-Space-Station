using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using TMPro;
using Entities.Player;
using UnityEngine.UI;
using Items.Healing;

public class PassiveItemUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI medkitCountText;
	[SerializeField] private TextMeshProUGUI keycardLevelText;

	[SerializeField] private Button _medkitUseButton;

	private PlayerInventory _playerInventory;
	private Medkit _medkit;
	private PlayerTablet _playerTablet;

	private void Awake()
	{
		if (_playerInventory == null)
			_playerInventory = PlayerManager.Instance.Player.GetComponent<PlayerInventory>();


		if (_medkit == null)
			_medkit = PlayerManager.Instance.Player.GetComponentInChildren<Medkit>();

		if (_medkitUseButton != null)
			_medkitUseButton.onClick.AddListener(OnMedkitUse);

		if (_playerTablet == null)
			_playerTablet = PlayerManager.Instance.Player.GetComponentInChildren<PlayerTablet>();
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

	public void OnMedkitUse()
	{
		if(_playerInventory.GetMedkitCount() > 0)
		{
			_medkit.StartHealing();
			_playerTablet.Unequip();
			//_playerInventory.RemoveMedkits(1);
		}
	}
}
