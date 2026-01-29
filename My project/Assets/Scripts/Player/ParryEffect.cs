using System.Collections;
using UnityEngine;

/// <summary>
/// 패링 성공 시 특수 효과를 관리하는 클래스
/// - 원형 충격파
/// - 화면 흔들림
/// - 플레이어 발광 효과
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class ParryEffect : MonoBehaviour
{
    [Header("충격파 설정")]
    [Tooltip("충격파 최대 크기")]
    [SerializeField] private float shockwaveMaxScale = 3f;
    
    [Tooltip("충격파 지속 시간")]
    [SerializeField] private float shockwaveDuration = 0.5f;
    
    [Tooltip("충격파 색상")]
    [SerializeField] private Color shockwaveColor = new Color(1f, 0.8f, 0.3f, 0.8f); // 금색
    
    [Header("발광 효과 설정")]
    [Tooltip("발광 색상")]
    [SerializeField] private Color glowColor = new Color(2f, 1.5f, 0.5f, 1f); // 밝은 금색 (HDR)
    
    [Tooltip("발광 지속 시간")]
    [SerializeField] private float glowDuration = 0.3f;
    
    [Header("화면 흔들림 설정")]
    [Tooltip("화면 흔들림 강도")]
    [SerializeField] private float shakeIntensity = 0.3f;
    
    [Tooltip("화면 흔들림 지속 시간")]
    [SerializeField] private float shakeDuration = 0.2f;
    
    private SpriteRenderer playerSpriteRenderer;
    private Color originalColor;
    private bool isGlowing = false;

    private void Awake()
    {
        // 플레이어의 SpriteRenderer 찾기
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            originalColor = Color.white;
        }
    }

    /// <summary>
    /// 패링 성공 시 모든 효과를 재생합니다.
    /// </summary>
    public void PlayParryEffect()
    {
        // 1. 원형 충격파 생성
        SpawnShockwave();
        
        // 2. 화면 흔들림
        TriggerCameraShake();
        
        // 3. 플레이어 발광 효과
        StartCoroutine(PlayerGlowEffect());
        
        Debug.Log("패링 효과 재생!");
    }

    /// <summary>
    /// 원형 충격파를 생성합니다.
    /// </summary>
    private void SpawnShockwave()
    {
        // 충격파 GameObject 생성
        GameObject shockwaveObj = new GameObject("ParryShockwave");
        shockwaveObj.transform.position = transform.position;
        shockwaveObj.transform.localScale = Vector3.zero;
        
        // SpriteRenderer 추가
        SpriteRenderer sr = shockwaveObj.AddComponent<SpriteRenderer>();
        
        // 원형 스프라이트 생성 (코드로 생성)
        Texture2D texture = CreateCircleTexture(256);
        Sprite circleSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        sr.sprite = circleSprite;
        sr.color = shockwaveColor;
        sr.sortingOrder = 100; // 최상위 레이어
        
        // 충격파 애니메이션 코루틴 시작
        StartCoroutine(AnimateShockwave(shockwaveObj));
    }

    /// <summary>
    /// 원형 텍스처를 생성합니다.
    /// </summary>
    private Texture2D CreateCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                // 원형 그라디언트 생성 (안쪽이 투명, 테두리만 보임)
                float alpha = 0f;
                if (distance > radius * 0.8f && distance < radius)
                {
                    // 테두리만 표시
                    alpha = 1f - Mathf.Abs(distance - radius * 0.9f) / (radius * 0.1f);
                }
                
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }

    /// <summary>
    /// 충격파 애니메이션을 실행합니다.
    /// </summary>
    private IEnumerator AnimateShockwave(GameObject shockwave)
    {
        float elapsed = 0f;
        SpriteRenderer sr = shockwave.GetComponent<SpriteRenderer>();
        Color startColor = shockwaveColor;
        
        while (elapsed < shockwaveDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / shockwaveDuration;
            
            // Scale 증가 (0 → maxScale)
            float scale = Mathf.Lerp(0f, shockwaveMaxScale, progress);
            shockwave.transform.localScale = new Vector3(scale, scale, 1f);
            
            // Alpha 감소 (투명해짐)
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, progress);
            sr.color = color;
            
            yield return null;
        }
        
        // 충격파 제거
        Destroy(shockwave);
    }

    /// <summary>
    /// 플레이어 발광 효과를 실행합니다.
    /// </summary>
    private IEnumerator PlayerGlowEffect()
    {
        if (playerSpriteRenderer == null)
        {
            yield break;
        }
        
        isGlowing = true;
        float elapsed = 0f;
        
        // 발광 시작
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / glowDuration;
            
            // 색상 보간 (흰색 → 금색 → 흰색)
            if (progress < 0.3f)
            {
                // 첫 30%: 빠르게 밝아짐
                float t = progress / 0.3f;
                playerSpriteRenderer.color = Color.Lerp(originalColor, glowColor, t);
            }
            else
            {
                // 나머지 70%: 서서히 원래대로
                float t = (progress - 0.3f) / 0.7f;
                playerSpriteRenderer.color = Color.Lerp(glowColor, originalColor, t);
            }
            
            yield return null;
        }
        
        // 원래 색상으로 복구
        playerSpriteRenderer.color = originalColor;
        isGlowing = false;
    }

    /// <summary>
    /// 화면 흔들림을 트리거합니다.
    /// </summary>
    private void TriggerCameraShake()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraController camController = mainCam.GetComponent<CameraController>();
            if (camController != null)
            {
                camController.TriggerShake(shakeIntensity, shakeDuration);
            }
        }
    }
}
