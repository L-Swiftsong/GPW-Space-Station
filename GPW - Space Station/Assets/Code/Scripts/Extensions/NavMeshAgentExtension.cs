using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtension
{
    #region Random Points

    /// <summary> Attempt to find a random point on the Navmesh that this agent can occupy within a box.</summary>
    public static bool TryFindRandomPoint(this NavMeshAgent agent, Vector3 centre, Vector3 extents, out Vector3 result, int maxAttempts = 5)
    {
        Vector3 randomPoint;
        NavMeshHit hit;
        for (int i = 0; i < maxAttempts; i++)
        {
            randomPoint = centre + new Vector3(
                x: Random.Range(-extents.x, extents.x),
                y: Random.Range(-extents.y, extents.y),
                z: Random.Range(-extents.z, extents.z));

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, agent.areaMask))
            {
                // We found a suitable point on the navmesh.
                result = hit.position;
                return true;
            }
        }

        // We couldn't find a suitable point on the navmesh.
        result = Vector3.zero;
        return false;
    }

    #endregion


    #region Sqr Distance Calculations

    /// <summary> Attempt to calculate the square distance to a point through the navmesh.</summary>
    public static bool TryCalculateSqrDistanceToPoint(this NavMeshAgent agent, Vector3 targetPoint, out float distanceToPoint, ref NavMeshPath pathToPoint)
    {
        // Set failure state return values.
        distanceToPoint = -1;

        // Attempt to find a valid path to the targetPoint.
        if (!NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, 1.0f, agent.areaMask))
        {
            // We failed to find a point on the navmesh near the targetPoint.
            return false;
        }
        if (!NavMesh.CalculatePath(agent.transform.position, hit.position, agent.areaMask, pathToPoint))
        {
            // We failed to calculate a path to the target position.
            return false;
        }
        if ((pathToPoint.corners[pathToPoint.corners.Length - 1] - hit.position).sqrMagnitude >= 0.5f)
        {
            // The path is invalid (It doesn't reach the target position).
            return false;
        }


        // We successfully found a valid path to the target position.
        distanceToPoint = agent.CalculatePathSqrLength(pathToPoint);
        return true;
    }
    /// <summary> Attempt to calculate the square distance to a point through the navmesh.</summary>
    public static bool TryCalculateSqrDistanceToPoint(this NavMeshAgent agent, Vector3 targetPoint, out float distanceToPoint)
    {
        NavMeshPath pathToPoint = new NavMeshPath();
        return agent.TryCalculateSqrDistanceToPoint(targetPoint, out distanceToPoint, ref pathToPoint);
    }


    public static bool IsPointWithinDistance(this NavMeshAgent agent, Vector3 targetPoint, float maxDistance) => agent.IsPointWithinDistance(targetPoint, maxDistance, out float pathSqrDistance);
    /// <summary> Check whether a given point is within a given radius when travelling along the NavMesh.</summary>
    /// <param name="pathSqrDistance"> An output of the sqrDistance of the path. -1 if the path is invalid.</param>
    public static bool IsPointWithinDistance(this NavMeshAgent agent, Vector3 targetPoint, float maxDistanceSquared, out float pathSqrDistance)
    {
        // Check the sqrDistance to the point in euclidean space.
        if ((agent.transform.position - targetPoint).sqrMagnitude > maxDistanceSquared)
        {
            // Point is outwith the distance in euclidean space, so will be outwith the distance when travelling along the NavMesh.
            pathSqrDistance = -1;
            return false;
        }

        // Check the sqrDistance to the point along the NavMesh.
        if (!agent.TryCalculateSqrDistanceToPoint(targetPoint, out pathSqrDistance))
        {
            // The point is outwith the max distance when travelling along the NavMesh.
            pathSqrDistance = -1;
            return false;
        }

        // The point is within the given distance.
        return true;
    }


    /// <summary> Calcylate the square length of the input path.</summary>
    public static float CalculatePathSqrLength(this NavMeshAgent agent, NavMeshPath path)
    {
        if (path == null || path.corners.Length == 0)
        {
            // Invalid path for calculation.
            return 0.0f;
        }

        float sqrDistance = 0.0f;
        Vector3 previousPosition = path.corners[0];
        for (int i = 1; i < path.corners.Length; i++)
        {
            sqrDistance += (path.corners[i] - previousPosition).sqrMagnitude;
            previousPosition = path.corners[i];
        }

        return sqrDistance;
    }

    #endregion
}
