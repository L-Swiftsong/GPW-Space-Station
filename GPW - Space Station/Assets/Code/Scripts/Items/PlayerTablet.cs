using Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTablet : MonoBehaviour
{
    private bool _isEquipped;


    [Header("References")]
    [SerializeField] private GameObject _worldCanvasGO;


    [Header("Debug")]
    [SerializeField] private bool _equip = false;


    private void Awake()
    {
        // Unequip (Without further unequip actions).
        _isEquipped = false;
        _worldCanvasGO.SetActive(false);
    }
    private void Update()
    {
        if (_equip && !_isEquipped)
        {
            Equip();
        }
        else if (!_equip && _isEquipped)
        {
            Unequip();
        }
    }


    private void Equip()
    {
        _isEquipped = true;
        _worldCanvasGO.SetActive(true);

        // Prevent unwanted input.
        PlayerInput.PreventMovementActions(this.GetType());
        PlayerInput.PreventCameraActions(this.GetType());
        PlayerInput.PreventInteractionActions(this.GetType());

        // Unlock the cursor.
        Cursor.lockState = CursorLockMode.None;
    }
    private void Unequip()
    {
        _isEquipped = false;
        _worldCanvasGO.SetActive(false);

        PlayerInput.RemoveMovementActionPrevention(this.GetType());
        PlayerInput.RemoveCameraActionPrevention(this.GetType());
        PlayerInput.RemoveInteractionActionPrevention(this.GetType());

        // Lock the cursor.
        Cursor.lockState = CursorLockMode.Locked;
    }
}
