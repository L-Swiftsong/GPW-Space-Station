using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chase
{
    [RequireComponent(typeof(Collider))]
    public class ChaseEndTrigger : MonoBehaviour
    {
        [System.Serializable] private enum EndType { PlayerEnter, MimicEnter, AnyEnter }
        [SerializeField] private EndType _chaseEndType;

        public static event System.Action OnChaseEndTriggerPassed;

        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Set this object's colliders to be trigger colliders.
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.isTrigger = true;
            }
        }
#endif


        private void OnTriggerEnter(Collider other)
        {
            TryEndChase(other.gameObject);
        }

        private void TryEndChase(GameObject objectToTest)
        {
            if (_chaseEndType == EndType.AnyEnter)
            {
                if (!(TestIsPlayer(objectToTest) || TestIsMimic(objectToTest)))
                {
                    // The entered entity was neither the player nor were they the mimic.
                    return;
                }
            }
            if (_chaseEndType == EndType.PlayerEnter)
            {
                if (!TestIsPlayer(objectToTest))
                {
                    // We are expecting a player to enter and what entered wasn't a player.
                    return;
                }
            }
            else if (_chaseEndType == EndType.MimicEnter)
            {
                if (!TestIsMimic(objectToTest))
                {
                    // We are expecting the mimic to enter and what entered wasn't the mimic.
                    return;
                }
            }

            // What entered our trigger was our desired entity.
            OnChaseEndTriggerPassed?.Invoke();
        }
        private bool TestIsPlayer(GameObject objectToTest) => objectToTest.GetComponent<PlayerController>() != null;
        private bool TestIsMimic(GameObject objectToTest) => objectToTest.GetComponent<AI.Mimic.MimicController>() != null || objectToTest.GetComponent<EnemyChaser>() != null;
    }
}