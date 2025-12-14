using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("AnimationEventProxy가 부모에서 PlayerController를 찾을 수 없습니다!");
        }
    }

    public void TriggerAnimationEvent()
    {
        if (playerController != null)
        {
            playerController.AnimationTrigger();
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 발소리 재생을 위해 호출
    /// </summary>
    public void PlayFootstepSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.PlayerFootstep);
        }
    }
}
