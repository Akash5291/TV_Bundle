using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Scale + fade intro for the splash logo. Done in code rather than an
// Animator/AnimationClip since it's a single one-shot effect.
public class LogoSplashAnimation : MonoBehaviour
{
    [SerializeField] Image logoImage;
    [SerializeField] float duration = 0.8f;
    [SerializeField] float startScale = 0.6f;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    void OnEnable()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        var rect = (RectTransform)transform;
        var color = logoImage.color;

        color.a = 0f;
        logoImage.color = color;
        rect.localScale = Vector3.one * startScale;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = easing.Evaluate(Mathf.Clamp01(t / duration));

            rect.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, p);
            color.a = p;
            logoImage.color = color;

            yield return null;
        }

        rect.localScale = Vector3.one;
        color.a = 1f;
        logoImage.color = color;
    }
}
