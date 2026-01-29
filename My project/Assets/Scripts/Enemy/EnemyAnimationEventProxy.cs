using UnityEngine;

public class EnemyAnimationEventProxy : MonoBehaviour
{
    private EnemyController enemyController;

    void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void TriggerAnimationEvent()
    {
        enemyController?.AnimationTrigger();
    }
}
