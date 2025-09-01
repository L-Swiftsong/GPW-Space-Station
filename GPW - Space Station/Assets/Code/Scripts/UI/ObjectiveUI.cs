using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : Singleton<ObjectiveUI>
{
	[SerializeField] private TextMeshProUGUI objectiveText;
	[SerializeField] private List<string> objectives = new List<string>();

	private int currentIndex = 0;


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


	public static void SetObjectiveIndex(int newValue)
	{
		Instance.currentIndex = newValue;
        Instance.objectiveText.text = Instance.objectives[Instance.currentIndex];
    }
    public static int GetObjectiveIndex() => HasInstance ? Instance.currentIndex : 0;
}
