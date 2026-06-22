using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable : MonoBehaviour
{
    [Header("대화 데이터")]
    [Tooltip("이 NPC와 상호작용 시 시작할 기본 대화 (대화1)")]
    [SerializeField] private DialogueData dialogueData;

    [Header("아이템 이벤트 대화")]
    [Tooltip("이 ID의 아이템을 가지고 있으면 기본 대화 대신 아래 대화2가 재생됨")]
    [SerializeField] private string requiredItemId;
    [Tooltip("아이템을 가지고 있을 때 재생할 대화 (대화2)")]
    [SerializeField] private DialogueData itemDialogueData;
    [Tooltip("대화2가 끝난 직후 발생할 월드 이벤트 (오브젝트 활성화/비활성화 등을 인스펙터에서 연결)")]
    [SerializeField] private UnityEvent onItemDialogueEnded;

    [Header("상호작용 설정")]
    [Tooltip("대화를 시작하는 키")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("상호작용 UI")]
    [Tooltip("플레이어가 범위 안에 있을 때 표시할 프롬프트 오브젝트")]
    [SerializeField] private GameObject interactPrompt;

    private bool playerInRange = false;

    private void Start()
    {
        SetPromptActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (DialogueSystem.Instance == null || DialogueSystem.Instance.IsDialogueActive) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (DialogueSystem.Instance == null) return;

        // 아이템 보유 여부에 따라 대화2(아이템 보유) 또는 대화1(기본)을 재생
        bool hasItem = !string.IsNullOrEmpty(requiredItemId)
            && InventoryManager.Instance != null
            && InventoryManager.Instance.HasItem(requiredItemId);

        DialogueSystem.Instance.SetInteractionTarget(transform);

        if (hasItem && itemDialogueData != null)
        {
            DialogueSystem.Instance.StartDialogue(itemDialogueData, onComplete: () => onItemDialogueEnded?.Invoke());
        }
        else if (dialogueData != null)
        {
            DialogueSystem.Instance.StartDialogue(dialogueData);
        }
        else
        {
            return;
        }

        SetPromptActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerInRange = true;
        SetPromptActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerInRange = false;
        SetPromptActive(false);
    }

    private void SetPromptActive(bool active)
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(active);
        }
    }
}
