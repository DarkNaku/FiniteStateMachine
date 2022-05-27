using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FSM {
    public abstract class FSMState<E, S, M>
                                where E : struct, IConvertible, IComparable
                                where S : FSMState<E, S, M> 
                                where M : FiniteStateMachine<E, S, M> {
        public abstract E ID { get; }

        public M FSM { get; private set; }

        protected ReadOnlyCollection<FSMAction<E, S, M>> Actions => _actions.AsReadOnly();

        private List<FSMAction<E, S, M>> _actions = new List<FSMAction<E, S, M>>();

        public void Initialize(M machine) {
            if (machine == null) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Initialize : Machine can't be null.", GetType().ToString());
                return;
            }

            FSM = machine;

            OnInitialize();

            FSM.LinkVariables(this);

            for (int i = 0; i < _actions.Count; i++) {
                _actions[i].Initialize(this as S);
            }
        }

        public void Enter() {
            OnEnter();

            for (int i = 0; i < _actions.Count; i++) {
                _actions[i].OnEnter();
            }
        }

        public void Update() {
            OnUpdate();

            for (int i = 0; i < _actions.Count; i++) {
                _actions[i].OnUpdate();

                if (FSM.StateChanged) break;
            }
        }

        public void Exit() {
            OnExit();

            for (int i = 0; i < _actions.Count; i++) {
                _actions[i].OnExit();
            }
        }

        public void Add(params FSMAction<E, S, M>[] actions) {
            if (actions == null) {
                UnityEngine.Debug.LogFormat("[{0}] Add : Actions can't be null.", GetType().ToString());
                return;
            }

            for (int i = 0; i < actions.Length; i++) {
                _actions.Add(actions[i]);
            }
        }

        public void ChangeState(E id) {
            FSM.ChangeState(id);
        }

        public void CompleteInitialize() {
            OnCompleteInitialize();

            for (int i = 0; i < Actions.Count; i++) {
                Actions[i].CompleteInitialize();
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnCompleteInitialize() { }
        protected virtual void OnEnter() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnExit() { }
    }
}
