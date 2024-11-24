using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.Menus;

namespace UI.TabGroup
{
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SettingsMenuUI _settingsUI;


        [Header("References")]
        [SerializeField] private Image _background;
        private TabGroup _tabGroup;


        [Header("Selection Settings")]
        [SerializeField] private Selectable _firstSelectedElement;
        [SerializeField] private Selectable _lastSelectedElement;


        public void AttachToGroup(TabGroup parentGroup) => this._tabGroup = parentGroup;



        public void OnPointerClick(PointerEventData eventData) => _tabGroup.SelectTab(this);
        public void OnPointerEnter(PointerEventData eventData) => _tabGroup.EnterTabHover(this);
        public void OnPointerExit(PointerEventData eventData) => _tabGroup.ExitTabHover(this);


        public void SetBackgroundColour(Color newColor) => _background.color = newColor;
        public void SelectGroup()
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedElement.gameObject);

            _settingsUI.SetupBackButton(selectOnUp: _lastSelectedElement, selectOnDown: _firstSelectedElement);
        }
    }
}