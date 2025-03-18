using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class WaypointData
{
    public Transform waypoint; // position the agent moves to
    public bool shouldPause; // should the mimic pause after reaching a waypoint
    public float pauseDuration = 1f; // how long the mimic should pause for
    public float speed = 3f; // speed the mimic will travel to a waypoint

    [Space(5)]
    public ShowLogType LogOnPartialPath = ShowLogType.Warning;

    [System.Serializable] public enum ShowLogType { None, Warning, Error }
}

public class MimicController : MonoBehaviour
{
    Animator animator;
    [SerializeField] private List<WaypointData> waypoints = new List<WaypointData>();

    // Enum for what type the mimic should be at end of route
    public enum MimicType { GeneralMimic, ChaseMimic, None }
    [SerializeField] private MimicType mimicType;

    [SerializeField] private GameObject generalMimicPrefab;
    [SerializeField] private GameObject chaseMimicPrefab;

    private NavMeshAgent agent;
    private bool isMoving = false;

    private void Start()
    {
        gameObject.SetActive(false); // disabled by default until start movements called
        animator = GetComponent<Animator>();
    }

    // Starts the movement sequence and activates the GameObject
    public void StartMovement()
    {
        if (isMoving || waypoints.Count == 0) return;

        gameObject.SetActive(true);
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            StartCoroutine(HandleMovement());
           
        }
    }

    private IEnumerator HandleMovement()
    {
        isMoving = true;
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            Debug.Log("Walking animation started");
        }


        for (int i = 0; i < waypoints.Count; i++)
        {
            var waypointData = waypoints[i];
            if (agent == null) yield break;
           
            agent.speed = waypointData.speed; // set speed for current waypoint
            agent.SetDestination(waypointData.waypoint.position);

            // wait until the agent reaches the waypoint
            yield return new WaitUntil(() => !agent.pathPending);

            if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                // We received a partial or invalid path.
                // Notify that the path was impartial (E.g. This was unexpected).
                switch (waypointData.LogOnPartialPath)
                {
                    case WaypointData.ShowLogType.Warning:
                        Debug.LogWarning($"Chase Mimic Path from {agent.transform.position} to {waypointData.waypoint.position} was partial or invalid.");
                        break;
                    case WaypointData.ShowLogType.Error:
                        Debug.LogError($"Chase Mimic Path from {agent.transform.position} to {waypointData.waypoint.position} was partial or invalid.");
                        break;
                }

                
                float sqrStoppingDistance = agent.stoppingDistance * agent.stoppingDistance;
                while ((agent.transform.position - waypointData.waypoint.position).sqrMagnitude > sqrStoppingDistance)
                {
                    agent.transform.position = Vector3.MoveTowards(agent.transform.position, waypointData.waypoint.position, agent.speed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                // We successfully received a path.
                yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            }

            if (waypointData.shouldPause)
            {
                animator.SetBool("IsWalking", false);
                yield return new WaitForSeconds(waypointData.pauseDuration);
                animator.SetBool("IsWalking", true);
            }
        }

        // Replaces the agent with chosen mimic prefab in inspector or do nothing if None is selected
        if (mimicType != MimicType.None)
        {
            ReplaceWithMimicPrefab();
        }

        animator.SetBool("IsWalking", false);
        isMoving = false;
    }


    // Replaces the event Mimic with a prefab based on the selected mimic type
    private void ReplaceWithMimicPrefab()
    {
        if (agent != null)
        {
            Vector3 position = agent.transform.position;
            Destroy(agent.gameObject); // Destroy the current mimic

            // Instantiate the selected mimic prefab at the agent's position
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
        }
    }

   
}









