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
            IPRunner runner = new IPRunnerCoroutine(this, monoBehavior);

            runner.Play();

            return runner;
        }
    }

    public class IPRunnerCoroutine: IPRunner
    {
        Coroutine coroutine;

        public IPRunnerCoroutine(IPInterpolator interpolator, MonoBehaviour monoBehaviour) : base(interpolator)
        {
            this.monoBehaviour = monoBehaviour;   
        }

        protected override float DeltaTime { get { return interpolator.realTime ? Time.unscaledDeltaTime : Time.deltaTime; } }

        public override void Play()
        {
            base.Play();

            coroutine = monoBehaviour.StartCoroutine(DelayAndExecute);
        }

        public override void Pause()
        {
            base.Pause();

            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        public override void Stop()
        {
            base.Stop();

            if (coroutine != null)
                monoBehaviour.StopCoroutine(coroutine);

            coroutine = null;
        }

        protected override IEnumerator DelayAndExecute
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

                yield return RunCoroutine();
            }
        }
    }
}
