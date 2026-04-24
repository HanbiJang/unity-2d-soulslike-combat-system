using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private float deathStartTime;
    private float deathDuration = 2f;
    private bool dialogueTriggered = false;

    public EnemyDeathState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        deathStartTime = Time.time;
        dialogueTriggered = false;
        enemy.SetVelocity(0, 0);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.EnemyDeath);
        }

        Debug.Log(enemy.gameObject.name + "가 처치되었습니다.");

        if (enemy.IsBoss)
        {
            BossHUD.Instance?.UnregisterBoss(enemy);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!dialogueTriggered && Time.time >= deathStartTime + deathDuration)
        {
            dialogueTriggered = true;

            if (enemy.deathDialogue != null && DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(enemy.deathDialogue, fadeOutOnEnd: true);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.SetVelocity(0, 0);
    }
}

