using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class TargetableObject : MonoBehaviour
    {
        [SerializeField] private List<Transform> _detectionTargets = new List<Transform>();
        public List<Transform> DetectionTargets => _detectionTargets;


        private PlayerController _playerController; // Temp.
        public bool IsHidden => _playerController.GetHiding();


        private void Awake() => _playerController = GetComponent<PlayerController>();
    }
}