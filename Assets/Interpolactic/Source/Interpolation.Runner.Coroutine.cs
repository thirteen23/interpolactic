using System.Collections;
using UnityEngine;

namespace Interpolactic
{
    public partial class Interpolation
    {
        /**
         * Creates a Runner object for the Interpolation.
         * 
         * \param monoBehaviour The MonoBehaviour object to be used as the starter
         * and stopper for the Runner's internal Coroutine.
         **/
        public Runner Build(MonoBehaviour monoBehaviour)
        {
            return new CoroutineRunner(this, monoBehaviour);
        }

        class CoroutineRunner : Runner
        {
            Coroutine coroutine;
            MonoBehaviour monoBehaviour;

            public CoroutineRunner(Interpolation interpolator, MonoBehaviour monoBehaviour) : base(interpolator)
            {
                this.monoBehaviour = monoBehaviour;
            }

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

            protected override float DeltaTime(bool realTime)
            {
                return realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            IEnumerator DelayAndExecute
            {
                get
                {
                    if (interpolation.delay > 0)
                    {
                        yield return 0;

                        if (interpolation.firstStepBeforeDelay)
                            interpolation.PerformStep(0);

                        if (interpolation.realTime)
                            yield return new WaitForSecondsRealtime(interpolation.delay);
                        else
                            yield return new WaitForSeconds(interpolation.delay);
                    }

                    yield return PerformInterpolation();
                }
            }
        }
    }
}
