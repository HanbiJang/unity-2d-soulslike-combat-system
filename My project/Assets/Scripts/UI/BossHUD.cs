using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    public static BossHUD Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image healthBackgroundImage;
    [SerializeField] private Image postureFillImage;
    
    [Header("애니메이션 설정")]
    [SerializeField] private float fillLerpSpeed = 5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Enemy currentBoss;
    private EnemyController currentBossController;
    private CanvasGroup canvasGroup;
    private bool isVisible = false;

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

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 초기에는 숨김
        if (bossPanel != null)
        {
            bossPanel.SetActive(false);
        }
        canvasGroup.alpha = 0f;
        isVisible = false;
    }

    public void RegisterBoss(Enemy boss)
    {
        if (boss == null) return;

        // 기존 보스가 있으면 해제
        if (currentBoss != null && currentBoss != boss)
        {
            UnregisterBoss(currentBoss);
        }

        currentBoss = boss;
        currentBossController = boss as EnemyController ?? boss.GetComponent<EnemyController>();
        ShowBossUI();
    }

    public void UnregisterBoss(Enemy boss)
    {
        // BossHUD가 파괴되었거나 파괴 중이면 무시
        if (this == null || !gameObject.activeInHierarchy) return;
        
        if (currentBoss == boss)
        {
            currentBoss = null;
            currentBossController = null;
            HideBossUI();
        }
    }

    private void ShowBossUI()
    {
        if (currentBoss == null) return;

        // 보스 이름 설정
        if (bossNameText != null)
        {
            bossNameText.text = currentBoss.BossName;
        }

        // 체력바 초기화
        if (healthFillImage != null)
        {
            float healthPercent = (float)currentBoss.CurrentHealth / currentBoss.MaxHealth;
            healthFillImage.fillAmount = healthPercent;
        }

        // 체간바 초기화
        if (postureFillImage != null)
            postureFillImage.fillAmount = 1f;

        // UI 표시
        if (bossPanel != null)
        {
            bossPanel.SetActive(true);
        }

        // 페이드 인 애니메이션
        StartCoroutine(FadeIn());
    }

    private void HideBossUI()
    {
        if (!isVisible) return;

        // 객체가 파괴되었거나 파괴 중이면 즉시 숨김 (코루틴 시작 불가)
        if (this == null || !gameObject.activeInHierarchy)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            isVisible = false;
            if (bossPanel != null)
            {
                bossPanel.SetActive(false);
            }
            return;
        }

        // 정상 상태면 페이드 아웃 애니메이션
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        isVisible = true;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isVisible = false;

        if (bossPanel != null)
        {
            bossPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentBoss == null || !isVisible) return;

        // 체력바 업데이트
        if (healthFillImage != null)
        {
            float targetFill = (float)currentBoss.CurrentHealth / currentBoss.MaxHealth;
            healthFillImage.fillAmount = Mathf.MoveTowards(
                healthFillImage.fillAmount,
                targetFill,
                Time.deltaTime * fillLerpSpeed
            );
        }

        // 체간바 업데이트
        if (postureFillImage != null && currentBossController != null)
        {
            postureFillImage.fillAmount = Mathf.MoveTowards(
                postureFillImage.fillAmount,
                currentBossController.PosturePercentage,
                Time.deltaTime * fillLerpSpeed
            );
        }
    }
}
