using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UltEvents;

namespace UI.TabGroup
{
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private Image _background;
        private TabGroup _tabGroup;

        public UltEvent OnSelected;


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
            if (_firstSelectedElement != null)
            {
                EventSystem.current.SetSelectedGameObject(_firstSelectedElement.gameObject);
            }

            OnSelected?.Invoke();
        }


        public Selectable FirstSelectedElement => _firstSelectedElement;
        public Selectable LastSelectedElement => _lastSelectedElement;
    }
}