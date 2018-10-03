using System.Collections.Generic;
using UnityEngine;
using Interpolactic;

public class RocketAnimator : MonoBehaviour
{
    public GameObject[] rocketObjects;
    public float flightDuration, flightDistance, flightStagger;

    Interpolation rocketAnimation;

    List<Interpolation.Runner> running = new List<Interpolation.Runner>();

    void Start()
    {
        //Can initialize an IPInterpolator once and compose it further before executing
        rocketAnimation = new Interpolation()
            .Duration(flightDuration)
            .EasingFunction(Mathf.SmoothStep);
    }

    public void Launch()
    {
        running.ForEach(runner => runner.Stop());
        running.Clear();

        for (int idx = 0; idx < rocketObjects.Length; idx++)
        {
            Transform rocket = rocketObjects[idx].transform;

            running.Add(
                rocketAnimation
                    .AddAction(t => SetY(rocket, t * flightDistance))
                    .Completion(() => SetY(rocket, 0))
                    .Delay(flightStagger * idx)
                    .FirstStepBeforeDelay(true)
                    .Build(this)
            );
        }

        running.ForEach(runner => runner.Play());
    }

    void SetY(Transform t, float y)
    {
        t.localPosition = new Vector3(t.position.x, y, t.position.z);
    }
}
