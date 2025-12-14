using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer sr;

    [Tooltip("이 적이 플레이어에게 입히는 피해량")]
    public int attackDamage = 1;
    
    [Tooltip("타격 시 재생될 사운드 재질 타입")]
    public MaterialType materialType = MaterialType.Flesh;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
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

    private void Die()
    {
        Debug.Log(gameObject.name + "가 처치되었습니다.");
        Destroy(gameObject);
    }
}
