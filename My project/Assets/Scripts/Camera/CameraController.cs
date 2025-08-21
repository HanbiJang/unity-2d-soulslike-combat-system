using UnityEngine;

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

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("카메라의 타겟이 설정되지 않았습니다.");
            return;
        }

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = targetPosition;
        ApplyBounds();
    }

    private void ApplyBounds()
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + camWidth, maxBounds.x - camWidth);
        float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + camHeight, maxBounds.y - camHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector2 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}