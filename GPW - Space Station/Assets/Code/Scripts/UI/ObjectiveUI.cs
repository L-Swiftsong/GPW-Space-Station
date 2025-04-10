using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ObjectiveUI : MonoBehaviour
{
	public static ObjectiveUI Instance { get; private set; }

	[SerializeField] private TextMeshProUGUI objectiveText;
	[SerializeField] private List<string> objectives = new List<string>();

	private int currentIndex = 0;

	private void Awake()
	{
		Instance = this;
	}

	public void SetNextObjectiveInstance()
	{
		if (currentIndex < objectives.Count)
		{
			objectiveText.text = objectives[currentIndex];
			currentIndex++;
		}
	}

	public static void SetNextObjective()
	{
		if (Instance != null)
		{
			Instance.SetNextObjectiveInstance();
		}
		else
		{
			Debug.LogWarning("ObjectiveSystem not initialized yet.");
		}
	}

	public static void AddObjective(string newObjective)
	{
		if (Instance != null)
		{
			Instance.objectives.Add(newObjective);
		}
		else
		{
			Debug.LogWarning("ObjectiveSystem not initialized yet.");
		}
	}
}
