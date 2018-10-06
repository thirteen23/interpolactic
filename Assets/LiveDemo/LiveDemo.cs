using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpolactic;

public class LiveDemo : MonoBehaviour
{
    public Transform obj;
    float distance = 10;

    void Start()
    {
        Vector3 start = obj.transform.position;

        new Interpolation(t => obj.transform.position = start + new Vector3(distance * t, 0, 0))
            .Duration(1.3f)
            .PingPong(true)
            .EasingFunction(Mathf.SmoothStep)
            .Build(this)
            .Play();
    }
}
