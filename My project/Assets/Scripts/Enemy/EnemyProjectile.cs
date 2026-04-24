using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("피해량")]
    public int damage = 1;
    [Tooltip("이동 속도")]
    public float speed = 10f;
    [Tooltip("최대 비행 시간 (초)")]
    public float lifetime = 4f;
    [Tooltip("지형 레이어 (이 레이어에 닿으면 소멸)")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir, Collider2D[] ignoreColliders, LayerMask ground)
    {
        groundLayer = ground;

        // 발사한 보스의 콜라이더 무시
        Collider2D[] myColliders = GetComponents<Collider2D>();
        foreach (var mine in myColliders)
            foreach (var ignore in ignoreColliders)
                if (ignore != null) Physics2D.IgnoreCollision(mine, ignore, true);

        if (rb != null)
            rb.velocity = dir.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 피해
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, transform);
            Destroy(gameObject);
            return;
        }

        // 지형 충돌 시 소멸
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
