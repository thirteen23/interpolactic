using UnityEngine;
using Interpolactic;

public class UIToggle : MonoBehaviour {

    [SerializeField]
    CanvasGroup fadableCanvasGroup;

    [SerializeField]
    float fadeDuration;

    bool UIVisible = true;

    void Start()
    {
        UpdateUI();
    }

    public void ToggleUI()
    {
        UIVisible = !UIVisible;

        UpdateUI();
    }

    void UpdateUI()
    {
        float toAlpha = UIVisible ? 1 : 0;
        float fromAlpha = fadableCanvasGroup.alpha;

        fadableCanvasGroup.interactable = UIVisible;

        IPInterpolator anim = new IPInterpolator(t => fadableCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t))
            .WithDuration(fadeDuration)
            .WithEasingFunction(Mathf.SmoothStep)
            .WithRealTime(true);

        if (UIVisible)
            fadableCanvasGroup.gameObject.SetActive(true);
        else
            anim = anim.WithCompletion(() => fadableCanvasGroup.gameObject.SetActive(false));

        anim.Build(this).Play();
    }
}
