using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance { get; private set; }
    
    [Header("폰트 설정")]
    [SerializeField] private Font defaultFont;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        ApplyFontToAllTexts();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때마다 폰트 재적용
        ApplyFontToAllTexts();
    }
    
    /// <summary>
    /// 현재 씬의 모든 Text 컴포넌트에 폰트를 적용합니다.
    /// </summary>
    public void ApplyFontToAllTexts()
    {
        if (defaultFont == null)
        {
            Debug.LogWarning("FontManager: 기본 폰트가 설정되지 않았습니다!");
            return;
        }
        
        // 활성화되지 않은 GameObject도 포함하여 모든 Text 찾기
        Text[] allTexts = FindObjectsOfType<Text>(true);
        int appliedCount = 0;
        
        foreach (Text text in allTexts)
        {
            if (text != null)
            {
                text.font = defaultFont;
                appliedCount++;
            }
        }
        
        Debug.Log($"FontManager: {appliedCount}개의 Text 컴포넌트에 폰트를 적용했습니다.");
    }
}

