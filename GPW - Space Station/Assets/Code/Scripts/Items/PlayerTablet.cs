using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTablet : MonoBehaviour
{
    private bool _isEquipped;


    [Header("References")]
    [SerializeField] private GameObject _rootGO;
    [SerializeField] private GameObject _worldCanvasGO;

    [Space(5)]
    [SerializeField] private Animator _animationController;
    private const string EQUIP_ANIMATION_VARIABLE_IDENTIFIER = "IsEquipped";


    [Header("Unequipping")]
    [SerializeField] private float _unequipTime = 0.5f;
    private Coroutine _disableSelfCoroutine;


    [Header("UI Sections")]
    [SerializeField] private Transform _sectionRootsContainer;


    private void Awake()
    {
        // Start unequipped, but without performing things like cursor lock or input prevention removal.
        _isEquipped = false;
        _worldCanvasGO.SetActive(false);
        _animationController.SetBool(EQUIP_ANIMATION_VARIABLE_IDENTIFIER, false);


        SubscribeToInput();
    }
    private void OnDestroy() => UnsubscribeFromInput();


    #region Input

    private void SubscribeToInput() => PlayerInput.OnPauseGamePerformed += PlayerInput_OnPauseGamePerformed;
    private void UnsubscribeFromInput() => PlayerInput.OnPauseGamePerformed -= PlayerInput_OnPauseGamePerformed;

    private void PlayerInput_OnPauseGamePerformed() => ToggleEquip();

    #endregion


    #region Equipping & Unequipping

    private void ToggleEquip()
    {
        if (_isEquipped)
        {
            Unequip();
        }
        else
        {
            Equip();
        }
    }
    private void Equip()
    {
        _rootGO.SetActive(true);
        if (_disableSelfCoroutine != null)
        {
            StopCoroutine(_disableSelfCoroutine);
        }

        _isEquipped = true;
        _worldCanvasGO.SetActive(true);

        _animationController.SetBool(EQUIP_ANIMATION_VARIABLE_IDENTIFIER, true);

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

        _animationController.SetBool(EQUIP_ANIMATION_VARIABLE_IDENTIFIER, false);

        PlayerInput.RemoveMovementActionPrevention(this.GetType());
        PlayerInput.RemoveCameraActionPrevention(this.GetType());
        PlayerInput.RemoveInteractionActionPrevention(this.GetType());

        // Lock the cursor.
        Cursor.lockState = CursorLockMode.Locked;

        _disableSelfCoroutine = StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(_unequipTime);
        _rootGO.SetActive(false);
    }

    #endregion


    public void EnableSection(GameObject sectionGO)
    {
        foreach(Transform sectionRoot in _sectionRootsContainer)
        {
            sectionRoot.gameObject.SetActive(sectionRoot.gameObject == sectionGO);
        }
    }



}
