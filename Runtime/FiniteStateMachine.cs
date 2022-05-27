using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FSM {
    public abstract class FiniteStateMachine<E, S, M> 
                                where E : struct, IConvertible, IComparable
                                where S : FSMState<E, S, M>
                                where M : FiniteStateMachine<E, S, M> {
        public class TransitionEvent : UnityEvent<E, E> {
        }

        public FSMState<E, S, M> CurrentState { get; protected set; }
        public TransitionEvent OnBeforeTransition { get; } = new TransitionEvent();
        public TransitionEvent OnAfterTransition { get; } = new TransitionEvent();

        public bool StateChanged {
            get {
                if (CurrentState == null) return false;

                return CurrentState.ID.CompareTo(_nextStateID) != 0;
            }
        }

        protected Dictionary<E, FSMState<E, S, M>> States { get; private set; } = new Dictionary<E, FSMState<E, S, M>>();

        private Dictionary<string, FSMVariable<int>> _intVariables = new Dictionary<string, FSMVariable<int>>();
        private Dictionary<string, FSMVariable<uint>> _uintVariables = new Dictionary<string, FSMVariable<uint>>();
        private Dictionary<string, FSMVariable<long>> _longVariables = new Dictionary<string, FSMVariable<long>>();
        private Dictionary<string, FSMVariable<ulong>> _ulongVariables = new Dictionary<string, FSMVariable<ulong>>();
        private Dictionary<string, FSMVariable<float>> _floatVariables = new Dictionary<string, FSMVariable<float>>();
        private Dictionary<string, FSMVariable<double>> _doubleVariables = new Dictionary<string, FSMVariable<double>>();
        private Dictionary<string, FSMVariable<string>> _stringVariables = new Dictionary<string, FSMVariable<string>>();
        private Dictionary<string, FSMVariable<Vector3>> _vector3Variables = new Dictionary<string, FSMVariable<Vector3>>();

        private E _nextStateID;

        public void Initialize(E id) {
            if (States.ContainsKey(id) == false) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Initialize : {1} is not on the list of states.", GetType().ToString(), id.ToString());
                return;
            }

            CurrentState = States[id];

            OnInitialize();

            LinkVariables(this);
            
            foreach (var state in States.Values) {
                state.Initialize(this as M);
            }

            CompleteInitialize();

            CurrentState.Enter();
        }

        public void Update() {
            if (CurrentState == null) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Update : Is not initialized.", GetType().ToString());
                return;
            }

            _nextStateID = CurrentState.ID;

            OnBeforeUpdate();

            CurrentState.Update();

            OnAfterUpdate();

            Translate();
        }

        public void Add(params FSMState<E, S, M>[] states) {
            if (states == null) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Add : States can't be null.", GetType().ToString());
                return;
            }

            for (int i = 0; i < states.Length; i++) {
                States.Add(states[i].ID, states[i]);
            }
        }

        public void ChangeState(E? id) {
            if (id == null) return;
            _nextStateID = id.Value;
        }

        public void LinkVariables(object obj) {
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields) {
                var type = field.FieldType;

                if (type.IsGenericType == false) continue;

                if (type.Name == typeof(FSMField<>).Name) {
                    var fieldValue = field.GetValue(obj);
                    var genericType = type.GetGenericArguments()[0];

                    if (genericType == typeof(int)) {
                        field.SetValue(obj, LinkVariable((FSMField<int>)fieldValue));
                    } else if (genericType == typeof(uint)) {
                        field.SetValue(obj, LinkVariable((FSMField<uint>)fieldValue));
                    } else if (genericType == typeof(long)) {
                        field.SetValue(obj, LinkVariable((FSMField<long>)fieldValue));
                    } else if (genericType == typeof(ulong)) {
                        field.SetValue(obj, LinkVariable((FSMField<ulong>)fieldValue));
                    } else if (genericType == typeof(float)) {
                        field.SetValue(obj, LinkVariable((FSMField<float>)fieldValue));
                    } else if (genericType == typeof(double)) {
                        field.SetValue(obj, LinkVariable((FSMField<double>)fieldValue));
                    } else if (genericType == typeof(string)) {
                        field.SetValue(obj, LinkVariable((FSMField<string>)fieldValue));
                    } else if (genericType == typeof(Vector3)) {
                        field.SetValue(obj, LinkVariable((FSMField<Vector3>)fieldValue));
                    } else {
                        UnityEngine.Debug.LogErrorFormat("Unhandle Type : {0} - {1}", type.Name, field.Name);
                    }
                }
            }
        }

        private FSMField<T> LinkVariable<T>(FSMField<T> field) {
            if (string.IsNullOrEmpty(field.Key)) return field;

            var variable = GetVariableRef<T>(field.Key);

            if (variable == null) {
                variable = new FSMVariable<T>();
                SetVariableRef(field.Key, variable);
            }

            field.Variable = variable;

            return field;
        }

        private FSMVariable<T> GetVariableRef<T>(string key) {
            if (typeof(T) == typeof(int)) {
                if (_intVariables.ContainsKey(key)) {
                    return _intVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(uint)) {
                if (_uintVariables.ContainsKey(key)) {
                    return _uintVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(long)) {
                if (_longVariables.ContainsKey(key)) {
                    return _longVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(ulong)) {
                if (_ulongVariables.ContainsKey(key)) {
                    return _ulongVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(float)) {
                if (_floatVariables.ContainsKey(key)) {
                    return _floatVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(double)) {
                if (_doubleVariables.ContainsKey(key)) {
                    return _doubleVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(string)) {
                if (_stringVariables.ContainsKey(key)) {
                    return _stringVariables[key] as FSMVariable<T>;
                } 
            } else if (typeof(T) == typeof(Vector3)) {
                if (_vector3Variables.ContainsKey(key)) {
                    return _vector3Variables[key] as FSMVariable<T>;
                } 
            }

            return null;
        }

        private void SetVariableRef<T>(string key, FSMVariable<T> variable) {
            if (typeof(T) == typeof(int)) {
                if (_intVariables.ContainsKey(key)) return;
                _intVariables[key] = variable as FSMVariable<int>;
            } else if (typeof(T) == typeof(uint)) {
                if (_uintVariables.ContainsKey(key)) return;
                _uintVariables[key] = variable as FSMVariable<uint>;
            } else if (typeof(T) == typeof(long)) {
                if (_longVariables.ContainsKey(key)) return;
                _longVariables[key] = variable as FSMVariable<long>;
            } else if (typeof(T) == typeof(ulong)) {
                if (_ulongVariables.ContainsKey(key)) return;
                _ulongVariables[key] = variable as FSMVariable<ulong>;
            } else if (typeof(T) == typeof(float)) {
                if (_floatVariables.ContainsKey(key)) return;
                _floatVariables[key] = variable as FSMVariable<float>;
            } else if (typeof(T) == typeof(double)) {
                if (_doubleVariables.ContainsKey(key)) return;
                _doubleVariables[key] = variable as FSMVariable<double>;
            } else if (typeof(T) == typeof(string)) {
                if (_stringVariables.ContainsKey(key)) return;
                _stringVariables[key] = variable as FSMVariable<string>;
            } else if (typeof(T) == typeof(Vector3)) {
                if (_vector3Variables.ContainsKey(key)) return;
                _vector3Variables[key] = variable as FSMVariable<Vector3>;
            }
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnInitializeComplete() { }
        protected virtual void OnBeforeUpdate() { }
        protected virtual void OnAfterUpdate() { }

        private void Translate() {
            if (CurrentState == null) return;
            if (CurrentState.ID.CompareTo(_nextStateID) == 0) return;

            if (States.ContainsKey(_nextStateID) == false) {
                UnityEngine.Debug.LogErrorFormat("[{0}] Translate : {1} is not on the list of states.", GetType().ToString(), _nextStateID.ToString());
                return;
            }

            var prev = CurrentState.ID;

            OnBeforeTransition?.Invoke(prev, _nextStateID);

            CurrentState.Exit();

            CurrentState = States[_nextStateID];

            CurrentState.Enter();

            OnAfterTransition?.Invoke(prev, _nextStateID);
        }

        private void CompleteInitialize() {
            OnInitializeComplete();

            foreach (var state in States.Values) {
                state.CompleteInitialize();
            }
        }
    }
}
