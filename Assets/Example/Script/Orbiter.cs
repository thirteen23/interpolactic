using UnityEngine;
using Interpolactic;

public class Orbiter : MonoBehaviour
{
    [SerializeField]
    GameObject toRotate;

    [SerializeField]
    float duration;

    IPRunner running;

    void Start()
    {
        running = new IPInterpolator(t => toRotate.transform.rotation = Quaternion.Euler(0, 0, t * 360f))
                .WithDuration(duration)
                .WithRepeats(true)
                .BuildMEC();
        
        running.Play();
    }

    public void StartStopOrbit()
    {
        if (running.playing)
            running.Pause();
        else
            running.Play();
    }
}
