using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkNaku.FSM;

public class HFSMTest: MonoBehaviour
{
    public enum STATE { NONE, IDLE, REST, EAT, SLEEP, MOVE, ATTACK, DEAD }

    private FiniteStateMachine<STATE> _fsm;
    
    private void Start()
    {
        _fsm = new FiniteStateMachine<STATE>();
        var idleFSM = new FiniteStateMachine<STATE>(STATE.IDLE);
        idleFSM.AddState(new RestState(), new EatState(), new SleepState());
        _fsm.AddState(idleFSM, new MoveState(), new AttackState(), new DeadState());
        _fsm.SetStartState(STATE.IDLE);
        _fsm.Initialize();
    }

    private void Update()
    {
        _fsm.Update();
    }
    
    public class RestState : BaseState<STATE>
    {
        public override STATE State => STATE.REST;

        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ChangeState(STATE.EAT);
            }
        }
    }
    
    public class EatState : BaseState<STATE>
    {
        public override STATE State => STATE.EAT;
        
        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ChangeState(STATE.SLEEP);
            }
        }
    }
    
    public class SleepState : BaseState<STATE>
    {
        public override STATE State => STATE.SLEEP;
        
        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ParentFSM.ChangeState(STATE.MOVE);
            }
        }
    }
    
    public class MoveState : BaseState<STATE>
    {
        public override STATE State => STATE.MOVE;
        
        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ChangeState(STATE.ATTACK);
            }
        }
    }
    
    public class AttackState : BaseState<STATE>
    {
        public override STATE State => STATE.ATTACK;
        
        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ChangeState(STATE.DEAD);
            }
        }
    }
    
    public class DeadState : BaseState<STATE>
    {
        public override STATE State => STATE.DEAD;
        
        private float _enterTime;

        public override void Enter()
        {
            Debug.Log($"Enter : {GetType()}");
            _enterTime = Time.time;
        }

        public override void Update()
        {
            if (Time.time - _enterTime > 1f)
            {
                FSM.ChangeState(STATE.IDLE);
            }
        }
    }
}
