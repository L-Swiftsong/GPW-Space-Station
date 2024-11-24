using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public static class TrapManager
{
    private static List<TrapPoint> s_trapPoints = new List<TrapPoint>();
    

    public static void AddTrapPoint(TrapPoint trapPoint) => s_trapPoints.Add(trapPoint);
    public static void RemoveTrapPoint(TrapPoint trapPoint) => s_trapPoints.Remove(trapPoint);


    public static List<TrapPoint> GetTrapPointsWithinRange(Vector3 centre, float maxRange)
    {
        float sqrMaxRange = maxRange * maxRange;
        List<TrapPoint> trapPointsInRange = new List<TrapPoint>();
        for(int i = 0; i < s_trapPoints.Count; i++)
        {
            if ((s_trapPoints[i].transform.position - centre).sqrMagnitude <= sqrMaxRange)
            {
                trapPointsInRange.Add(s_trapPoints[i]);
            }
        }

        return trapPointsInRange;
    }
    public static List<TrapPoint> GetTrapPointsWithinNavMeshRange(NavMeshAgent agent, float maxRange)
    {
        float sqrMaxRange = maxRange * maxRange;
        List<TrapPoint> trapPointsInRange = new List<TrapPoint>();
        for (int i = 0; i < s_trapPoints.Count; i++)
        {
            if (agent.IsPointWithinDistance(s_trapPoints[i].transform.position, sqrMaxRange, out float pathSqrDistance))
            {
                // This trap point is within the max detection range.
                trapPointsInRange.Add(s_trapPoints[i]);
            }
        }

        return trapPointsInRange;
    }
}
