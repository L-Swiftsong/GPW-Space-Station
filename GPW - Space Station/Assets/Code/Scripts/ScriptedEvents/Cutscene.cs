using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;
using System.Linq;

namespace ScriptedEvents
{
    public class Cutscene : MonoBehaviour
    {
        [SerializeField] private CutsceneComponent[] _cutsceneComponents = new CutsceneComponent[0];
        private bool _hasTriggered;


        public void Trigger()
        {
            if (_hasTriggered)
                return;
            _hasTriggered = true;

            StartCoroutine(RunCutscene());
        }
        private IEnumerator RunCutscene()
        {
            float time = 0;
            int index = 0;
            while(index < _cutsceneComponents.Length)
            {
                if (time >= _cutsceneComponents[index].TriggerTime)
                {
                    _cutsceneComponents[index].OnTriggered?.Invoke();
                    index += 1;
                }

                yield return null;
                time += Time.deltaTime;
            }
        }


        #if UNITY_EDITOR

        [ContextMenu("Organise Cutscene Components")]
        private void OrganiseCutsceneComponents() => _cutsceneComponents = _cutsceneComponents.OrderBy(t => t.TriggerTime).ToArray();

        #endif
    }


    [System.Serializable]
    public class CutsceneComponent
    {
        public float TriggerTime;
        public UltEvent OnTriggered;
    }
}