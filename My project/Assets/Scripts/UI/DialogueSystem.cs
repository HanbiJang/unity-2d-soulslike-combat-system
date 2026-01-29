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
    [SerializeField] private Image continueImage;
    [SerializeField] private float fadeSpeed = 2f; // 페이드인아웃 속도

    [Header("참조")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private CameraController cameraController;

    private DialogueData currentDialogue;
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;
    private bool canContinue = false;
    private PlayerController playerController;
    private Coroutine fadeCoroutine; // 페이드인아웃 코루틴 참조

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
            // 씬 안에서 PlayerController를 찾아 Transform과 Controller를 모두 세팅
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player;
            }
        }
        else
        {
            // 인스펙터에서 Transform을 직접 넣어둔 경우, 여기서 PlayerController를 가져온다
            playerController = playerTransform.GetComponent<PlayerController>();
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

        // 카메라 컨트롤러 찾기
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }

        // Continue 프롬프트 초기화
        if (continuePrompt != null)
        {
            continuePrompt.SetActive(false);
        }
        
        // Continue 이미지 초기화 (투명하게)
        if (continueImage != null)
        {
            Color color = continueImage.color;
            color.a = 0f;
            continueImage.color = color;
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

        // 컷씬 시작 시 모든 적이 플레이어를 바라보도록
        FaceAllEnemiesTowardPlayer();

        // 플레이어 입력 비활성화
        DisablePlayerInput();

        // 첫 번째 대화 표시
        ShowDialogueLine(0);
    }

    private void FaceAllEnemiesTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector3 playerPos = playerTransform.position;
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float dir = (playerPos - enemy.transform.position).x;
            if (!Mathf.Approximately(dir, 0f))
                enemy.CheckAndFlip(dir);
        }
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
            
            // 카메라를 대사하는 캐릭터로 이동
            if (cameraController != null)
            {
                cameraController.SetTarget(targetTransform);
            }
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
            // 페이드인아웃 애니메이션 시작
            StartFadeAnimation();
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

        // 페이드인아웃 애니메이션 중지
        StopFadeAnimation();

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

        // 페이드인아웃 애니메이션 중지
        StopFadeAnimation();

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

        // 카메라를 원래 타겟(플레이어)으로 복원
        if (cameraController != null)
        {
            cameraController.RestoreOriginalTarget();
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

    /// <summary>
    /// Continue 이미지의 페이드인아웃 애니메이션을 시작합니다.
    /// </summary>
    private void StartFadeAnimation()
    {
        if (continueImage == null) return;

        // 기존 코루틴이 실행 중이면 중지
        StopFadeAnimation();

        // 새 코루틴 시작
        fadeCoroutine = StartCoroutine(FadeInOutAnimation());
    }

    /// <summary>
    /// Continue 이미지의 페이드인아웃 애니메이션을 중지합니다.
    /// </summary>
    private void StopFadeAnimation()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 이미지를 즉시 투명하게 만들기
        if (continueImage != null)
        {
            Color color = continueImage.color;
            color.a = 0f;
            continueImage.color = color;
        }
    }

    /// <summary>
    /// Continue 이미지를 0에서 1로 반복적으로 페이드인아웃하는 코루틴입니다.
    /// </summary>
    private IEnumerator FadeInOutAnimation()
    {
        if (continueImage == null) yield break;

        while (canContinue && isDialogueActive)
        {
            Color color = continueImage.color;
            
            // 0에서 1로, 1에서 0으로 반복 (Mathf.PingPong 사용)
            // Time.time * fadeSpeed를 사용하여 시간에 따라 0~1 사이를 왕복
            color.a = Mathf.PingPong(Time.time * fadeSpeed, 1f);
            continueImage.color = color;

            yield return null;
        }

        // 애니메이션 종료 시 투명하게
        if (continueImage != null)
        {
            Color color = continueImage.color;
            color.a = 0f;
            continueImage.color = color;
        }
    }
}


