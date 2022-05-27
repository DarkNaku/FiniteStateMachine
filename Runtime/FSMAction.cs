using System;

namespace FSM {
    public abstract class FSMAction<E, S, M>
                                where E : struct, IConvertible, IComparable
                                where S : FSMState<E, S, M> 
                                where M : FiniteStateMachine<E, S, M> {
        protected M FSM => State.FSM;
        protected S State { get; private set; }

        public void Initialize(S state) {
            if (state == null) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Initialize : State can't be null.", GetType().ToString());
                return;
            }

            State = state;

            OnInitialize();

            FSM.LinkVariables(this);
        }

        public void ChangeState(E? id) {
            FSM.ChangeState(id);
        }

        public void CompleteInitialize() {
            OnCompleteInitialize();
        }

        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }

        protected virtual void OnInitialize() { }
        protected virtual void OnCompleteInitialize() { }
    }
}
