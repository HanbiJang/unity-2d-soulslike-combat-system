using UnityEngine;

// 애니메이션 이벤트를 NormalMonsterController로 전달하는 브릿지
// 보스의 EnemyAnimationEventProxy와 동일한 역할
public class NormalMonsterAnimProxy : MonoBehaviour
{
    private NormalMonsterController monster;

    private void Awake()
    {
        monster = GetComponentInParent<NormalMonsterController>();
    }

    public void TriggerAnimationEvent()
    {
        monster?.AnimationTrigger();
    }
}
