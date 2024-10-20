using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPW.Tests.AI.Stalking
{
    public class HidingSpotManager : MonoBehaviour
    {
        private static HidingSpotManager _instance;
        public static HidingSpotManager Instance
        {
            get { return _instance; }
            set 
            {
                if (_instance != null)
                {
                    Debug.LogError(string.Format("Error: A HidingSpotManager instance already exists in the scene ({0}). Deleting {1}.", _instance.name, value.name));
                    Destroy(value.gameObject);
                    return;
                }

                _instance = value;
            }
        }

        
        [Header("References")]
        [SerializeField] private Transform _player;
        [SerializeField] private float _closeToPlayerMaxDistance = 10.0f;
        [Space(5)]


        [SerializeField] private List<HidingSpot> _hidingSpots = new List<HidingSpot>();


        private void Awake() => Instance = this;
        private void Update() => UpdateHidingSpots();
        

        private void UpdateHidingSpots()
        {
            foreach (HidingSpot hidingSpot in _hidingSpots)
            {
                hidingSpot.UpdateHidingSpot(_player.position, _closeToPlayerMaxDistance);
            }
        }

        public bool TryGetAvailableHidingSpot(out HidingSpot selectedHidingSpot)
        {
            // Get all available hiding spots.
            IEnumerable<HidingSpot> availableHidingSpots = _hidingSpots.Where(spot => spot.IsValid);


            if (!availableHidingSpots.Any())
            {
                // There are no available hiding spots.
                selectedHidingSpot = null;
                return false;
                
            }

            // There is an available hiding spot.

            // Order the available hiding spots by ascending sqrDistance to the player (0 is closest).
            availableHidingSpots = availableHidingSpots.OrderBy(spot => spot.SqrDistanceToPlayer);

            selectedHidingSpot = availableHidingSpots.First();
            return true;
        }


        private void OnDrawGizmosSelected()
        {
            foreach (HidingSpot hidingSpot in _hidingSpots)
            {
                hidingSpot.DrawGizmos();
            }
        }
    }
}