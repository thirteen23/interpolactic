using UnityEngine;
using Interpolactic;

public class Orbiter : MonoBehaviour
{
    [SerializeField]
    GameObject toRotate;

    [SerializeField]
    float duration;

    Coroutine running;

    public void PerformOrbit()
    {
        if (running != null)
            StopCoroutine(running);

        running = new IPRunner(t => toRotate.transform.rotation = Quaternion.Euler(0, 0, t * 360))
            .WithDuration(duration)
            .WithEasingFunction(Mathf.SmoothStep)
            .WithCompletion(LogCompleted)
            .Execute(this);
    }

    void LogCompleted()
    {
        Debug.Log(gameObject.name + " finished orbit.");
    }
}
