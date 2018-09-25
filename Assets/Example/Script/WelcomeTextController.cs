using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interpolactic;

[ExecuteInEditMode]
public class WelcomeTextController : MonoBehaviour
{
    [SerializeField]
    Text text;

    [SerializeField]
    float typingDuration;

    [SerializeField]
    [TextArea(10, 10)]
    string welcomeText;

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
        text.text = "";

        new IPRunner(t => text.text = str.Substring(0, (int)(t * str.Length)))
            .WithDuration(typingDuration)
            .WithRealTime(true)
            .Execute(this);
    }
}
