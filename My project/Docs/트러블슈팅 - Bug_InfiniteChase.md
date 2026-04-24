# 무한 Chase 버그 수정 정리

---

## Before — 기존 설계

`CanPerformAction()`이 **"지금 행동할 수 있는지 여부 판단" + "다음 행동까지 대기 타이머 시작"을 한 번에** 처리했습니다.

```csharp
public bool CanPerformAction()
{
    if (nextActionDelay <= 0f)
    {
        nextActionDelay = Random.Range(1.5f, 5.5f); // 호출하는 순간 바로 타이머 시작
        lastActionTime = Time.time;
        return true;
    }
    if (Time.time >= lastActionTime + nextActionDelay)
    {
        nextActionDelay = Random.Range(1.5f, 5.5f); // 호출하는 순간 바로 타이머 시작
        lastActionTime = Time.time;
        return true;
    }
    return false;
}
```

ChaseState에서는 이렇게 사용:

```
canAct = CanPerformAction()     ← 이 시점에 타이머가 이미 시작됨

  → canAct=True이고 rush 조건 맞으면  → RushState
  → canAct=True이고 ranged 조건 맞으면 → RangedAttackState
  → canAct=True인데 아무 조건도 안 맞으면 → MoveTowardsPlayer()만 실행
  
  → 세 경우 모두 타이머는 이미 시작되어 있음
```

---

## Problem — 문제의 원인

> `canAct=True`가 됐을 때, **실제로 행동을 하지 않았어도 타이머가 시작**됩니다.
> 즉, "행동할 수 있는 타이밍"이 왔는데 마땅한 행동이 없으면, 그 타이밍은 그냥 버려지고 다음 타이밍까지 1.5~5.5초를 기다려야 합니다.

### 재현 시나리오

플레이어가 보스를 한 대 때리고 즉시 회피(hit and run)하면:

1. 보스가 HitState 진입 → 0.5초 후 ChaseState 복귀
2. ChaseState 복귀 직후 `CanPerformAction()` 호출 → `canAct=True`, **타이머 시작**
3. 플레이어가 **죽은 구간**(attackRange ~ rushTriggerDistance 사이)에 위치
4. 어떤 행동 조건도 불만족 → `MoveTowardsPlayer()`만 실행
5. 타이머가 이미 시작됐으므로 → 다음 1.5~5.5초 동안 `canAct=False`
6. 이 사이에 플레이어가 rush 범위 안으로 들어와도 반응 불가
7. 다음 타이밍에서도 또 죽은 구간 → 무한 반복

### 실제 로그 증거

| 시각 | 거리 | canAct | 결과 |
|------|------|--------|------|
| t=10.68 | 7.88 | **True** | rush 거리(8) 미달 → 행동 없음, 타이머 시작 |
| t=10.76 | **8.39** | False | rush 가능한 거리인데 타이머 대기 중이라 불가 |
| ... | ... | False | 계속 대기 |
| t=13.21 | 4.59 | **True** | 모든 조건 불만족 → 행동 없음, 타이머 또 시작 |
| t=13.31 | 4.59 | False | 또 1.5~5.5초 대기... |

### 죽은 구간(Dead Zone) 구조

```
0          attackRange(3)     rushTriggerDistance(8)    rangedAttackDistance(10)
|---------------|----------------------|------------------------|
  근접 공격 가능  ← 죽은 구간: 아무것도 못함 →  돌진 가능     원거리 가능
```

플레이어가 이 죽은 구간(3~8)을 유지하면:

- 근접 공격: 거리가 너무 멀어서 불가 (`dist > 3`)
- 돌진: 거리가 너무 가까워서 불가 (`dist < 8`)
- 원거리: 거리가 너무 가까워서 불가 (`dist < 10`)
- 결과: 행동할 수 있는 타이밍이 와도 쓸 수 없고, 타이머만 시작되어 또 대기 → **무한 Chase**

---

## After — 해결 방법

**"지금 행동할 수 있는지 판단"** 과 **"다음 행동까지 대기 타이머 시작"** 을 두 함수로 분리했습니다.

```csharp
// 지금 행동해도 되는 타이밍인지 판단만 함 (타이머는 건드리지 않음)
public bool CanPerformAction()
{
    if (nextActionDelay <= 0f) return true;
    return Time.time >= lastActionTime + nextActionDelay;
}

// 실제로 행동을 실행했을 때만 호출 → 이때 비로소 다음 대기 타이머 시작
public void ConsumeAction()
{
    nextActionDelay = Random.Range(1.5f, 5.5f);
    lastActionTime = Time.time;
}
```

ChaseState에서는 **실제로 행동을 실행하는 시점에만** `ConsumeAction()` 호출:

```csharp
bool canAct = enemy.CanPerformAction(); // 판단만, 타이머 시작 안 함

if (canAct && dist >= rushTriggerDistance && rushCooldown 완료)
{
    enemy.ConsumeAction(); // 실제로 돌진할 때만 타이머 시작
    stateMachine.ChangeState(RushState);
    return;
}
if (canAct && dist >= rangedAttackDistance && rangedCooldown 완료)
{
    enemy.ConsumeAction(); // 실제로 원거리 공격할 때만 타이머 시작
    stateMachine.ChangeState(RangedAttackState);
    return;
}
// 조건 불만족 → ConsumeAction() 호출 안 함 → 타이머 시작 안 됨
// → 다음 프레임에도 canAct=True 유지 → 조건이 맞는 순간 즉시 반응
```

---

## Effect — 수정 효과

### 흐름 비교

**수정 전:**
```
행동 타이밍 도래 → 조건 불만족 → 타이머 시작 → 1.5~5.5초 대기 → 반복
                                                      ↑
                                       이 사이에 기회가 와도 반응 불가
```

**수정 후:**
```
행동 타이밍 도래 → 조건 불만족 → 타이머 시작 안 함 → 다음 프레임도 즉시 판단
                                                            ↓
                                            플레이어가 rush 범위 진입 → 즉시 돌진
```

### 수치 비교

| 항목 | 수정 전 | 수정 후 |
|------|---------|---------|
| 죽은 구간에서 타이밍 낭비 | 발생 | 없음 |
| rush 범위 진입 반응 속도 | 최대 5.5초 지연 | 즉시 반응 |
| hit and run 대응 | Chase 무한 지속 | 범위 진입 즉시 행동 |
| 행동 간 딜레이 의도 | 유지 안됨 (낭비됨) | 정확히 유지됨 |
