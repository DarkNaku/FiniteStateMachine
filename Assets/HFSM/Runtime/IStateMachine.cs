using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.HFSM {
    public interface IStateMachine<T> {
        T State { get; }
        IStateMachine<T> ParentFSM { get; }

        void ChangeState(T id);
    }
}