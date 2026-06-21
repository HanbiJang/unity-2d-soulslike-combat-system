using UnityEngine;

// 유니티 메뉴 Assets > Create > Item > ItemData 로 아이템을 만들 수 있게 해줌
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    [Tooltip("코드에서 아이템을 식별할 고유 ID (예: iron_sword)")]
    public string itemId;

    [Tooltip("화면에 표시될 이름")]
    public string itemName;

    [TextArea]
    [Tooltip("아이템 설명")]
    public string description;

    [Tooltip("인벤토리에 표시될 아이콘")]
    public Sprite icon;
}
