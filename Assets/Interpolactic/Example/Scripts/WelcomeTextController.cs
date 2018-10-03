using UnityEngine;
using UnityEngine.UI;
using Interpolactic;

[ExecuteInEditMode]
public class WelcomeTextController : MonoBehaviour
{
    public Text text;
    public float typingDuration;

    [TextArea(10, 10)]
    public string welcomeText;

    void OnEnable()
    {
        if (Application.isPlaying)
            ShowText(welcomeText);
    }

    void OnGUI()
    {
        if (!Application.isPlaying)
            text.text = welcomeText;
    }

    void ShowText(string str)
    {
        new Interpolation(t => text.text = str.Substring(0, (int)(t * str.Length)))
            .Duration(typingDuration)
            .RealTime(true)
            .Delay(0.5f)
            .FirstStepBeforeDelay(true)
            .Build(this)
            .Play();
    }
}
