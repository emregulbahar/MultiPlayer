using System.Collections;
using UnityEngine;

public class InteractAnimation : MonoBehaviour
{
    [SerializeField] private Transform _resourceVisual;
    [SerializeField] private float _shakeScale = 1.2f;
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _returnDuration = 0.3f;

    private Vector3 _originScale;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _originScale = _resourceVisual.localScale;
    }

    public void Shake()
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 targetScale = _originScale * _shakeScale;
        float elapsed = 0f;

        // Scale up
        while (elapsed < _shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _shakeDuration;
            _resourceVisual.localScale = Vector3.Lerp(_originScale, targetScale, t);
            yield return null;
        }

        _resourceVisual.localScale = targetScale;
        elapsed = 0f;

        // Scale back
        while (elapsed < _returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _returnDuration;
            // EaseOut: fast at start, slow at end
            t = 1f - Mathf.Pow(1f - t, 3f);
            _resourceVisual.localScale = Vector3.Lerp(targetScale, _originScale, t);
            yield return null;
        }

        _resourceVisual.localScale = _originScale;
        _shakeCoroutine = null;
    }
}
