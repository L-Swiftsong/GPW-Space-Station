using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallOnDestroy : MonoBehaviour
{
    public UnityEvent OnObjectDestroyed;

    private void OnDestroy() => OnObjectDestroyed?.Invoke();
}
