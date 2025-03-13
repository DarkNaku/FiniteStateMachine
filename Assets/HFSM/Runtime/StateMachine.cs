using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.HFSM {
    public class StateMachine<T> : BaseState<T>, IStateMachine<T> {
        public class TransitionEvent : UnityEvent<T, T> {
        }

        public override T State => _state;

        public T CurrentState => _current;

        public IStateMachine<T> ParentFSM => FSM;

        public TransitionEvent OnTransition { get; } = new TransitionEvent();

        public bool IsRootFSM => FSM == null;

        private BaseState<T> Current {
            get {
                if (_states.TryGetValue(_current, out var state)) {
                    return _states[_current];
                } else {
                    Debug.LogError($"[HFSM] Current : Does not contain state. {_current}");
                    return null;
                }
            }
        }

        private bool _initialized;
        private T _state;
        private T _start;
        private T _current;
        private T _next;
        private Dictionary<T, BaseState<T>> _states = new Dictionary<T, BaseState<T>>();

        public StateMachine() {
            _state = default;
        }

        public StateMachine(T state) {
            _state = state;
        }

        public void AddState(params BaseState<T>[] states) {
            if (states == null || states.Length == 0) return;

            foreach (var state in states) {
                state.FSM = this;
                state.Initialize();
                _states.Add(state.State, state);

                if (_states.Count == 1) {
                    SetStartState(state.State);
                }
            }
        }

        public void SetStartState(T state) {
            _start = state;
            _current = state;
            _next = state;
        }

        public void ChangeState(T state) {
            _next = state;
        }

        public sealed override void Initialize() {
            if (IsRootFSM) {
                Enter();
            }

            OnInitialize();

            _initialized = true;
        }

        public sealed override void Enter() {
            _current = _start;
            _next = _current;

            Current?.Enter();

            OnEnter();

            ChangeStateIfNecessary();
        }

        public sealed override void Update(float deltaTime) {
            if (_initialized == false) {
                Debug.LogError($"[HFSM] Update : Not initialized.");
                return;
            }

            Current?.Update(deltaTime);

            OnUpdate(deltaTime);

            ChangeStateIfNecessary();
        }

        public sealed override void Exit() {
            Current?.Exit();

            OnExit();
        }

        protected virtual void OnInitialize() {
        }

        protected virtual void OnEnter() {
        }

        protected virtual void OnUpdate(float deltaTime) {
        }

        protected virtual void OnExit() {
        }

        private void ChangeStateIfNecessary() {
            if (_next.Equals(_current)) return;

            var prev = _current;

            Current.Exit();

            _current = _next;

            Current.Enter();

            OnTransition?.Invoke(prev, _next);

            ChangeStateIfNecessary();
        }
    }

    public class StateMachine : StateMachine<string> {
    }
}