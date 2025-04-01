using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Entities.Mimic;

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

namespace Entities.Mimic
{
    public class MimicController : BaseMimic
    {
        [SerializeField] private Animator animator;
        private static readonly int MOVEMENT_SPEED_HASH = Animator.StringToHash("MovementSpeed");

        [SerializeField] private List<WaypointData> waypoints = new List<WaypointData>();

        [Space(5)]
        [SerializeField] private BaseMimic _linkedMimic;
        [SerializeField] private bool _overridePositionAndRotation = true;

        private NavMeshAgent agent;
        private bool isMoving = false;

        private void Start()
        {
            gameObject.SetActive(false); // disabled by default until start movements called

            if (_linkedMimic != null)
            {
                _linkedMimic.Deactivate();
            }
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


            for (int i = 0; i < waypoints.Count; i++)
            {
                var waypointData = waypoints[i];
                if (agent == null) yield break;

                if (animator != null)
                {
                    animator.SetFloat(MOVEMENT_SPEED_HASH, waypointData.speed);
                }

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
                    if (animator != null)
                    {
                        animator.SetFloat(MOVEMENT_SPEED_HASH, 0.0f);
                    }
                    yield return new WaitForSeconds(waypointData.pauseDuration);
                }
            }

            // Replaces the agent with chosen mimic prefab in inspector or do nothing if None is selected
            if (_linkedMimic != null)
            {
                _linkedMimic.Activate();
                if (_overridePositionAndRotation)
                    _linkedMimic.SetPositionAndRotation(transform.position, transform.rotation);
            }


            if (animator != null)
            {
                animator.SetFloat(MOVEMENT_SPEED_HASH, 0.0f);
            }
            isMoving = false;
        }


        protected override Saving.LevelData.MimicSavableState GetSavableState() => Saving.LevelData.MimicSavableState.Idle;
    }
}