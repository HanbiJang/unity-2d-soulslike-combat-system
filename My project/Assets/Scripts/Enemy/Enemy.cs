using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;
    protected SpriteRenderer sr;

    [Header("보스 설정")]
    [Tooltip("이 적이 보스인지 여부 (보스일 경우 화면 하단에 UI가 표시됩니다)")]
    [SerializeField] private bool isBoss = false;
    
    [Tooltip("보스의 이름 (보스일 경우에만 사용됩니다)")]
    [SerializeField] private string bossName = "보스";

    [Tooltip("이 적이 플레이어에게 입히는 피해량")]
    public int attackDamage = 1;
    
    [Tooltip("타격 시 재생될 사운드 재질 타입")]
    public MaterialType materialType = MaterialType.Flesh;

    // 체력 정보 접근용 프로퍼티
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsBoss => isBoss;
    public string BossName => bossName;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        
        // 보스인 경우 BossHUD에 등록
        if (isBoss)
        {
            BossHUD.Instance?.RegisterBoss(this);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + "가 " + damage + "의 피해를 입었습니다! 현재 체력: " + currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + "가 처치되었습니다.");
        
        // 보스인 경우 BossHUD에서 해제
        if (isBoss)
        {
            BossHUD.Instance?.UnregisterBoss(this);
        }
        
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        // 객체가 파괴될 때도 BossHUD에서 해제 (안전장치)
        if (isBoss)
        {
            // BossHUD가 존재하고 활성화되어 있는지 확인
            if (BossHUD.Instance != null && BossHUD.Instance.gameObject != null)
            {
                BossHUD.Instance.UnregisterBoss(this);
            }
        }
    }
}
