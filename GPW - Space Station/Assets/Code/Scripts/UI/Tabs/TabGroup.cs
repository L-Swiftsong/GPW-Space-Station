using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.TabGroup
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> _tabButtons = new List<TabButton>();
        private TabButton _selectedTab = null;

        [SerializeField] private bool _loopSelectFromInput = false;


        [Header("Selection Visualisation Settings")]
        [SerializeField] private Color _tabIdleColour;
        [SerializeField] private Color _tabHoverColour;
        [SerializeField] private Color _tabActiveColour;


        private void Awake()
        {
            for(int i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].AttachToGroup(this);
            }
        }
        private void OnEnable()
        {
            // Setup input stuff.
            SubscribeToInput();

            // Select the first element.
            SelectTab(_tabButtons[0]);
        }
        private void OnDisable() => UnsubscribeFromInput();

        #region Input Events

        private void SubscribeToInput()
        {
            PlayerInput.OnSelectNextTabPerformed += PlayerInput_OnSelectNextTabPerformed;
            PlayerInput.OnSelectPreviousTabPerformed += PlayerInput_OnSelectPreviousTabPerformed;
        }
        private void UnsubscribeFromInput()
        {
            PlayerInput.OnSelectNextTabPerformed -= PlayerInput_OnSelectNextTabPerformed;
            PlayerInput.OnSelectPreviousTabPerformed -= PlayerInput_OnSelectPreviousTabPerformed;
        }


        private void PlayerInput_OnSelectNextTabPerformed() => SelectNextTab();
        private void PlayerInput_OnSelectPreviousTabPerformed() => SelectPreviousTab();

        #endregion


        private void SelectNextTab()
        {
            int selectedTabIndex = _tabButtons.IndexOf(_selectedTab);

            if (selectedTabIndex == _tabButtons.Count - 1)
            {
                if (_loopSelectFromInput)
                    selectedTabIndex = 0;
            }
            else
            {
                selectedTabIndex++;
            }
            
            // Select the next tab.
            SelectTab(_tabButtons[selectedTabIndex]);
        }
        private void SelectPreviousTab()
        {
            int selectedTabIndex = _tabButtons.IndexOf(_selectedTab);

            if (selectedTabIndex == 0)
            {
                if (_loopSelectFromInput)
                    selectedTabIndex = _tabButtons.Count - 1;
            }
            else
            {
                selectedTabIndex--;
            }

            // Select the previous tab.
            SelectTab(_tabButtons[selectedTabIndex]);
        }


        public void EnterTabHover(TabButton tab)
        {
            // Reset all buttons.
            ResetTabs();

            // Set the colour of our hovered tab.
            if (_selectedTab != tab)
            {
                tab.SetBackgroundColour(_tabHoverColour);
            }
        }
        public void ExitTabHover(TabButton tab)
        {
            // Reset all buttons.
            ResetTabs();
        }
        public void SelectTab(TabButton tab)
        {
            // Set the selected tab.
            _selectedTab = tab;
            _selectedTab.SelectGroup();

            // Reset all buttons.
            ResetTabs();

            // Set the colour of our selected tab.
            tab.SetBackgroundColour(_tabActiveColour);
        }

        private void ResetTabs()
        {
            foreach(TabButton tab in _tabButtons)
            {
                if (tab != _selectedTab)
                {
                    tab.SetBackgroundColour(_tabIdleColour);
                }
            }
        }
    }
}