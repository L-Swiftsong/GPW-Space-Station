using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private List<AnimationEvent> _animationEvents = new List<AnimationEvent>();

    public void OnAnimationEventTriggered(string eventName)
    {
        AnimationEvent matchingEvent = _animationEvents.Find(animationEvent => animationEvent.EventName == eventName);
        matchingEvent?.OnAnimationEvent?.Invoke();
    }
}
