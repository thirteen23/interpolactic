using System.Collections.Generic;
using UnityEngine;

namespace Interpolactic
{
    public partial class Interpolation
    {
        public abstract class Runner
        {
            /**
             * The interval in seconds since the last frame.
             **/
            protected abstract float DeltaTime(bool realTime);

            /**
             * Whether the Runner has been allowed to run to completion.
             * This will always be "false" for runners of repeating Interpolations.
             **/
            public bool finished { get; private set; }

            /**
             * Whether the Runner is currently animating.
             **/
            public bool playing { get; private set; }

            /**
             * Whether the Runner was stopped manually via Stop().
             **/
            public bool stopped { get; private set; }

            /**
             * Whether the Runner has begun to play.
             **/
            public bool started { get; private set; }

            /**
             * The Interpolation model for the Runner's behavior.
             **/
            protected Interpolation interpolation;

            float elapsedTime;

            float normalizedScaledTime
            {
                get
                {
                    //Use modulus of elapsed time in case of repeats
                    float modulus = Mathf.Repeat(elapsedTime, interpolation.duration);
                    return interpolation.easingFunction(0, 1, modulus / interpolation.duration);
                }
            }

            internal Runner(Interpolation interpolation)
            {
                this.interpolation = interpolation;
            }

            /**
             * Stops the Runner and frees all of its resources.
             * 
             * Calls the interpolation's onStop action, if defined.
             **/
            public virtual void Stop()
            {
                playing = false;
                stopped = true;

                if (!finished && interpolation.onStop != null)
                    interpolation.onStop(normalizedScaledTime);
            }

            /**
             * Begins or resumes playback on the Runner.
             * 
             * \warning Will throw an exception if the Runner has already been stopped.
             **/
            public virtual void Play()
            {
                if (stopped)
                    throw new UnityException("Cannot play a stopped " + GetType());

                if (!started)
                    Start();

                playing = true;
            }

            /**
             * Suspends playback on the Runner. Resources are still allocated in a paused
             * Runner, so be sure to only call Pause() if planning on resuming the
             * animation.
             **/
            public virtual void Pause()
            {
                playing = false;
            }

            void Start()
            {
                started = true;

                if (interpolation.firstStepBeforeDelay)
                    interpolation.PerformStep(0);
            }

            /**
             * IEnumerator for the actual interpolation of t from 0 to 1. Will perform
             * the Interpolation in its entirety then call the onComplete callback,
             * if defined.
             **/
            protected IEnumerator<float> PerformInterpolation()
            {
                while (interpolation.repeats || elapsedTime < interpolation.duration)
                {
                    interpolation.PerformStep(normalizedScaledTime);

                    elapsedTime += DeltaTime(interpolation.realTime);

                    yield return 0;
                }

                interpolation.PerformStep(1);

                finished = true;

                if (interpolation.onComplete != null)
                    interpolation.onComplete();
            }
        }
    }
}
