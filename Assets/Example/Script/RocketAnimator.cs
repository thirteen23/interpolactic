using System.Collections.Generic;
using UnityEngine;
using Interpolactic;

public class RocketAnimator : MonoBehaviour
{
    [SerializeField]
    GameObject[] rocketObjects;

    [SerializeField]
    float flightDuration, flightDistance, flightStagger;

    IPRunner rocketAnimation;

    List<Coroutine> running = new List<Coroutine>();

    void Start()
    {
        //Can initialize an IPRunner once and compose it further before executing
        rocketAnimation = new IPRunner()
            .WithDuration(flightDuration)
            .WithEasingFunction(Mathf.SmoothStep);
    }

    public void Launch()
    {
        running.ForEach(coroutine => StopCoroutine(coroutine));
        running.Clear();

        for (int idx = 0; idx < rocketObjects.Length; idx++)
        {
            Transform rocket = rocketObjects[idx].transform;

            running.Add(
                rocketAnimation
                    .WithAction(t => SetY(rocket, t * flightDistance))
                    .WithCompletion(() => SetY(rocket, 0))
                    .WithDelay(flightStagger * idx)
                    .Execute(this)
            );
        }
    }

    void SetY(Transform t, float y)
    {
        t.localPosition = new Vector3(t.position.x, y, t.position.z);
    }
}
