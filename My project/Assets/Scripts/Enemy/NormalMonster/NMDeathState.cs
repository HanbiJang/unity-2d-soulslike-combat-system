using UnityEngine;

public class NMDeathState : NormalMonsterState
{
    // 사망 애니메이션 재생 후 오브젝트 제거까지 대기 시간
    private const float DURATION = 1.5f;
    private float timer;

    public NMDeathState(NormalMonsterController monster, string stateName)
        : base(monster, stateName) { }

    public override void Enter()
    {
        base.Enter();
        monster.StopMovement();
        timer = 0f;
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= DURATION)
            Object.Destroy(monster.gameObject);
    }
}
