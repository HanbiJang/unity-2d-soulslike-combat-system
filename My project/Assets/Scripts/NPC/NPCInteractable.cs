using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [Header("대화 데이터")]
    [Tooltip("이 NPC와 상호작용 시 시작할 대화 데이터")]
    [SerializeField] private DialogueData dialogueData;

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
        if (dialogueData == null || DialogueSystem.Instance == null) return;

        DialogueSystem.Instance.SetInteractionTarget(transform);
        DialogueSystem.Instance.StartDialogue(dialogueData);

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
