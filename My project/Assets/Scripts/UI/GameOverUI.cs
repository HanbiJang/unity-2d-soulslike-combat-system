using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Image darkOverlay;

    [Header("애니메이션 설정")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float textDelay = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isShowing = false;

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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (darkOverlay != null)
        {
            darkOverlay.gameObject.SetActive(false);
        }
        canvasGroup.alpha = 0f;
        isShowing = false;

        // 리트라이 버튼 이벤트 연결
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(Retry);
        }
    }

    public void ShowGameOver()
    {
        if (isShowing) return;

        isShowing = true;
        StartCoroutine(ShowGameOverCoroutine());
    }

    private IEnumerator ShowGameOverCoroutine()
    {
        // 어두운 오버레이 표시
        if (darkOverlay != null)
        {
            darkOverlay.gameObject.SetActive(true);
            darkOverlay.color = new Color(0, 0, 0, 0);
            
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 0.9f, elapsed / fadeInDuration);
                darkOverlay.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            darkOverlay.color = new Color(0, 0, 0, 0.9f);
        }

        // 텍스트 딜레이
        yield return new WaitForSeconds(textDelay);

        // 랜덤 게임 오버 메시지 설정
        if (gameOverText != null)
        {
            gameOverText.text = GetRandomGameOverMessage();
        }

        // Game Over 패널 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 페이드 인 애니메이션
        float elapsed2 = 0f;
        while (elapsed2 < fadeInDuration)
        {
            elapsed2 += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed2 / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void Retry()
    {
        // 현재 씬 리로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HideGameOver()
    {
        if (!isShowing) return;

        isShowing = false;
        StartCoroutine(HideGameOverCoroutine());
    }

    private IEnumerator HideGameOverCoroutine()
    {
        // 페이드 아웃
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (darkOverlay != null)
        {
            darkOverlay.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 랜덤 게임 오버 메시지를 반환합니다.
    /// </summary>
    private string GetRandomGameOverMessage()
    {
        string[] messages = {
            "YOU DIED",
            "판결자가 당신을 기다립니다...",
            "판결자의 양분이 되었습니다",
            "기록에서 삭제되었습니다",
            "받아치기 연습이 필요합니다"
        };
        
        return messages[Random.Range(0, messages.Length)];
    }
}

