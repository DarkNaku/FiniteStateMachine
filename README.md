# HFSM (Hierarchical Finite State Machine)

### 소개
단순한 계층 유한 상태 머신(HFSM : Hierarchical Finite State Machine) 라이브러리 입니다. 간단한 HFSM이 필요했으나 보통은 전환(Transition)을 통한 상태 변환을 하는 라이브러리 밖에 찾을 수가 없어서 제가 원하는 형태로 만들어 보았습니다.

### 설치방법
1. 패키지 관리자의 툴바에서 좌측 상단에 플러스 메뉴를 클릭합니다.
2. 추가 메뉴에서 Add package from git URL을 선택하면 텍스트 상자와 Add 버튼이 나타납니다.
3. https://github.com/DarkNaku/HFSM.git?path=/Assets/HFSM 입력하고 Add를 클릭합니다.

### 사용
    
1. 상태를 정의할 타입을 선언합니다. (문자열을 사용해도 됩니다.)
```csharp
public enum STATE { NONE, IDLE, REST, EAT, SLEEP, MOVE, ATTACK, DEAD }
```

2. 상태 클래스와 서브 머신 클래스를 구현해 줍니다. (나머지 코드는 샘플을 확인하세요.)
```csharp
    // 상태가 전환되고 1초 후에 서브 머신의 EAT 상태로 전환 됩니다.
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
                return;
            }
        }
    }
    .
    .
    .
```

3. FSM을 생성하고 상태를 추가합니다. (서브 스테이트 머신도 동일한 타이밍에 생성하여 추가합니다.)

```csharp
using DarkNaku.FSM;

private FiniteStateMachine<STATE> _fsm;

private void Start()
{
    _fsm = new FiniteStateMachine<STATE>();
    var idleFSM = new FiniteStateMachine<STATE>(STATE.IDLE); // 서브 스테이트 머신 선언
    idleFSM.AddState(new RestState(), new EatState(), new SleepState());
    _fsm.AddState(idleFSM, new MoveState(), new AttackState(), new DeadState()); // 서브 스테이트 머신은 다른 상태와 동일하게 추가
    _fsm.SetStartState(STATE.IDLE);
    _fsm.Initialize();
}
```

4. 업데이트 주기마다 호출 FSM 갱신을 호출해 줍니다.
```csharp
private void Update()
{
    _fsm.Update();
}
```

### 부가설명
* Transition은 지원하지 않습니다.
* Enter 이벤트 내부에서 ChangeState를 호출하여 프레임 손실 없이 상태를 전환 할 수 있으나 무한루프에 빠질 수 있으니 주의 하세요.(추후에 방지 코드를 넣을 예정)
* 서브 스테이스에서는 ParentFSM 속성을 통해 상위 머신에 접근할 수 있습니다.