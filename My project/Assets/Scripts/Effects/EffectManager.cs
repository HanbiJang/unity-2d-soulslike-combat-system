using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("이펙트 프리팹")]
    [SerializeField] private GameObject enemyHitEffectPrefab;   // 1. 적 타격 이펙트
    [SerializeField] private GameObject parryEffectPrefab;      // 2. 패링 성공 이펙트
    [SerializeField] private GameObject playerHitEffectPrefab;  // 3. 플레이어 피격 이펙트

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayEnemyHitEffect(Vector2 position)   => SpawnEffect(enemyHitEffectPrefab, position);
    public void PlayParryEffect(Vector2 position)      => SpawnEffect(parryEffectPrefab, position);
    public void PlayPlayerHitEffect(Vector2 position)  => SpawnEffect(playerHitEffectPrefab, position);

    private void SpawnEffect(GameObject prefab, Vector2 position)
    {
        if (prefab == null) return;

        GameObject effect = Instantiate(prefab, position, Quaternion.identity);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            Destroy(effect, main.duration + main.startLifetime.constantMax);
        }
        else
        {
            Destroy(effect, 2f);
        }
    }
}
