using UnityEngine;

/// <summary>
/// 카메라 무빙 설정 클래스
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Tracking")]
    [Tooltip("카메라가 따라갈 대상입니다.")]
    [SerializeField] private Transform target;
    [Tooltip("카메라가 타겟을 따라가는 속도입니다. 작을수록 빠르게 반응합니다.")]
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Look Ahead")]
    [Tooltip("플레이어의 이동 방향으로 얼마나 더 앞을 비출지 결정합니다.")]
    [SerializeField] private float lookAheadDistance = 2f;
    [Tooltip("전방 주시 지점까지 카메라가 이동하는 데 걸리는 시간입니다.")]
    [SerializeField] private float lookAheadSmoothTime = 0.5f;

    [Header("Camera Bounds")]
    [Tooltip("카메라가 이동할 수 있는 최소 좌표(좌측, 하단)입니다.")]
    [SerializeField] private Vector2 minBounds;
    [Tooltip("카메라가 이동할 수 있는 최대 좌표(우측, 상단)입니다.")]
    [SerializeField] private Vector2 maxBounds;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lookAheadVelocity = Vector3.zero;
    private Vector3 lookAheadOffset;
    private float lastPlayerX;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
        if (target != null)
        {
            lastPlayerX = target.position.x;
        }
        lookAheadOffset = Vector3.zero;
    }

    // 모든 게임 로직이 실행된 후 카메라 위치를 업데이트
    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("카메라의 타겟이 설정되지 않았습니다.");
            return;
        }

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = targetPosition; // SmoothDamp 없이 바로 위치 지정

        ApplyBounds();
    }

    /// <summary>
    /// 계산된 카메라 위치를 설정된 경계 내에 가두는 함수
    /// </summary>
    private void ApplyBounds()
    {
        // 카메라 영역의 절반 크기를 계산
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        // Mathf.Clamp를 사용
        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + camWidth, maxBounds.x - camWidth);
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + camHeight, maxBounds.y - camHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    /// <summary>
    /// 유니티 에디터의 씬(Scene) 뷰에서 경계를 시각적으로 보임
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); // 반투명 빨간색
        Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector2 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}