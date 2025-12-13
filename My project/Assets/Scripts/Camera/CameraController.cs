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
    private Vector3 shakeOffset;
    private float shakeTimeRemaining;
    private float shakeIntensity;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
        if (target != null)
        {
            lastPlayerX = target.position.x;
        }
        lookAheadOffset = Vector3.zero;
        shakeOffset = Vector3.zero;
        shakeTimeRemaining = 0f;
        shakeIntensity = 0f;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("카메라의 타겟이 설정되지 않았습니다.");
            return;
        }

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        if (shakeTimeRemaining > 0f)
        {
            Vector2 rnd = Random.insideUnitCircle * shakeIntensity;
            shakeOffset = new Vector3(rnd.x, rnd.y, 0f);
            shakeTimeRemaining -= Time.deltaTime;
            if (shakeTimeRemaining <= 0f)
            {
                shakeOffset = Vector3.zero;
                shakeIntensity = 0f;
            }
        }
        transform.position = targetPosition + shakeOffset;
        ApplyBounds();
    }
    public void TriggerShake(float intensity, float duration)
    {
        shakeIntensity = Mathf.Max(shakeIntensity, intensity);
        shakeTimeRemaining = Mathf.Max(shakeTimeRemaining, duration);
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
