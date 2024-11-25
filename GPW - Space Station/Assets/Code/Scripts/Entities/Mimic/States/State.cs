using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Mimic.States
{
    public abstract class State : MonoBehaviour
    {
        public abstract string Name { get; }


        public virtual void OnEnter() { }
        public virtual void OnLogic() { }
        public virtual void OnExit() { }
    }
}