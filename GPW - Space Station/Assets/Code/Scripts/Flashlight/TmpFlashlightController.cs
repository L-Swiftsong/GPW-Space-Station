using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpFlashlightController : MonoBehaviour
{
    [SerializeField] private Transform _flashlightHolder;
    public Transform FlashlightHolder => _flashlightHolder;
}
