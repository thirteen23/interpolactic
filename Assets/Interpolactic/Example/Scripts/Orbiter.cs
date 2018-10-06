using UnityEngine;
using Interpolactic;

public class Orbiter : MonoBehaviour
{
    public GameObject toRotate;
    public float duration;

    Interpolation.Runner running;

    void Start()
    {
        running = new Interpolation(t => toRotate.transform.rotation = Quaternion.Euler(0, 0, t * 360f))
            .Duration(duration)
            .Repeats(true)
            .Build(this);
        
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
