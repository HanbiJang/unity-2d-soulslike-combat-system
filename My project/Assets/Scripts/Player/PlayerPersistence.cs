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
