using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpolactic
{
    public partial class IPInterpolator
    {
        /**
         * Builds the IPInterpolator's Coroutine and starts it with the specified MonoBehaviour.
         **/
        //public Coroutine Execute(MonoBehaviour monoBehaviour)
        //{
        //    if (delayAfterFirstStep)
        //        PerformStep(0);

        //    return monoBehaviour.StartCoroutine(DelayAndExecute);
        //}

        internal Func<float> DeltaTimeCoroutine
        {
            get
            {
                return () => realTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        internal IEnumerator DelayAndExecute
        {
            get
            {
                yield return 0;

                if (delayAfterFirstStep)
                    PerformStep(0);

                if (realTime)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);

                yield return Coroutine(DeltaTimeCoroutine);
            }
        }

        internal IEnumerator<float> Coroutine(Func<float> deltaTimeFunction)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                PerformStep(easingFunction(0, 1, elapsed / duration));

                elapsed += deltaTimeFunction();

                yield return 0;
            }

            PerformStep(1);

            if (onComplete != null)
                onComplete();
        }
    }
}
