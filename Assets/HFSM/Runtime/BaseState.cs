using UnityEngine;

namespace DarkNaku.HFSM {
    public abstract class BaseState<T> {
        public abstract T State { get; }
        public IStateMachine<T> FSM { get; set; }

        public virtual void Initialize() {
        }

        public virtual void Enter() {
        }

        public virtual void Update(float deltaTime) {
        }

        public virtual void Exit() {
        }
    }
}