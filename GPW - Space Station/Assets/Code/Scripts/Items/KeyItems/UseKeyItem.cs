using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Collectables;
using UI.ItemDisplay;
using Interaction;
using TMPro;
using Audio;
using Entities.Player;


public class UseKeyItem : MonoBehaviour, IInteractable
{
    #region IInteractable Properties & Events

    [field: SerializeField] public bool IsInteractable { get; set; } = true;

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    [Header("Key Item")]
	[SerializeField] private KeyItemData _requiredKeyItem;
	[SerializeField] private Transform _keyItemPlacement;
	[SerializeField] private GameObject _ghostModel;
	[SerializeField] private GameObject _completeModel;

	[Header("Player Tablet")]
	[SerializeField] private PlayerTablet _playerTablet;
	[SerializeField] private GameObject _itemsTab;

	[Header("UI Feedback")]
	[SerializeField] private GameObject _feedbackText;
	[SerializeField] private Transform _feedbackTransform;

	[Header("Audio")]
	[SerializeField] private AudioClip correctItemSound;
	[SerializeField] private AudioClip incorrectItemSound;
	[SerializeField] private AudioSource audioSource;

	private bool _hasPlacedItem;

	public bool IsKeyItemCorrect(KeyItemData selectedKeyItem)
	{
		return _requiredKeyItem == selectedKeyItem;
	}

    private void Awake()
    {
        _completeModel.SetActive(false);
		_ghostModel.SetActive(false);
    }
    private void Start()
	{
		if (_playerTablet == null)
		{
			if (PlayerManager.Instance != null && PlayerManager.Instance.Player != null)
			{
				_playerTablet = PlayerManager.Instance.Player.GetComponentInChildren<PlayerTablet>(true);
			}
		}
	}

	public void TryUseKeyItem(KeyItemData selectedKeyItem)
	{
		if (_hasPlacedItem) return;

		if (IsKeyItemCorrect(selectedKeyItem))
		{
			SFXManager.Instance.PlayClipAtPosition(correctItemSound, transform.position, 1, 1, 2f);
			//KeyItemManager.Instance.PlaceItemAtLocation(_requiredKeyItem, _keyItemPlacement);
			OnSuccessfulInteraction?.Invoke();
			_hasPlacedItem = true;

			_ghostModel.SetActive(false);
            _completeModel.SetActive(true);

            UpdateUsedKeyItems();

            _playerTablet.Unequip();
		}
		else
		{
			FailInteraction();
		}
	}

	public void Interact(PlayerInteraction interaction)
	{
		if (interaction == null)
		{
			Debug.LogError("PlayerInteraction is null!");
			return;
		}

		Debug.Log("Interact() called on " + gameObject.name);


		if (_hasPlacedItem) return;

		_playerTablet.Equip(PlayerTabletMenu.Items);
		//_itemsTab.SetActive(true);

		var repairSpotManager = FindObjectOfType<RepairSpotManager>();

		if (repairSpotManager != null)
		{
			repairSpotManager.InteractWithRepairSpot(this);
			Debug.Log("Repair spot manager interaction triggered.");
		}
		else
		{
			Debug.LogError("RepairSpotManager not found in the scene.");
		}
	}

	public void Highlight()
	{
		IInteractable.StartHighlight(this.gameObject, ref _previousLayer);

		if (!_hasPlacedItem && CollectableManager.HasObtainedCollectable(_requiredKeyItem) && _ghostModel != null)
		{
			_ghostModel.SetActive(true);
		}
	}
	public void StopHighlighting()
	{
		IInteractable.StopHighlight(this.gameObject, _previousLayer);

		if (_ghostModel != null)
			_ghostModel.SetActive(false);
	}

    public void FailInteraction()
	{
		OnFailedInteraction?.Invoke();
		ShowErrorMessage("Incorrect item", 1f);
		SFXManager.Instance.PlayClipAtPosition(incorrectItemSound, transform.position, 1f, 1f, 2f);
	}

	private void ShowErrorMessage(string message, float duration)
	{
		GameObject feedbackInstance = Instantiate(_feedbackText, _feedbackTransform);

		TextMeshProUGUI feedbackText = feedbackInstance.GetComponent<TextMeshProUGUI>();

		if (feedbackText != null)
		{
			feedbackText.text = message;
		}

		Destroy(feedbackInstance, duration);
	}

	private void UpdateUsedKeyItems()
	{
        if (KeyItemEntryUI.s_KeyItemDataUsedState.TryAdd(_requiredKeyItem, _hasPlacedItem) == false)
		{
            // Failed to add (There is already an instance).
            KeyItemEntryUI.s_KeyItemDataUsedState[_requiredKeyItem] = _hasPlacedItem;
        }
    }



	public void SetHasPlacedItem(bool newValue)
	{
		_hasPlacedItem = newValue;
		UpdateUsedKeyItems();
	}
	public bool GetHasPlacedItem() => _hasPlacedItem;

	public KeyItemData GetKeyItemData() => _requiredKeyItem;
}