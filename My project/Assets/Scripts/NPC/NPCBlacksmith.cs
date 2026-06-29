using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBlacksmith : MonoBehaviour
{
    [Header("대화 데이터")]
    [Tooltip("아이템 체크 대화 완료 후, 실행될 대화")]
    [SerializeField] private DialogueData dialogueData;

    [Header("보상 아이템")]
    [Tooltip("대화 완료 후, 받을 보상")]
    [SerializeField] private ItemData itemData;

    [Header("애니메이션")]
    [Tooltip("재생할 애니메이션 이름")]
    [SerializeField] private string upgradeAnimationName = "NPC1_Nomal";

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void UpgradeWeapon()
    {
        if (DialogueSystem.Instance == null || dialogueData == null) return;

        DialogueSystem.Instance.StartDialogue(dialogueData);

        //보상 아이템 습득
        InventoryManager.Instance?.AddItem(itemData);
        ItemPickupUI.Instance?.Show(itemData.itemName);

        //대장장이 애니메이션 재생
        anim?.Play(upgradeAnimationName);

        dialogueData = null; //1회 적용
    }
}
