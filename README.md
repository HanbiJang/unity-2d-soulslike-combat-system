# 2D 소울라이크 전투 시스템

**FSM 기반 플레이어 컨트롤러와 반응형 보스 AI를 직접 설계한 2D 소울라이크 전투 시스템 구현 프로젝트**

[![Unity](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)

[![게임플레이 영상](https://img.youtube.com/vi/mXKTkPdiSQg/0.jpg)](https://youtu.be/mXKTkPdiSQg?si=bHDAR7blZi7zrtal)

> 이미지를 클릭하면 게임플레이 영상을 볼 수 있습니다.
[![YouTube](https://img.shields.io/badge/YouTube-FF0000?style=flat&logo=youtube&logoColor=white)](https://youtu.be/mXKTkPdiSQg?si=bHDAR7blZi7zrtal)

---

## 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 장르 | 2D 소울라이크 액션 RPG |
| 개발 기간 | 2025.08 ~ 2026.01 |
| 팀 구성 | 1인 개발 |
| 사용 기술 | Unity (C#), FSM, ScriptableObject |

**본인 담당 파트:** 전체

---

## 프로젝트 개요

소울라이크 장르 특유의 전투 시스템(패링, 스태미나, 보스 2페이즈 각성 등)을 직접 설계하고 구현한 프로젝트입니다.

플레이어와 보스 모두 FSM(유한 상태 머신) 기반으로 설계하여 상태 간 결합도를 낮추고 확장성을 확보했습니다. 보스 AI는 플레이어의 현재 상태를 실시간으로 읽어 반응형으로 행동합니다.

---

## 코드 상세

### 플레이어 (`Player/`)

플레이어의 모든 행동은 `PlayerStateMachine`이 관리하며, 각 상태는 `Enter` / `LogicUpdate` / `PhysicsUpdate` / `Exit` 생명주기를 갖습니다.

| 파일 | 역할 |
|---|---|
| [`Player/PlayerController.cs`](./My%20project/Assets/Scripts/Player/PlayerController.cs) | 플레이어 최상위 컨트롤러. 입력 처리, 컴포넌트 참조, 물리 연산 담당 |
| [`Player/PlayerStateMachine.cs`](./My%20project/Assets/Scripts/Player/PlayerStateMachine.cs) | 현재 상태 관리 및 전이 처리. `PreviousState` 추적 포함 |
| [`Player/PlayerState.cs`](./My%20project/Assets/Scripts/Player/PlayerState.cs) | 모든 플레이어 상태의 베이스 클래스 |
| [`Player/PlayerStatsSO.cs`](./My%20project/Assets/Scripts/Player/PlayerStatsSO.cs) | ScriptableObject 기반 스탯 데이터. 체력, 스태미나, 이동속도, 공격력, 대시, 패링 수치 포함 |
| [`Player/PlayerStatsManager.cs`](./My%20project/Assets/Scripts/Player/PlayerStatsManager.cs) | 스태미나 소비 및 자동 회복 처리 |
| [`Player/PlayerHealth.cs`](./My%20project/Assets/Scripts/Player/PlayerHealth.cs) | 피격 처리, 사망 판정, 힐 차지 관리 |
| [`Player/PlayerDefendState.cs`](./My%20project/Assets/Scripts/Player/PlayerDefendState.cs) | 방어 및 패링 로직. 입력 초반 `parryWindow`(0.15초) 내 적 공격 감지 시 패링 성립 |
| [`Player/PlayerAttackState.cs`](./My%20project/Assets/Scripts/Player/PlayerAttackState.cs) | 지상 콤보 공격. `AttackData` 배열로 다단 콤보 구성, `comboResetTime` 내 연속 입력 처리 |
| [`Player/PlayerAirAttackState.cs`](./My%20project/Assets/Scripts/Player/PlayerAirAttackState.cs) | 공중 공격 상태 |
| [`Player/PlayerDashAttackState.cs`](./My%20project/Assets/Scripts/Player/PlayerDashAttackState.cs) | 대시 공격 상태 |
| [`Player/PlayerSpecialAttackState.cs`](./My%20project/Assets/Scripts/Player/PlayerSpecialAttackState.cs) | 특수 공격 상태 |
| [`Player/PlayerThrowState.cs`](./My%20project/Assets/Scripts/Player/PlayerThrowState.cs) | 투척 공격 상태 |
| [`Player/PlayerDashState.cs`](./My%20project/Assets/Scripts/Player/PlayerDashState.cs) | 대시 상태. 스태미나 소비 적용 |
| [`Player/PlayerHealState.cs`](./My%20project/Assets/Scripts/Player/PlayerHealState.cs) | 힐 상태. 힐 차지 소비 처리 |
| [`Player/PlayerHitState.cs`](./My%20project/Assets/Scripts/Player/PlayerHitState.cs) | 피격 상태 |
| [`Player/PlayerDeathState.cs`](./My%20project/Assets/Scripts/Player/PlayerDeathState.cs) | 사망 상태 |
| [`Player/PlayerGroundedState.cs`](./My%20project/Assets/Scripts/Player/PlayerGroundedState.cs) | 지상 공통 상태 (Idle / Move / Run의 베이스) |
| [`Player/PlayerInAirState.cs`](./My%20project/Assets/Scripts/Player/PlayerInAirState.cs) | 공중 공통 상태 |
| [`Player/PlayerWallSlideState.cs`](./My%20project/Assets/Scripts/Player/PlayerWallSlideState.cs) | 벽 슬라이드 상태 |
| [`Player/PlayerWallJumpState.cs`](./My%20project/Assets/Scripts/Player/PlayerWallJumpState.cs) | 벽 점프 상태 |
| [`Player/PlayerClimbState.cs`](./My%20project/Assets/Scripts/Player/PlayerClimbState.cs) | 클라이밍 상태 |
| [`Player/PlayerCrouchState.cs`](./My%20project/Assets/Scripts/Player/PlayerCrouchState.cs) | 웅크리기 상태 |
| [`Player/ParryEffect.cs`](./My%20project/Assets/Scripts/Player/ParryEffect.cs) | 패링 성립 시 시각 이펙트 처리 |
| [`Player/AnimationEventProxy.cs`](./My%20project/Assets/Scripts/Player/AnimationEventProxy.cs) | 애니메이션 이벤트를 상태 머신으로 전달하는 프록시 |

### 보스 AI (`Enemy/`)

`EnemyStateMachine`으로 보스 전용 AI를 구현했습니다. 플레이어의 현재 상태(`IsPlayerHealing()`, `IsPlayerDefending()` 등)를 실시간으로 감지하여 반응형으로 행동합니다.

| 파일 | 역할 |
|---|---|
| [`Enemy/EnemyController.cs`](./My%20project/Assets/Scripts/Enemy/EnemyController.cs) | 보스 최상위 컨트롤러. AI 판단, 공격 범위 감지, 플레이어 상태 확인 메서드 포함 |
| [`Enemy/EnemyStateMachine.cs`](./My%20project/Assets/Scripts/Enemy/EnemyStateMachine.cs) | 보스 상태 관리 및 전이 처리 |
| [`Enemy/EnemyState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyState.cs) | 모든 보스 상태의 베이스 클래스 |
| [`Enemy/Enemy.cs`](./My%20project/Assets/Scripts/Enemy/Enemy.cs) | 보스 체력, 피격 처리, 2페이즈 진입 조건(체력 50% 이하) 판정 |
| [`Enemy/EnemyIdleState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyIdleState.cs) | 대기 상태. `minActionDelay ~ maxActionDelay` 랜덤 딜레이 후 다음 행동 결정 |
| [`Enemy/EnemyChaseState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyChaseState.cs) | 추적 상태 |
| [`Enemy/EnemyMeleeAttackState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyMeleeAttackState.cs) | 근접 공격 상태 |
| [`Enemy/EnemyRangedAttackState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyRangedAttackState.cs) | 원거리 공격 상태 |
| [`Enemy/EnemyRushState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyRushState.cs) | 돌진 공격 상태 |
| [`Enemy/EnemyBackAwayState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyBackAwayState.cs) | 후퇴 상태 |
| [`Enemy/EnemyParryState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyParryState.cs) | 보스 패링 상태 |
| [`Enemy/EnemyEnrageState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyEnrageState.cs) | 2페이즈 각성 상태. 각성 애니메이션 1회 재생 보장 처리 포함 |
| [`Enemy/EnemyHitState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyHitState.cs) | 피격 상태 |
| [`Enemy/EnemyDeathState.cs`](./My%20project/Assets/Scripts/Enemy/EnemyDeathState.cs) | 사망 상태 |
| [`Enemy/EnemyAnimationEventProxy.cs`](./My%20project/Assets/Scripts/Enemy/EnemyAnimationEventProxy.cs) | 보스 애니메이션 이벤트 프록시 |

### 시스템 / UI (`UI/`, `Sound/`, `Camera/`)

| 파일 | 역할 |
|---|---|
| [`Camera/CameraController.cs`](./My%20project/Assets/Scripts/Camera/CameraController.cs) | 공격 히트 시 카메라 셰이크 연출. `cameraShakeOnHitIntensity / Duration` 값 기반 |
| [`Sound/SoundManager.cs`](./My%20project/Assets/Scripts/Sound/SoundManager.cs) | BGM, SFX 중앙 관리 싱글턴. 랜덤 재생 지원 |
| [`Sound/SoundType.cs`](./My%20project/Assets/Scripts/Sound/SoundType.cs) | 사운드 종류 열거형 |
| [`UI/PlayerHUD.cs`](./My%20project/Assets/Scripts/UI/PlayerHUD.cs) | 체력, 스태미나, 힐 차지, 특수 공격 차지 표시 |
| [`UI/BossHUD.cs`](./My%20project/Assets/Scripts/UI/BossHUD.cs) | 보스 체력바 UI |
| [`UI/GameOverUI.cs`](./My%20project/Assets/Scripts/UI/GameOverUI.cs) | 사망 메시지 랜덤화, 리트라이 기능 |
| [`UI/DialogueSystem.cs`](./My%20project/Assets/Scripts/UI/DialogueSystem.cs) | 인게임 대화 시스템. `DialogueData(ScriptableObject)` + `SpeechBubble` 프리팹 조합 |
| [`UI/DangerUIController.cs`](./My%20project/Assets/Scripts/UI/DangerUIController.cs) | 위험 상황 UI 연출 |

---

## 핵심 구현

### FSM 기반 플레이어 컨트롤러

`PlayerStateMachine` + `PlayerState` 베이스 클래스를 설계하고 20개 이상의 독립 상태 클래스로 행동을 분리했습니다. 상태 전이 시 `PreviousState`를 저장해 직전 상태를 추적할 수 있습니다.

구현 상태: Idle, Move, Run, Jump, Dash, DashAttack, Attack(콤보), AirAttack, SpecialAttack, Throw, Defend, Parry, Heal, Hit, Death, WallSlide, WallJump, Climb, Crouch

### 보스 AI FSM 및 2페이즈 각성 시스템

체력이 50% 이하로 떨어지면 `EnrageState`로 전환되는 2페이즈 각성 시스템을 구현했습니다. 각성 애니메이션은 1회만 재생되도록 처리했으며, 보스 AI는 `IsPlayerHealing()`, `IsPlayerDefending()`, `IsPlayerAttacking()` 등 플레이어 상태를 실시간으로 감지해 반응형으로 행동합니다.

### 소울라이크 핵심 전투 시스템

- 패링: 방어 입력 초반 `parryWindow`(0.15초) 내 적 공격 감지 시 패링 성립, 시각 이펙트 출력
- 스태미나: 달리기, 점프, 대시, 공격 등 모든 행동에 스태미나 소비 적용, 딜레이 후 자동 회복
- 콤보 공격: `AttackData` 배열로 다단 콤보 구성, `comboResetTime` 내 연속 입력 시 다음 공격으로 진행

### ScriptableObject 기반 스탯 관리

`PlayerStatsSO`로 플레이어 수치를 코드와 분리하여 인스펙터에서 직접 조정 가능하도록 설계했습니다.

---

## 폴더 구조

- `My project/Assets/Scripts/Player/` : 플레이어 FSM 상태 클래스 전체
- `My project/Assets/Scripts/Enemy/` : 보스 AI FSM 상태 클래스 전체
- `My project/Assets/Scripts/Camera/` : 카메라 셰이크 연출
- `My project/Assets/Scripts/Sound/` : 사운드 매니저
- `My project/Assets/Scripts/UI/` : HUD, 대화 시스템, 게임오버 UI
