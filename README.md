<img width="459" height="255" alt="Image" src="https://github.com/user-attachments/assets/361a93ba-6d16-48f1-9e01-7e7035110467" />

## 🎮 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 2D 소울라이크 액션 RPG |
| 엔진 | Unity (C#) |
| 개발 기간 | 2025.08 ~ 2026.01 |
| 개발 인원 | 1인 |
| 핵심 목표 | 소울라이크 특유의 전투 시스템 구현 (패링, 스태미나, 보스 2페이즈 등) |

---

## 🔧 핵심 기술 및 구현 내용

### 1. FSM(유한 상태 머신) 기반 플레이어 컨트롤러
`PlayerStateMachine` + `PlayerState` 베이스 클래스를 설계하고, 20개 이상의 독립 상태 클래스로 행동을 분리했습니다. 각 상태는 `Enter`, `LogicUpdate`, `PhysicsUpdate`, `Exit`의 생명주기를 가지며 상태 전이 시 `PreviousState`를 저장해 직전 상태를 추적할 수 있도록 구현했습니다.

- **구현 상태**: Idle / Move / Run / Jump / Dash / DashAttack / Attack (콤보) / AirAttack / SpecialAttack / Throw / Defend / Parry / Heal / Hit / Death / WallSlide / WallJump / Climb / Crouch

### 2. 보스 AI FSM 및 2페이즈 각성 시스템
`EnemyStateMachine`을 통한 보스 전용 AI를 구현했습니다. 체력이 50% 이하로 떨어지면 `EnrageState`로 전환되는 2페이즈 각성 시스템을 포함하며, 각성 애니메이션은 1회만 재생되도록 처리했습니다.

- **구현 상태**: Idle / Chase / Rush / MeleeAttack / RangedAttack / BackAway / Parry / Enrage / Hit / Death
- **행동 딜레이**: `minActionDelay ~ maxActionDelay` 사이의 랜덤 딜레이로 패턴 다양화
- **플레이어 상태 감지**: `IsPlayerHealing()`, `IsPlayerDefending()`, `IsPlayerAttacking()` 등 플레이어 상태를 실시간으로 확인해 반응형 AI 구현

### 3. ScriptableObject 기반 스탯 관리
`PlayerStatsSO`를 활용해 플레이어 수치(체력, 스태미나, 이동속도, 공격력, 대시, 패링 등)를 코드와 분리하여 인스펙터에서 직접 조정 가능하도록 설계했습니다.

### 4. 소울라이크 핵심 전투 시스템
- **패링**: 방어 입력 초반 `parryWindow`(0.15초) 내 적 공격 감지 시 패링 성립, 시각 이펙트 출력
- **스태미나**: 달리기·점프·대시·공격 등 모든 행동에 스태미나 소비 적용, 딜레이 후 자동 회복
- **콤보 공격**: `AttackData` 배열로 다단 콤보 구성, `comboResetTime` 내 연속 입력 시 다음 공격으로 진행
- **다양한 공격 액션**: 지상 공격, 공중 공격, 대시 공격, 특수 공격, 투척 공격

### 5. 사운드 매니저 (싱글턴)
`SoundManager`와 `SoundType` 열거형으로 BGM·SFX를 중앙 관리하며, 랜덤 재생 기능을 지원합니다.

### 6. 대화 시스템 및 UI
`DialogueSystem` + `DialogueData(ScriptableObject)` + `SpeechBubble` 프리팹 조합으로 인게임 대화 시스템을 구현했습니다. 플레이어/적 각각 말풍선 프리팹을 분리하여 사용합니다.
- PlayerHUD (체력·스태미나·힐 차지·특수 공격 차지 표시)
- BossHUD (보스 체력바)
- GameOverUI (사망 메시지 랜덤화, 리트라이 기능)
- DangerUIController (위험 상황 UI 연출)

### 7. 카메라 연출
공격 히트 시 `cameraShakeOnHitIntensity / Duration` 값 기반의 카메라 셰이크 효과를 적용해 타격감을 강화했습니다.

---


## 🎯 주요 구현 포인트 

- 상태 기반 설계로 로직 간의 **결합도를 낮추고 확장성**을 확보한 FSM 아키텍처
- ScriptableObject 활용으로 **데이터와 로직의 분리**, 인스펙터 기반 밸런싱 가능
- 보스 AI가 플레이어의 현재 상태(힐링 중, 방어 중, 공격 중 등)를 읽고 **반응형으로 행동**하는 AI 설계
- **체력 50% 이하 2페이즈 각성** + 각성 연출 1회 보장 처리
- 소울라이크 장르 핵심인 **패링·스태미나·콤보** 시스템의 완성도 있는 구현
