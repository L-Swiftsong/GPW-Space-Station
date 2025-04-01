using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtension
{
    public static bool TryGetComponentThroughParents<T>(this Component child, out T component)
    {
        Transform targetTransform = child.transform;
        do
        {
            if (targetTransform.TryGetComponent<T>(out component))
                return true;

            targetTransform = targetTransform.parent;
        } while (targetTransform != null);

        return false;
    }
    public static IEnumerable<T> GetComponentsInParents<T>(this Component child)
    {
        Transform targetTransform = child.transform;
        do
        {
            foreach(T component in targetTransform.GetComponents<T>())
            {
                yield return component;
            }

            targetTransform = targetTransform.parent;
        } while(targetTransform != null);
    }
    public static bool TryFindFirstWithCondition<T>(this Component child, System.Func<T, bool> condition, out T foundInstance, int maxSearches = 3)
    {
        Transform targetTransform = child.transform;
        do
        {
            foreach (T component in targetTransform.GetComponents<T>())
            {
                if (condition(component))
                {
                    foundInstance = component;
                    return true;
                }
            }

            targetTransform = targetTransform.parent;
            --maxSearches;
        } while (targetTransform != null && maxSearches > 0);

        foundInstance = default(T);
        return false;
    }
}
