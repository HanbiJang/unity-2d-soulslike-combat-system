using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject bubblePanel;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image bubbleBackground;
    
    [Header("애니메이션 설정")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    [Header("위치 설정")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector2 offset = new Vector2(0, 2f);
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Camera mainCamera;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // SetActive 대신 CanvasGroup으로 숨김 (GameObject는 항상 활성 유지)
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    public void ShowDialogue(string speakerName, string text)
    {
        if (speakerNameText != null) speakerNameText.text = speakerName;
        if (dialogueText != null)    dialogueText.text = text;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        StartCoroutine(FadeIn());
    }

    private bool isFadingOut = false;
    
    // 즉시 숨김 (전환 시 겹침 방지용)
    public void HideImmediate()
    {
        StopAllCoroutines();
        isFadingOut = false;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void HideDialogue()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOut());
        }
    }

    public bool IsFadingOut => isFadingOut;

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        isFadingOut = true;
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        isFadingOut = false;
    }

    private void Update()
    {
        if (targetTransform != null && mainCamera != null && canvas != null)
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(targetTransform.position + (Vector3)offset);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            rectTransform.anchoredPosition = localPoint;
        }
    }
}



