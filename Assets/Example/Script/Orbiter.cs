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
                //.WithEasingFunction(Mathf.SmoothStep)
                //    .WithCompletion(() => {
                //    PerformOrbit(offset);
                //})
                //.WithOnCancel(progress => PerformOrbit(progress + offset))
                .Execute(this);
    }

    public void PerformOrbit(float offset = 0)
    {
        Debug.Log("Starting orbit with offset " + offset);

        if (running != null && !running.finished)
            running.Stop();
        else
        {
            running = new IPInterpolator(t => toRotate.transform.rotation = Quaternion.Euler(0, 0, t * 360f + offset * 360f))
                .WithDuration(duration)
                .WithRepeats(true)
                //.WithEasingFunction(Mathf.SmoothStep)
                //    .WithCompletion(() => {
                //    PerformOrbit(offset);
                //})
                //.WithOnCancel(progress => PerformOrbit(progress + offset))
                .Execute(this);
        }
    }

    public void StartStopOrbit()
    {
        if (running.playing)
        {
            running.Pause();
        }
        else
        {
            running.Play();
        }
    }
    void LogCompleted()
    {
        Debug.Log(gameObject.name + " finished orbit.");
    }
}
