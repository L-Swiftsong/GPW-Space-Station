using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.World;
using Interaction;
using UI.Popups;

namespace Computers
{
    public class Computer : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private WorldSpaceButton _openLogsButton;
        [SerializeField] private WorldSpaceButton _closeLogsButton;

        [Space(5)]
        [SerializeField] private WorldSpaceButton _saveGameButton;
        [SerializeField] private ScreenSpacePopupTrigger _popupTrigger;
        [SerializeField] private AudioTrigger _audioTrigger;

        [Space(5)]
        [SerializeField] private WorldSpaceButton _enableTerminalButton;
        [SerializeField] private WorldSpaceButton _disableTerminalButton;

        [Space(5)]
        [SerializeField] private GameObject _enabledUIRoot;
        [SerializeField] private GameObject _mainEnabledUIRoot;
        [SerializeField] private GameObject _logsUIRoot;
        [SerializeField] private GameObject _disabledUIRoot;


        [Header("Logs")]
        [SerializeField] private Transform _terminalLogListRoot;
        [SerializeField] private GameObject _terminalLogListPrefab;

        [Space(5)]
        [SerializeField] private TerminalLogSO[] _terminalLogsArray = new TerminalLogSO[0];

        [Space(5)]
        [SerializeField] private TerminalLogUI _terminalLogUI;


        [Header("Performance")]
        [SerializeField] private float _maxCanvasDistance = 4.0f;
        private float _sqrMaxCanvasDistance => _maxCanvasDistance * _maxCanvasDistance;
        
        [SerializeField] private GameObject _worldCanvasGO;

        private const int DISABLE_CHECK_FREQUENCY = 5;
        private int _checkI = 0;


        private void Awake()
        {
            // Start disabled.
            DisableTerminal();

            // Generate UI.
            GenerateUI();
        }
        private void OnEnable()
        {
            // Subscribe to Buttons.
            _openLogsButton.OnSuccessfulInteraction += OpenLogsMenu;
            _closeLogsButton.OnSuccessfulInteraction += CloseLogsMenu;

            _saveGameButton.OnSuccessfulInteraction += SaveGame;

            _enableTerminalButton.OnSuccessfulInteraction += EnableTerminal;
            _disableTerminalButton.OnSuccessfulInteraction += DisableTerminal;
        }
        private void OnDisable()
        {
            // Unsubscribe from Buttons.
            _openLogsButton.OnSuccessfulInteraction -= OpenLogsMenu;
            _closeLogsButton.OnSuccessfulInteraction -= CloseLogsMenu;

            _saveGameButton.OnSuccessfulInteraction -= SaveGame;

            _enableTerminalButton.OnSuccessfulInteraction -= EnableTerminal;
            _disableTerminalButton.OnSuccessfulInteraction -= DisableTerminal;
        }

        private void Update()
        {
            // Check only every 'DISABLE_CHECK_FREQUENCY' frames for performance.
            if (_checkI % DISABLE_CHECK_FREQUENCY == 0)
            {
                // Disable the canvas if the player moves too far away.
                if ((Entities.Player.PlayerManager.Instance.Player.position - transform.position).sqrMagnitude >= _sqrMaxCanvasDistance)
                {
                    if (_worldCanvasGO.activeSelf)
                    {
                        DisableTerminal();
                        _worldCanvasGO.SetActive(false);
                    }
                }
                else
                {
                    if (!_worldCanvasGO.activeSelf)
                    {
                        _worldCanvasGO.SetActive(true);
                    }
                }
            }
            ++_checkI;
        }


        private void GenerateUI()
        {
            // Create and Setup our required data.
            foreach(TerminalLogSO terminalLogData in _terminalLogsArray)
            {
                WorldSpaceButton terminalLogInstance = Instantiate(_terminalLogListPrefab, _terminalLogListRoot).GetComponent<WorldSpaceButton>();
                terminalLogInstance.TrySetText(terminalLogData.LogName);
                terminalLogInstance.OnSuccessfulInteraction += () => DisplayLog(terminalLogData);
            }
        }


        public void OpenLogsMenu()
        {
            _mainEnabledUIRoot.SetActive(false);
            _logsUIRoot.SetActive(true);

            // Display the first log.
            if (_terminalLogsArray.Length > 0)
            {
                DisplayLog(_terminalLogsArray[0]);
            }
        }
        public void CloseLogsMenu()
        {
            _mainEnabledUIRoot.SetActive(true);
            _logsUIRoot.SetActive(false);
        }

        public void SaveGame()
        {
            Saving.SaveManager.Instance.SaveGameManual();

            _popupTrigger.Trigger();
            _audioTrigger.PlaySound();
        }

        public void DisplayLog(TerminalLogSO terminalLogSO)
        {
            Debug.Log("Displaying Log: " + terminalLogSO.LogName);
            _terminalLogUI.SetupLogUI(terminalLogSO);
        }

        public void EnableTerminal()
        {
            _enabledUIRoot.SetActive(true);
            _disabledUIRoot.SetActive(false);

            // Enabled Submenus.
            _mainEnabledUIRoot.SetActive(true);
            _logsUIRoot.SetActive(false);
        }
        public void DisableTerminal()
        {
            _enabledUIRoot.SetActive(false);
            _disabledUIRoot.SetActive(true);
        }
    }
}