using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using Items.KeyItem;
using UnityEngine.Rendering;


public class EscapePodInteraction : MonoBehaviour, IInteractable
{
	public event System.Action OnSuccessfulInteraction;
	public event System.Action OnFailedInteraction;

	[SerializeField] private UseKeyItem[] _repairSpots;
	private UseKeyItem _activeRepairSpot;

	private bool _hasInteracted;

	private void Start()
	{
		_repairSpots = GetComponentsInChildren<UseKeyItem>();
	}

	public void Interact(PlayerInteraction interaction)
	{
		_activeRepairSpot = GetClosestRepairSpot(interaction.transform.position);

		if (_hasInteracted) return;
		_hasInteracted = true;

		OnSuccessfulInteraction?.Invoke();

		KeyItemManager.Instance.AllowKeyItemEquip();
	}

	public void ResetInteraction()
	{
		_hasInteracted = false;
		_activeRepairSpot = null;	
	}

	public void FailInteraction()
	{
		OnFailedInteraction?.Invoke();
	}

	private UseKeyItem GetClosestRepairSpot(Vector3 playerPosition)
	{
		UseKeyItem closestSpot = null;

		float closestDistance = Mathf.Infinity;

		foreach (UseKeyItem spot in _repairSpots)
		{
			float distance = (spot.transform.position - playerPosition).sqrMagnitude;

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestSpot = spot;
			}
		}

		return closestSpot;
	}

	public UseKeyItem GetActiveRepairSpot()
	{ 
		return _activeRepairSpot; 
	}

}


