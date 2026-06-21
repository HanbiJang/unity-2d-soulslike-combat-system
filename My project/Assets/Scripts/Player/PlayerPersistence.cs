using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;

    private void Awake()
    {
        // 다른 씬에 미리 배치된 Player는 중복이므로 제거
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환 시 입력 막힘/대시 레이어 무시 초기화
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.IsInputDisabled = false;
            pc.IsInvincible = false;

            // 대시 중 씬 이동 시 레이어 충돌 무시가 남을 수 있음 — 전부 복원
            if (pc.stats != null)
            {
                int playerLayer = gameObject.layer;
                int enemyMask = pc.stats.enemyLayer.value;
                for (int i = 0; i < 32; i++)
                {
                    if ((enemyMask & (1 << i)) != 0)
                        Physics2D.IgnoreLayerCollision(playerLayer, i, false);
                }
            }
        }

        // 새 씬의 카메라가 이 플레이어를 따라가도록 재연결
        CameraController cam = FindObjectOfType<CameraController>();
        if (cam != null)
        {
            cam.SetPlayerTarget(transform);
        }

        // 전환 트리거가 지정한 스폰포인트로 이동
        if (string.IsNullOrEmpty(SceneTransitionTrigger.PendingSpawnId)) return;

        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnId == SceneTransitionTrigger.PendingSpawnId)
            {
                transform.position = spawnPoint.transform.position;
                break;
            }
        }

        SceneTransitionTrigger.PendingSpawnId = null;
    }
}
