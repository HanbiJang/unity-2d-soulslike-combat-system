using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Tooltip("대화를 하는 캐릭터 이름")]
    public string speakerName = "플레이어";
    
    [Tooltip("대사 내용")]
    [TextArea(2, 5)]
    public string dialogueText = "";
    
    [Tooltip("말풍선이 표시될 위치 (플레이어 또는 적)")]
    public DialogueTarget target = DialogueTarget.Player;
}

public enum DialogueTarget
{
    Player,
    Enemy
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    [Header("대화 설정")]
    [Tooltip("대화가 자동으로 진행되는지 (false면 클릭/키 입력 필요)")]
    public bool autoPlay = false;

    [Tooltip("자동 진행 시 각 대사 표시 시간")]
    public float autoPlayDelay = 3f;

/*    [Header("화면 페이드 설정")]
    [Tooltip("대화 시작/종료 시 화면 페이드 사용 여부")]
    public bool useScreenFade = true;*/

    [Tooltip("화면 페이드 지속 시간(초)")]
    public float screenFadeDuration = 0.8f;

    [Tooltip("대화 목록")]
    public DialogueLine[] dialogueLines = new DialogueLine[0];
}


