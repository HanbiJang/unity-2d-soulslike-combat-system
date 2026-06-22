using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBlacksmith : MonoBehaviour
{
    [Header("대화 데이터")]
    [Tooltip("대화 완료 후, 실행될 함수")]
    [SerializeField] private DialogueData dialogueData;

    [Header("보상 아이템")]
    [Tooltip("대화 완료 후, 받을 보상")]
    [SerializeField] private ItemData itemData;

    public void UpgradeWeapon()
    {
        if (DialogueSystem.Instance == null || dialogueData == null) return;

        DialogueSystem.Instance.StartDialogue(dialogueData);

        //보상 아이템 습득
        InventoryManager.Instance?.AddItem(itemData);
        ItemPickupUI.Instance?.Show(itemData.itemName);

        dialogueData = null; //1회 적용
    }
}
