using System;
using System.Collections.Generic;

namespace Interpolactic
{
    public partial class IPInterpolator
    {
        public IPRunner BuildMEC()
        {
            return new IPRunnerMEC(this);
        }
    }

    public class IPRunnerMEC: IPRunner
    {
        MEC.CoroutineHandle coroutineHandle;

        public IPRunnerMEC(IPInterpolator interpolator) : base(interpolator)
        {}

        MEC.Segment Segment
        {
            get
            {
                return
                      /*** UNCOMMENT THE FOLLOWING LINE IF MEC PRO IS AVAILABLE ***/
                      interpolator.realTime ? MEC.Segment.RealtimeUpdate :
                                  MEC.Segment.Update;
            }
        }

        protected override float DeltaTime
        {
            get
            {
                return playing ? MEC.Timing.DeltaTime : 0;
            }
        }

        IEnumerator<float> DelayAndExecute
        {
            get
            {
                yield return 0;

                if(interpolator.delayAfterFirstStep)
                    interpolator.PerformStep(0);

                yield return MEC.Timing.WaitForSeconds(interpolator.delay);

                yield return MEC.Timing.WaitUntilDone(RunCoroutine(), Segment);
            }
        }

        public override void Stop()
        {
            base.Stop();

            MEC.Timing.KillCoroutines(coroutineHandle.Tag);
        }

        public override void Play()
        {
            if (!started)
            {
                coroutineHandle = MEC.Timing.RunCoroutine(DelayAndExecute, Segment);
                coroutineHandle.Tag = GetHashCode().ToString();
            }

            base.Play();

            UnityEngine.Debug.Log(coroutineHandle.IsValid);
            MEC.Timing.ResumeCoroutines(coroutineHandle.Tag);
        }
    }
}
