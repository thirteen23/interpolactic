using UnityEngine;
using Interpolactic;

public class StarColorChanger : MonoBehaviour
{
    [SerializeField]
    Color[] colors;

    [SerializeField]
    ParticleSystem particles;

    [SerializeField]
    float transitionDuration;

    IPRunner running;

    int colorIndex;

    const string colorKey = "_TintColor";

    void Start()
    {
        UpdateColor(false);
    }

    public void CycleColor()
    {
        colorIndex = (colorIndex + 1) % colors.Length;

        UpdateColor(true);
    }

    void UpdateColor(bool animated)
    {
        Renderer particleRenderer = particles.GetComponent<Renderer>();

        Color from = particleRenderer.material.GetColor(colorKey);
        Color to = colors[colorIndex];

        System.Action<float> step = t =>
            particleRenderer.material.SetColor(colorKey, Color.Lerp(from, to, t));

        if (running != null)
            running.Stop();

        running = new IPInterpolator(step)
            .WithDuration(animated ? transitionDuration : 0)
            .Build(this);

        running.Play();
    }
}
