using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpolactic
{
    public abstract class IPRunner
    {
        protected abstract float DeltaTime { get; }
        protected abstract IEnumerator DelayAndExecute { get; }

        public bool finished { get; private set; }
        public bool started { get; private set; }
        public bool playing { get; private set; }

        protected IPInterpolator interpolator;
        protected MonoBehaviour monoBehaviour;

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

        public IPRunner(IPInterpolator interpolator)
        {
            this.interpolator = interpolator;
        }

        public virtual void Stop()
        {  
            playing = false;

            if (!finished && interpolator.onCancel != null)
                interpolator.onCancel(normalizedScaledTime);
        }

        public virtual void Play()
        {
            if (!started)
                Start();

            playing = true;
        }

        public virtual void Pause()
        {
            playing = false;
        }

        internal void Start()
        {
            started = true;

            if (interpolator.delayAfterFirstStep)
                interpolator.PerformStep(0);
        }

        internal IEnumerator<float> RunCoroutine()
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
