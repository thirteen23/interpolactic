using System;
using System.Collections.Generic;

namespace Interpolactic
{
    public partial class IPRunner
    {
        internal Func<float> DeltaTimeMEC = () => MEC.Timing.DeltaTime;

        /**
         * Compile the IPRunner into a MEC, from "More Effective Coroutines", a
         * third-party library with Coroutines that play nicer with Unity's garbage 
         * collector.
         * 
         * https://assetstore.unity.com/packages/tools/animation/more-effective-coroutines-free-54975
         * 
         * \warning To run an IPRunner as an MEC in real time, MEC Pro must be purchased
         * and the line in the getter for "Segment" must be uncommented.
         **/
        public MEC.CoroutineHandle ExecuteMEC()
        {
            PerformStep(0);

            return MEC.Timing.RunCoroutine(DelayAndExecuteMEC, Segment);
        }

        MEC.Segment Segment
        {
            get
            {
                return 
                    /*** UNCOMMENT THE FOLLOWING LINE IF MEC PRO IS AVAILABLE ***/
                    //realTime ? MEC.Segment.RealtimeUpdate : 
                                  MEC.Segment.Update;
            }
        }

        IEnumerator<float> DelayAndExecuteMEC
        {
            get
            {
                yield return 0;

                PerformStep(0);

                yield return MEC.Timing.WaitForSeconds(delay);

                yield return MEC.Timing.WaitUntilDone(Coroutine(DeltaTimeMEC), Segment);
            }
        }
    }
}
