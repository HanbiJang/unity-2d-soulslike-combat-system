using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 아이템 획득 시 화면에 잠깐 표시되는 알림
// LocationNameUI와 동일한 페이드인/홀드/페이드아웃 패턴
public class ItemPickupUI : MonoBehaviour
{
    public static ItemPickupUI Instance { get; private set; }

    [SerializeField] private Text messageText;
    [SerializeField] private float fadeInDuration  = 0.3f;
    [SerializeField] private float holdDuration    = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    // 현재 실행 중인 코루틴을 저장 (연속 습득 시 이전 걸 취소하기 위함)
    private Coroutine showCoroutine;

    private void Awake()
    {
        Instance = this;
        if (messageText == null) messageText = GetComponentInChildren<Text>();
        SetAlpha(0f); // 시작 시 투명하게
    }

    // 외부(ItemPickup)에서 호출 - 아이템 이름을 받아 메시지 표시
    public void Show(string itemName)
    {
        // 이미 표시 중이면 중단하고 새로 시작 (빠른 연속 습득 대응)
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        messageText.text = $"{itemName}을(를) 획득했습니다.";
        showCoroutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return StartCoroutine(FadeTo(1f, fadeInDuration));  // 나타남
        yield return new WaitForSeconds(holdDuration);            // 유지
        yield return StartCoroutine(FadeTo(0f, fadeOutDuration)); // 사라짐
    }

    // alpha를 duration초 동안 target 값으로 서서히 변경
    private IEnumerator FadeTo(float target, float duration)
    {
        float start   = messageText.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, target, elapsed / duration));
            yield return null;
        }
        SetAlpha(target); // 마지막에 정확한 값으로 고정
    }

    private void SetAlpha(float a)
    {
        Color c = messageText.color;
        c.a = a;
        messageText.color = c;
    }
}
