using Entities.Player;
using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
	[SerializeField] private ForegroundSceneTransition _winTransition;


	private void OnEnable()
	{
		RepairSpotManager.OnAllRepairsCompleted += HandleWinCondition;
	}

	private void OnDisable()
	{
		RepairSpotManager.OnAllRepairsCompleted -= HandleWinCondition;
	}

	private void HandleWinCondition()
	{
		SceneLoader.Instance.PerformTransition(_winTransition);

		Vector3 spawnPosition = Vector3.zero;

		PlayerManager.Instance.SetPlayerPositionAndRotation(spawnPosition, 0f);
	}
}

