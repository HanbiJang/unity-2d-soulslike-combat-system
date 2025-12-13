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
            healthFillImage.fillAmount = Mathf.Lerp(healthFillImage.fillAmount, target, Time.deltaTime * fillLerpSpeed);
        }
        if (staminaFillImage != null && player.StatsManager != null)
        {
            float target = player.StatsManager.CurrentStamina / player.stats.maxStamina;
            staminaFillImage.fillAmount = Mathf.Lerp(staminaFillImage.fillAmount, target, Time.deltaTime * fillLerpSpeed);
        }
    }
}
