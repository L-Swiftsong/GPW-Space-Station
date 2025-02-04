using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


// Source: 'https://www.youtube.com/watch?v=XEDi7fUCQos'.
public class AnimationEventStateBehaviour : StateMachineBehaviour
{
    public string EventName;
    [Range(0.0f, 1.0f)] public float TriggerTime;

    public bool CanTriggerAgainAfterLooping = false;

    private bool _hasTriggeredSinceEntry;
    private bool _hasTriggeredThisLoop;
    private int _loopIndex;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasTriggeredSinceEntry = false;
        _hasTriggeredThisLoop = false;
        _loopIndex = 0;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CanTriggerAgainAfterLooping)
        {
            // Check if we've entered a new loop.
            int currentLoopIndex = Mathf.FloorToInt(stateInfo.normalizedTime);
            if (currentLoopIndex != _loopIndex)
            {
                _hasTriggeredThisLoop = false;
                _loopIndex = currentLoopIndex;
            }
        }

        if (!ShouldRunUpdate())
        {
            return;
        }

        // Get progress within our current loop.
        float currentTime = stateInfo.normalizedTime % 1.0f;
        if (currentTime >= TriggerTime)
        {
            NotifyReceiver(animator);
            _hasTriggeredSinceEntry = true;
            _hasTriggeredThisLoop = true;
        }
    }


    private bool ShouldRunUpdate()
    {
        if (_hasTriggeredSinceEntry && !CanTriggerAgainAfterLooping)
        {
            // We've already triggered since entry, and don't wish to trigger on subsequent loops.
            return false;
        }
        if (_hasTriggeredThisLoop)
        {
            // We've already triggered this loop.
            return false;
        }
        
        return true;
    }

    private void NotifyReceiver(Animator animator)
    {
        AnimationEventReceiver receiver = animator.GetComponent<AnimationEventReceiver>();

        if (receiver != null)
        {
            receiver.OnAnimationEventTriggered(EventName);
        }
    }
}
