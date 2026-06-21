using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 획득한 아이템을 순서대로 저장하는 리스트
    private readonly List<ItemData> items = new List<ItemData>();

    // 외부에서 읽기만 가능하게 노출 (직접 수정 불가)
    public IReadOnlyList<ItemData> Items => items;

    // 씬이 로드되기 전에 자동으로 생성됨 → 첫 씬에 배치 안 해도 됨
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("[InventoryManager]");
        go.AddComponent<InventoryManager>();
    }

    private void Awake()
    {
        // 이미 인스턴스가 있으면 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 씬이 바뀌어도 이 오브젝트는 파괴되지 않음
        DontDestroyOnLoad(gameObject);
    }

    // 아이템 추가 (ItemPickup에서 호출)
    public void AddItem(ItemData item)
    {
        items.Add(item);
        Debug.Log($"[인벤토리] {item.itemName} 획득. 보유 수: {items.Count}");
    }

    // 특정 ID의 아이템을 가지고 있는지 확인
    public bool HasItem(string itemId)
    {
        return items.Exists(i => i.itemId == itemId);
    }

    // 현재 보유 아이템 전체 목록 출력 (디버그용)
    public void PrintAll()
    {
        foreach (var item in items)
            Debug.Log($"  - {item.itemName} ({item.itemId})");
    }
}
