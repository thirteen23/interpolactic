using UnityEngine;
using Interpolactic;

public class UIToggle : MonoBehaviour {

    public CanvasGroup fadableCanvasGroup;
    public float fadeDuration;

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

        Interpolation anim = new Interpolation(t => fadableCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t))
            .Duration(fadeDuration)
            .EasingFunction(Mathf.SmoothStep)
            .RealTime(true);

        if (UIVisible)
            fadableCanvasGroup.gameObject.SetActive(true);
        else
            anim = anim.Completion(() => fadableCanvasGroup.gameObject.SetActive(false));

        anim.Build(this).Play();
    }
}
