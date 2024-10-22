using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPW.Tests.Camouflage;


namespace Testing.Mimicry
{
    public class MimicryTest : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        private ActiveCamoController _camoController;


        [SerializeField] private float _minCamoDistance;
        [SerializeField] private float _maxCamoDistance;
        [SerializeField] private AnimationCurve _camoRampCurve;


        private void Awake() => _camoController = GetComponent<ActiveCamoController>();

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            float percentageDistance = Mathf.Clamp01((distanceToPlayer - _minCamoDistance) / (_maxCamoDistance - _minCamoDistance));
            float camoPercentage = _camoRampCurve.Evaluate(percentageDistance);
            _camoController.SetActiveCamoRamp(camoPercentage);
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _minCamoDistance);
            Gizmos.DrawWireSphere(transform.position, _maxCamoDistance);
        }
    }
}