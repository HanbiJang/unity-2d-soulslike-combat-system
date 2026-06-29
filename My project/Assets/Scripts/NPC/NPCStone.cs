using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStone : MonoBehaviour
{
    [Header("대화 데이터")]
    [Tooltip("아이템 체크 대화 완료 후, 실행될 대화")]
    [SerializeField] private DialogueData dialogueData;

    [Header("이동 제한 콜라이더")]
    [SerializeField] private BoxCollider2D collider_;

    private void Start()
    {
    }

    public void DestroyStone()
    {
        if (DialogueSystem.Instance == null || dialogueData == null) return;

        DialogueSystem.Instance.StartDialogue(dialogueData);

        dialogueData = null; //1회 적용

        //석상 이동 제한 없어지기
        Destroy(collider_);
    }
}
