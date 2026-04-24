using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SlowMotionEffects : MonoBehaviour
{
    public static SlowMotionEffects Instance { get; private set; }

    [Header("카메라 줌 설정")]
    [SerializeField] private float zoomAmount = 1.5f;
    [SerializeField] private float zoomSpeed = 8f;

    [Header("포스트 프로세스 설정")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private float targetVignetteIntensity = 0.55f;
    [SerializeField] private float vignetteSpeed = 8f;

    private Camera mainCam;
    private float originalOrthoSize;
    private float originalVignetteIntensity;
    private Vignette vignette;
    private float targetOrthoSize;
    private float targetVignette;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        mainCam = Camera.main;
        if (mainCam != null)
        {
            originalOrthoSize = mainCam.orthographicSize;
            targetOrthoSize = originalOrthoSize;
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out vignette))
        {
            originalVignetteIntensity = vignette.intensity.value;
            targetVignette = originalVignetteIntensity;
        }
    }

    public void SetSlowMotion(bool active)
    {
        if (active)
        {
            targetOrthoSize = originalOrthoSize - zoomAmount;
            targetVignette = targetVignetteIntensity;
        }
        else
        {
            targetOrthoSize = originalOrthoSize;
            targetVignette = originalVignetteIntensity;
        }
    }

    private void LateUpdate()
    {
        if (mainCam != null)
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targetOrthoSize, Time.unscaledDeltaTime * zoomSpeed);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, Time.unscaledDeltaTime * vignetteSpeed);
    }
}
