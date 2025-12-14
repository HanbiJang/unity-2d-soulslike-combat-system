using UnityEngine;
using UnityEngine.UI;
public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private float fillLerpSpeed = 8f;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        
        // 배경음악 재생 (시작 시)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBackgroundMusic(SoundType.BackgroundMusic, 1f);
        }
    }

    private void Start()
    {
        if (player != null)
        {

            if (healthFillImage != null)
            {
                float h = player.Health != null ? (float)player.Health.CurrentHealth / player.stats.maxHealth : 0f;
                healthFillImage.fillAmount = h;
            }
            if (staminaFillImage != null)
            {
                float s = player.StatsManager != null ? player.StatsManager.CurrentStamina / player.stats.maxStamina : 0f;
                staminaFillImage.fillAmount = s;
            }
        }
    }

    private void Update()
    {
        if (player == null) return;
 
        if (healthFillImage != null && player.Health != null)
        {
            float target = (float)player.Health.CurrentHealth / player.stats.maxHealth;
            healthFillImage.fillAmount = Mathf.MoveTowards(healthFillImage.fillAmount, target, Time.deltaTime * fillLerpSpeed);
        }
        if (staminaFillImage != null && player.StatsManager != null)
        {
            float target = player.StatsManager.CurrentStamina / player.stats.maxStamina;
            staminaFillImage.fillAmount = Mathf.MoveTowards(staminaFillImage.fillAmount, target, Time.deltaTime * fillLerpSpeed);
        }
    }
}
