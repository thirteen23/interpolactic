using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpolactic;

public class StarHover : MonoBehaviour {

    public float distance;
    public float duration;

	void Start () {
        Vector3 initalPosition = transform.position + transform.up * distance / 2;
        Vector3 driftTransformation = -transform.up * distance;

        new Interpolation(t => transform.position = initalPosition + driftTransformation * t)
            .Duration(duration)
            .EasingFunction(Mathf.SmoothStep)
            .Repeats(true)
            .PingPong(true)
            .Build(this)
            .Play();
	}
}
