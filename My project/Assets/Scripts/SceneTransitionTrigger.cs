using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Tooltip("이동할 씬 이름 (Build Settings에 등록되어 있어야 함)")]
    [SerializeField] private string targetSceneName;

    [Tooltip("이동 후 플레이어가 위치할 스폰포인트 ID (PlayerSpawnPoint의 spawnId와 일치해야 함)")]
    [SerializeField] private string targetSpawnId;

    // 씬 전환 후 PlayerPersistence가 읽어가는 다음 스폰포인트 ID
    public static string PendingSpawnId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        PendingSpawnId = targetSpawnId;
        SceneManager.LoadScene(targetSceneName);
    }
}
