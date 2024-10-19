using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GPW.Tests.Mimicry
{
    public class MimicableObject : MonoBehaviour
    {
        [SerializeField] private Transform _graphicsParent;
        public Transform GetGraphicsParent() => _graphicsParent;
    }
}