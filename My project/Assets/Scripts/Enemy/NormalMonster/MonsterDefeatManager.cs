using UnityEngine;
using System.Collections.Generic;

// 처치한 몬스터의 ID를 게임 내내 기억해서, 씬에 재진입해도 다시 소환되지 않게 함
public class MonsterDefeatManager : MonoBehaviour
{
    public static MonsterDefeatManager Instance { get; private set; }

    private readonly HashSet<string> defeatedMonsterIds = new HashSet<string>();

    // 씬이 로드되기 전에 자동으로 생성됨 → 첫 씬에 배치 안 해도 됨
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("[MonsterDefeatManager]");
        go.AddComponent<MonsterDefeatManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 씬이 바뀌어도 이 오브젝트는 파괴되지 않음
        DontDestroyOnLoad(gameObject);
    }

    public bool IsDefeated(string monsterId)
    {
        return !string.IsNullOrEmpty(monsterId) && defeatedMonsterIds.Contains(monsterId);
    }

    public void MarkDefeated(string monsterId)
    {
        if (!string.IsNullOrEmpty(monsterId))
            defeatedMonsterIds.Add(monsterId);
    }
}
