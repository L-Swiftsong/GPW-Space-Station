using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaypointData
{
    public Transform waypoint; // The position the agent moves to
    public bool shouldPause; // Should the agent pause here?
    public float pauseDuration = 1f; // How long to pause if enabled
    public float speed = 3f; // Speed for this waypoint
}

public class MimicController : MonoBehaviour
{
    [SerializeField] private List<WaypointData> waypoints = new List<WaypointData>(); // Waypoints list

    // Enum for selecting the mimic type
    public enum MimicType { GeneralMimic, ChaseMimic, None }
    [SerializeField] private MimicType mimicType; // Option to choose the mimic type in the Inspector

    [SerializeField] private GameObject generalMimicPrefab; // Prefab for GeneralMimic
    [SerializeField] private GameObject chaseMimicPrefab; // Prefab for ChaseMimic

    private NavMeshAgent agent;
    private bool isMoving = false;

    private void Start()
    {
        gameObject.SetActive(false); // Start with this object disabled
    }

    /// <summary>
    /// Starts the movement sequence and activates the GameObject.
    /// </summary>
    public void StartMovement()
    {
        if (isMoving || waypoints.Count == 0) return;

        gameObject.SetActive(true); // Activate the GameObject
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            StartCoroutine(HandleMovement());
        }
    }

    private IEnumerator HandleMovement()
    {
        isMoving = true;

        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypointData = waypoints[i];
            if (agent == null) yield break; // Stop if the agent is missing

            agent.speed = waypointData.speed; // Set speed for this waypoint
            agent.SetDestination(waypointData.waypoint.position);

            // Wait until the agent reaches the target
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

            if (waypointData.shouldPause)
            {
                yield return new WaitForSeconds(waypointData.pauseDuration);
            }
        }

        // Replace the agent with the correct mimic prefab or do nothing if None is selected
        if (mimicType != MimicType.None)
        {
            ReplaceWithMimicPrefab();
        }

        isMoving = false;
    }

    /// <summary>
    /// Replaces the current agent with a new prefab based on the selected mimic type.
    /// </summary>
    private void ReplaceWithMimicPrefab()
    {
        if (agent != null)
        {
            Vector3 position = agent.transform.position; // Get current position
            Destroy(agent.gameObject); // Destroy the current agent

            // Instantiate the selected prefab at the agent's position
            GameObject newMimic = null;
            switch (mimicType)
            {
                case MimicType.GeneralMimic:
                    newMimic = Instantiate(generalMimicPrefab, position, Quaternion.identity);
                    break;
                case MimicType.ChaseMimic:
                    newMimic = Instantiate(chaseMimicPrefab, position, Quaternion.identity);
                    break;
            }

            if (newMimic != null)
            {
                Debug.Log("Replaced with " + mimicType.ToString() + " prefab.");
            }
        }
    }
}









