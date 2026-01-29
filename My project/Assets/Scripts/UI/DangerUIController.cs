using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 체력이 20% 이하일 때 화면에 붉은색 위험 UI를 점멸시키는 컨트롤러
/// </summary>
public class DangerUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Image dangerImage;
    
    [Header("Settings")]
    [Tooltip("체력이 이 비율 이하일 때 위험 UI 활성화")]
    [SerializeField] private float healthThreshold = 0.2f; // 20%
    
    [Tooltip("페이드 인/아웃 속도")]
    [SerializeField] private float fadeSpeed = 1.5f;
    
    [Tooltip("최대 알파값 (완전 불투명 = 1)")]
    [SerializeField] private float maxAlpha = 0.5f;
    
    [Tooltip("최소 알파값 (완전 투명 = 0)")]
    [SerializeField] private float minAlpha = 0f;

    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        // 플레이어 자동 찾기
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        // dangerImage가 설정되지 않은 경우 자동으로 찾기
        if (dangerImage == null)
        {
            dangerImage = GetComponent<Image>();
        }

        // Resources 폴더에서 dangerUI.png 자동 로드
        if (dangerImage != null && dangerImage.sprite == null)
        {
            Sprite dangerSprite = Resources.Load<Sprite>("dangerUI");
            if (dangerSprite != null)
            {
                dangerImage.sprite = dangerSprite;
            }
            else
            {
                Debug.LogWarning("dangerUI.png를 Resources 폴더에서 찾을 수 없습니다. Resources 폴더에 dangerUI.png 파일을 추가해주세요.");
            }
        }

        // 시작 시 UI 숨김
        if (dangerImage != null)
        {
            Color color = dangerImage.color;
            color.a = 0f;
            dangerImage.color = color;
            dangerImage.enabled = false;
        }
    }

    private void Update()
    {
        if (player == null || player.Health == null || dangerImage == null)
            return;

        // 현재 체력 비율 계산
        float healthRatio = (float)player.Health.CurrentHealth / player.stats.maxHealth;

        // 체력이 임계값 이하일 때
        if (healthRatio <= healthThreshold && healthRatio > 0)
        {
            if (!isFlashing)
            {
                StartFlashing();
            }
        }
        else
        {
            if (isFlashing)
            {
                StopFlashing();
            }
        }
    }

    private void StartFlashing()
    {
        isFlashing = true;
        
        if (dangerImage != null)
        {
            dangerImage.enabled = true;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private void StopFlashing()
    {
        isFlashing = false;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        // 페이드 아웃 후 비활성화
        if (dangerImage != null)
        {
            StartCoroutine(FadeOutAndDisable());
        }
    }

    private IEnumerator FlashCoroutine()
    {
        while (isFlashing)
        {
            // 페이드 인
            yield return StartCoroutine(FadeTo(maxAlpha));
            
            // 페이드 아웃
            yield return StartCoroutine(FadeTo(minAlpha));
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (dangerImage == null)
            yield break;

        Color color = dangerImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;
        float duration = Mathf.Abs(targetAlpha - startAlpha) / fadeSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            dangerImage.color = color;
            yield return null;
        }

        // 최종값 정확히 설정
        color.a = targetAlpha;
        dangerImage.color = color;
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return StartCoroutine(FadeTo(0f));
        
        if (dangerImage != null)
        {
            dangerImage.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
    }
}
