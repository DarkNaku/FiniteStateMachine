using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkNaku.FSM {
    public interface IFiniteStateMachine<T>
    {
        IFiniteStateMachine<T> ParentFSM { get; }

        void ChangeState(T id);
    }

    public class FiniteStateMachine<T> : BaseState<T>, IFiniteStateMachine<T>
    {
        public class TransitionEvent : UnityEvent<T, T> 
        {
        }

        public override T State => default(T);

        public IFiniteStateMachine<T> ParentFSM => FSM;

        public TransitionEvent OnTransition { get; } = new TransitionEvent();

        public bool IsRootFSM => FSM == null;

        public BaseState<T> CurrentState {
            get
            {
                if (_states.TryGetValue(_current, out var state))
                {
                    return _states[_current];
                }
                else
                {
                    Debug.LogError($"[FiniteStateMachine] CurrentState : Does not contain state. {_current}");
                    return null;
                }
            }
        }

        private bool _initialized;
        private T _start;
        private T _current;
        private T _next;
        private Dictionary<T, BaseState<T>> _states = new Dictionary<T, BaseState<T>>();

        public void AddState(params BaseState<T>[] states)
        {
            if (states == null || states.Length == 0) return;

            for (int i = 0; i < states.Length; i++)
            {
                AddState(states[i]);
            }
        }

        public void AddState(BaseState<T> state)
        {
            if (state == null) return;

            state.FSM = this;
            state.Initialize();
            _states.Add(state.State, state);

            if (_states.Count == 1)
            {
                SetStartState(state.State);
            }
        }

        public void SetStartState(T state)
        {
            _start = state;
            _current = state;
            _next = state;
        }

        public void ChangeState(T state)
        {
            _next = state;
        }

        public sealed override void Initialize()
        {
            if (IsRootFSM) 
            {
                Enter();
            }

            OnInitialize();

            _initialized = true;
        }

        public sealed override void Enter()
        {
            _current = _start;
            _next = _current;

            CurrentState?.Enter();

            OnEnter();

            ChangeStateIfNecessary();
        }

        public sealed override void Update()
        {
            if (_initialized == false) 
            {
                Debug.LogError($"[FiniteStateMachine] Update : Not initialized. {GetType()}");
                return;
            }

            CurrentState?.Update();

            OnUpdate();

            ChangeStateIfNecessary();
        }

        public sealed override void Exit()
        {
            CurrentState?.Exit();

            OnExit();
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }

        private void ChangeStateIfNecessary()
        {
            if (_next.Equals(_current)) return;

            var prev = _current;

            CurrentState.Exit();

            _current = _next;

            CurrentState.Enter();

            OnTransition?.Invoke(prev, _next);

            ChangeStateIfNecessary();
        }
    }
}