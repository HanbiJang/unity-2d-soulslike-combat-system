using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LocationNameUI : MonoBehaviour
{
    [SerializeField] private Text locationText;
    [SerializeField] private float fadeInDuration  = 0.8f;
    [SerializeField] private float holdDuration    = 3f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    private Coroutine _showCoroutine;

    private void Awake()
    {
        if (locationText == null) locationText = GetComponentInChildren<Text>();
        SetAlpha(0f);
    }

    public void Show(string locationName)
    {
        if (_showCoroutine != null) StopCoroutine(_showCoroutine);
        locationText.text = locationName;
        _showCoroutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return StartCoroutine(FadeTo(1f, fadeInDuration));
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeTo(0f, fadeOutDuration));
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start   = locationText.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, target, elapsed / duration));
            yield return null;
        }

        SetAlpha(target);
    }

    private void SetAlpha(float a)
    {
        Color c = locationText.color;
        c.a = a;
        locationText.color = c;
    }
}
