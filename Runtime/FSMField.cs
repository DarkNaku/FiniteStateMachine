namespace FSM {
    public struct FSMField<T> {
        public string Key { get; set; }
        public FSMVariable<T> Variable { get; set; }

        public T Value {
            get {
                if (Variable == null) {
                    return default(T);
                } else {
                    return Variable.Value;
                }
            }
            set {
                if (Variable != null) {
                    Variable.Value = value;
                }
            }
        }
    }
}
