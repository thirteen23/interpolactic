using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpolactic
{
    public partial class IPInterpolator
    {
        public IPRunner Execute(MonoBehaviour monoBehavior)
        {
            IPRunner runner = new IPRunner(this, monoBehavior);

            runner.Play();

            return runner;
        }
    }

    public class IPRunner
    {
        public bool finished { get; private set; }
        public bool started { get; private set; }
        public bool playing { get; private set; }

        IPInterpolator interpolator;
        MonoBehaviour monoBehaviour;

        Coroutine coroutine;

        float elapsedTime;

        float normalizedScaledTime
        {
            get
            {
                //Use modulus of duration in case of repeats
                float modulus = Mathf.Repeat(elapsedTime, interpolator.duration);
                return interpolator.easingFunction(0, 1, modulus / interpolator.duration);
            }
        }

        public IPRunner(IPInterpolator interpolator, MonoBehaviour monoBehaviour)
        {
            this.interpolator = interpolator;
            this.monoBehaviour = monoBehaviour;
        }

        public void Stop()
        {
            if (coroutine != null)
                monoBehaviour.StopCoroutine(coroutine);
            
            if (!finished && interpolator.onCancel != null)
                interpolator.onCancel(normalizedScaledTime);

            playing = false;
            coroutine = null;
        }

        public void Play()
        {
            if (!started)
                Start();

            playing = true;
            
            coroutine = monoBehaviour.StartCoroutine(DelayAndExecute);
        }

        public void Pause()
        {
            monoBehaviour.StopCoroutine(coroutine);

            playing = false;
            coroutine = null;
        }

        internal void Start()
        {
            started = true;

            if (interpolator.delayAfterFirstStep)
                interpolator.PerformStep(0);
        }

        internal float DeltaTime
        {
            get
            {
                return interpolator.realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        internal IEnumerator DelayAndExecute
        {
            get
            {
                if (interpolator.delay > 0)
                {
                    yield return 0;

                    if (interpolator.delayAfterFirstStep)
                        interpolator.PerformStep(0);

                    if (interpolator.realTime)
                        yield return new WaitForSecondsRealtime(interpolator.delay);
                    else
                        yield return new WaitForSeconds(interpolator.delay);
                }

                yield return this.Coroutine();
            }
        }

        internal IEnumerator<float> Coroutine()
        {                
            while (interpolator.repeats || elapsedTime < interpolator.duration)
            {
                interpolator.PerformStep(normalizedScaledTime);

                elapsedTime += DeltaTime;

                yield return 0;
            }

            interpolator.PerformStep(1);

            finished = true;

            if (interpolator.onComplete != null)
                interpolator.onComplete();
        }
    }
}
