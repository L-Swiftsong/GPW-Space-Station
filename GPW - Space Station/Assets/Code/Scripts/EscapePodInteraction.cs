using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
using Items.KeyItem;


public class EscapePodInteraction : MonoBehaviour, IInteractable
{
    #region IInteractable Properties & Events

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    public event System.Action OnPlayerInteracted;

	private bool _hasInteracted;


	public void Interact(PlayerInteraction interaction)
	{
		if (_hasInteracted) return;
		_hasInteracted = true;

		OnSuccessfulInteraction?.Invoke();

		KeyItemManager.Instance.AllowKeyItemEquip();
    }
    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


    public void ResetInteraction()
	{
		_hasInteracted = false;
	}

	public void FailInteraction()
	{
		OnFailedInteraction?.Invoke();
	}

}


