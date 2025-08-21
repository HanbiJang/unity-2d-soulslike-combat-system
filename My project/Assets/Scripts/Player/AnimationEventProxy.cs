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
}
