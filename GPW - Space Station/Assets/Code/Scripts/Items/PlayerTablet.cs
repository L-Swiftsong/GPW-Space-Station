using System.Collections;
using System.Collections.Generic;
using UI.Popups;
using UnityEngine;

public class PlayerTablet : MonoBehaviour
{
    private bool _isEquipped;


    [Header("References")]
    [SerializeField] private GameObject _rootGO;
    [SerializeField] private GameObject _worldCanvasGO;
    [SerializeField] private ScreenSpacePopupTrigger _popupTrigger;
    [SerializeField] private AudioTrigger _audioTrigger;

    [Space(5)]
    [SerializeField] private Animator _animationController;
    private const string EQUIP_ANIMATION_VARIABLE_IDENTIFIER = "IsEquipped";


    [Header("Unequipping")]
    [SerializeField] private float _unequipTime = 0.5f;
    private Coroutine _disableSelfCoroutine;


    [Header("UI Sections")]
    [SerializeField] private Transform _sectionRootsContainer;

    private bool _hasOpenItemTab = false;


    [System.Serializable]
    private struct MenuTypeTransformPair
    {
        public PlayerTabletMenu MenuType;
        public UI.TabGroup.TabButton CorrespondingTabButton;
    }
    [SerializeField] [ReadOnlyWhenPlaying] private MenuTypeTransformPair[] _tabletMenus;
    private Dictionary<PlayerTabletMenu, UI.TabGroup.TabButton> _menuTypeToTabButtonDictionary;


    private void Awake()
    {
        // Start unequipped, but without performing things like cursor lock or input prevention removal.
        _isEquipped = false;
        _worldCanvasGO.SetActive(false);
        _animationController.SetBool(EQUIP_ANIMATION_VARIABLE_IDENTIFIER, false);

        _menuTypeToTabButtonDictionary = new Dictionary<PlayerTabletMenu, UI.TabGroup.TabButton>();
        for (int i = 0; i < _tabletMenus.Length; ++i)
        {
            _menuTypeToTabButtonDictionary.Add(_tabletMenus[i].MenuType, _tabletMenus[i].CorrespondingTabButton);
        }

        SubscribeToInput();
    }
    private void OnDestroy()
    {
        UnsubscribeFromInput();
        AllowInput();
    }


    #region Input

    private void SubscribeToInput()
    {
        PlayerInput.OnPauseGamePerformed += PlayerInput_OnPauseGamePerformed;
        PlayerInput.OnOpenJournalPerformed += PlayerInput_OnOpenJournalPerformed;
        PlayerInput.OnOpenItemsPerformed += PlayerInput_OnOpenItemsPerformed;
    }
    private void UnsubscribeFromInput()
    {
        PlayerInput.OnPauseGamePerformed -= PlayerInput_OnPauseGamePerformed;
        PlayerInput.OnOpenJournalPerformed -= PlayerInput_OnOpenJournalPerformed;
        PlayerInput.OnOpenItemsPerformed -= PlayerInput_OnOpenItemsPerformed;
    }

    private void PlayerInput_OnPauseGamePerformed() => ToggleEquip();
    private void PlayerInput_OnOpenJournalPerformed() => ToggleEquip(PlayerTabletMenu.Journal);
    private void PlayerInput_OnOpenItemsPerformed()
    {
        if (!_hasOpenItemTab)
        {
            _popupTrigger.Trigger();
            _audioTrigger.PlaySound();
            _hasOpenItemTab = true;
        }

        ToggleEquip(PlayerTabletMenu.Items);
    }


    private void PreventInput()
    {
        PlayerInput.PreventMovementActions(this.GetType());
        PlayerInput.PreventCameraActions(this.GetType());
        PlayerInput.PreventInteractionActions(this.GetType());
    }
    private void AllowInput()
    {
        PlayerInput.RemoveMovementActionPrevention(this.GetType());
        PlayerInput.RemoveCameraActionPrevention(this.GetType());
        PlayerInput.RemoveInteractionActionPrevention(this.GetType());
    }

    #endregion


    #region Equipping & Unequipping

    private void ToggleEquip() => ToggleEquip(PlayerTabletMenu.Objective);
    private void ToggleEquip(PlayerTabletMenu selectedMenu)
    {
        if (_isEquipped)
        {
            Unequip();
        }
        else
        {
            Equip(selectedMenu);
        }
    }
    public void Equip() => Equip(PlayerTabletMenu.Objective);
    public void Equip(PlayerTabletMenu selectedMenu)
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
        PreventInput();

        // Unlock the cursor.
        Cursor.lockState = CursorLockMode.None;


        // Select the desired menu (If it exists).
        if (_menuTypeToTabButtonDictionary.TryGetValue(selectedMenu, out UI.TabGroup.TabButton tabButton))
        {
            tabButton.OnPointerClick(null);
        }
    }
    public void Unequip()
    {
		_isEquipped = false;
        _worldCanvasGO.SetActive(false);

        _animationController.SetBool(EQUIP_ANIMATION_VARIABLE_IDENTIFIER, false);

        AllowInput();

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

[System.Serializable]
public enum PlayerTabletMenu
{
    Objective = 0,
    Journal = 1,
    Items = 2
}
