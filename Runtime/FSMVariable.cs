using System.Collections.Generic;

namespace FSM {
    public class FSMVariable<T> {
        private T _value;
        public T Value {
            get => _value;
            set { 
                if (EqualityComparer<T>.Default.Equals(_value, value) == false) {
                    _value = value;
                }
            }
        }
    }
}
