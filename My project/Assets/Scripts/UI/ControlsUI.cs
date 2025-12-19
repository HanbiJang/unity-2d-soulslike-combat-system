using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Text controlsText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("타이밍 설정")]
    [Tooltip("게임 시작 후 몇 초 후에 표시될지")]
    [SerializeField] private float showDelay = 3f;
    
    [Tooltip("표시된 후 몇 초 동안 보여줄지")]
    [SerializeField] private float displayDuration = 5f;
    
    [Header("애니메이션 설정")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    
    [Header("조작법 텍스트")]
    [TextArea(5, 10)]
    [Tooltip("여기에 조작법을 입력하세요")]
    [SerializeField] private string controlsMessage = "조작법을 여기에 입력하세요";

    private void Start()
    {
        // CanvasGroup이 없으면 추가
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        // Text 컴포넌트가 없으면 찾기
        if (controlsText == null)
        {
            controlsText = GetComponentInChildren<Text>();
        }
        
        // 초기 상태: 숨김
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        // 텍스트 설정
        if (controlsText != null)
        {
            controlsText.text = controlsMessage;
        }
        
        // 조작법 표시 시작
        StartCoroutine(ShowControlsSequence());
    }

    private IEnumerator ShowControlsSequence()
    {
        // 대기 시간
        yield return new WaitForSeconds(showDelay);
        
        // 페이드 인
        yield return StartCoroutine(FadeIn());
        
        // 표시 시간 대기
        yield return new WaitForSeconds(displayDuration);
        
        // 페이드 아웃
        yield return StartCoroutine(FadeOut());
        
        // 완전히 숨김
        gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        
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
        if (canvasGroup == null) yield break;
        
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
}




