using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("대화 데이터")]
    [Tooltip("씬 시작 시 재생할 대화 데이터")]
    [SerializeField] private DialogueData startDialogue;

    [Header("말풍선 UI")]
    [SerializeField] private SpeechBubble playerBubble;
    [SerializeField] private SpeechBubble enemyBubble;

    [Header("대화 진행 UI")]
    [SerializeField] private GameObject continuePrompt;
    [SerializeField] private Text continueText;

    [Header("참조")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;

    private DialogueData currentDialogue;
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;
    private bool canContinue = false;
    private PlayerController playerController;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 플레이어 찾기
        if (playerTransform == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player;
            }
        }

        // 적 찾기
        if (enemyTransform == null)
        {
            EnemyController enemy = FindObjectOfType<EnemyController>();
            if (enemy != null)
            {
                enemyTransform = enemy.transform;
            }
        }

        // Continue 프롬프트 초기화
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }
    }

    private void Start()
    {
        // 씬 시작 시 대화 재생
        if (startDialogue != null && startDialogue.dialogueLines.Length > 0)
        {
            StartDialogue(startDialogue);
        }
    }

    private void Update()
    {
        // 대화 중이고 계속 진행 가능할 때
        if (isDialogueActive && canContinue)
        {
            // 클릭 또는 스페이스바로 다음 대화
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                NextDialogue();
            }
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Length == 0) return;

        currentDialogue = dialogue;
        currentDialogueIndex = 0;
        isDialogueActive = true;
        canContinue = false;

        // 플레이어 입력 비활성화
        DisablePlayerInput();

        // 첫 번째 대화 표시
        ShowDialogueLine(0);
    }

    private void ShowDialogueLine(int index)
    {
        if (currentDialogue == null || index < 0 || index >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.dialogueLines[index];
        
        // 이전 말풍선 숨기기 (페이드 아웃 대기)
        StartCoroutine(HidePreviousBubblesAndShowNew(line));
    }
    
    private IEnumerator HidePreviousBubblesAndShowNew(DialogueLine line)
    {
        // 이전 말풍선 숨기기
        if (playerBubble != null)
        {
            playerBubble.HideDialogue();
        }
        if (enemyBubble != null)
        {
            enemyBubble.HideDialogue();
        }
        
        // 페이드 아웃이 완료될 때까지 대기 (최대 0.6초)
        float waitTime = 0f;
        while (waitTime < 0.6f && 
               ((playerBubble != null && playerBubble.IsFadingOut) || 
                (enemyBubble != null && enemyBubble.IsFadingOut)))
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        
        // 약간의 추가 딜레이 (부드러운 전환)
        yield return new WaitForSeconds(0.1f);

        // 타겟에 따라 말풍선 표시
        SpeechBubble targetBubble = null;
        Transform targetTransform = null;

        if (line.target == DialogueTarget.Player)
        {
            targetBubble = playerBubble;
            targetTransform = playerTransform;
        }
        else
        {
            targetBubble = enemyBubble;
            targetTransform = enemyTransform;
        }

        if (targetBubble != null && targetTransform != null)
        {
            targetBubble.SetTarget(targetTransform);
            targetBubble.ShowDialogue(line.speakerName, line.dialogueText);
        }

        // 자동 진행 또는 수동 진행
        if (currentDialogue.autoPlay)
        {
            StartCoroutine(AutoContinueDialogue());
        }
        else
        {
            canContinue = true;
            if (continuePrompt != null)
            {
                continuePrompt.SetActive(true);
            }
        }
    }

    private IEnumerator AutoContinueDialogue()
    {
        yield return new WaitForSeconds(currentDialogue.autoPlayDelay);
        NextDialogue();
    }

    public void NextDialogue()
    {
        if (!isDialogueActive) return;

        currentDialogueIndex++;
        canContinue = false;

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        if (currentDialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowDialogueLine(currentDialogueIndex);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        canContinue = false;

        // 모든 말풍선 숨기기
        if (playerBubble != null)
        {
            playerBubble.HideDialogue();
        }
        if (enemyBubble != null)
        {
            enemyBubble.HideDialogue();
        }

        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }

        // 플레이어 입력 활성화
        EnablePlayerInput();
    }

    private void DisablePlayerInput()
    {
        if (playerController != null)
        {
            // 이동/공격 입력 차단
            playerController.IsInputDisabled = true;
            // 대화 중에는 데미지로 죽지 않도록 무적 처리
            playerController.IsInvincible = true;
            // 플레이어를 Idle 상태로 고정
            if (playerController.StateMachine != null && playerController.StateMachine.CurrentState != playerController.IdleState)
            {
                playerController.StateMachine.ChangeState(playerController.IdleState);
            }
        }
    }

    private void EnablePlayerInput()
    {
        if (playerController != null)
        {
            // 무적 해제
            playerController.IsInvincible = false;
            playerController.IsInputDisabled = false;
        }
    }

    public bool IsDialogueActive => isDialogueActive;
}


