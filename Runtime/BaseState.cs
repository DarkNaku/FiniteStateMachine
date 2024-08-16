using UnityEngine;

namespace DarkNaku.FSM {
    public abstract class BaseState<T>
    {
        public abstract T State { get; }
        public IFiniteStateMachine<T> FSM { get; set; }

        public virtual void Initialize() 
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit() 
        { 
        }
    }
}