using Entities.Player;
using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
	[SerializeField] private SceneField winSceneField;

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
		if (winSceneField == null)
		{
			Debug.LogError("Win Scene is not assigned!");
			return;
		}

		ForegroundSceneTransition winTransition = ScriptableObject.CreateInstance<ForegroundSceneTransition>();

		winTransition.ScenesToLoad = new SceneField[] { winSceneField };
		winTransition.ActiveSceneIndex = 0;

		SceneLoader.Instance.PerformTransition(winTransition);

		Vector3 spawnPosition = Vector3.zero;

		PlayerManager.Instance.SetPlayerPositionAndRotation(spawnPosition, 0f);
	}
}

