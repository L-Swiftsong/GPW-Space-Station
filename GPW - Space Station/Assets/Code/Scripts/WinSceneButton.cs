using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinSceneButton : MonoBehaviour
{
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private Button _mainMenuButton;
	[SerializeField] private LayerMask _uiLayer;

	[SerializeField] private float _interactionDistance = 3f;

	private void Awake()
	{
		_playerCamera = GetComponent<Camera>();
	}

	private void Update()
	{
		RaycastForButtonInteraction();
	}

	private void RaycastForButtonInteraction()
	{
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _interactionDistance, _uiLayer))
		{
			if (hit.collider.gameObject == _mainMenuButton.gameObject)
			{
				if (Input.GetMouseButtonDown(0))
				{
					_mainMenuButton.onClick.Invoke();
				}
			}
		}
	}

	public void OnBackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
