using System.Collections.Generic;
using UnityEngine;
using System;
/** \mainpage Interpolactic
 * <b>BOILERPLATE, BEGONE!</b>
 * 
 * Interpolactic takes the pain out of Coroutine-based animations by 
 * bundling the boilerplate of a time-based IEnumerator into a composable, 
 * reusable builder.
 * 
 * All Interpolactic needs is a function or closure that acts upon a time value
 * ranging from 0 to 1. 
 * 
 * Fading in a CanvasGroup's alpha can be done in just a few keystrokes:
 * \code{.cs}
 *   new Interpolation(t => canvasGroup.alpha = t) //Set alpha to t at each time step
 *     .Duration(1) //1 second
 *     .Build(this) //Build a runner for this interpolation using this Monobehaviour
 *     .Play(); //Fire!
 * \endcode
 * 
 * Animating a transform's position can be done similarly:
 * \code{.cs}
 *   Vector3 start = transform.position;
 *   Vector3 movement = target.transform.position - start;
 * 
 *   new Interpolation(t => transform.position = start + movement * t)
 *     .Duration(1)
 *     .Build(this)
 *     .Play();
 * \endcode
 * 
 * 
 * <b>ANIMATE ANYTHING</b>
 * 
 * The beauty of Interpolactic is that the caller declares the implementation
 * by defining what happens at every time step. While many other plugins 
 * restrict interpolation to common types such as float and Color, Interpolactic
 * simply needs an action to perform with respect to time.
 * 
 * For example, we can use caller-defined interpolation across a string to add a
 * "typing" effect:
 * \code{.cs}
 * string str = "abcdefg";
 * 
 * new Interpolation(t => 
 *   {
 *      int substringLength = (int) t * str.Length;
 *      textField.text = str.Substring(0, substringLength));
 *   }
 * ).Duration(1).Build(this).Play();
 * \endcode
 * 
 * 
 * <b>PAIN-FREE PLAYBACK</b>
 * 
 * The actual Coroutine is then wrapped in a Runner object
 * that offers a kit of utility methods to control playback.
 * \code{.cs}
 * void TogglePlaying()
 * {
 *      if(animation.playing)
 *          animation.Pause();
 *      else
 *          animation.Play();
 *       
 * }
 * \endcode
 */
namespace Interpolactic 
{
    /**
     * The IPInterpolation class acts as a template for Coroutines
     * to act on a callback with a time value from 0 to 1.
     *
     * The IPInterpolation doesn't generate a IPRunner until Build() is called on it.
     * Therefore, for repeated animations an IPInterpolation can be kept and tweaked,
     * generating a new IPRunner with every Build.
     **/
    public partial class Interpolation 
    {
        internal Action<float>[] stepActions = new Action<float>[0];
        internal Action<float> onStop;
        internal Action onComplete;

        /**
         * Function that scales the time value in interpolation between 0 and 1,
         * such as Mathf.SmoothStep.
         * 
         * Defaults to Mathf.Lerp.
         **/
        internal Func<float, float, float, float> easingFunction = Mathf.Lerp;

        /**
         * Length in seconds of the Interpolation over time.
         * 
         * Defaults to 0.
         **/
        public float duration { get; private set; }

        /**
         * Length in seconds that the Interpolation's runner will wait before
         * beginning interpolation.
         * 
         * Defaults to 0.
         **/
        public float delay { get; private set; }

        /**
         * If true, the interpolation will execute independently of 
         * Time.timeScale.
         * 
         * Defaults to false.
         */
        public bool realTime { get; private set; }

        /**
         * If true, the Interpolation's step at t=0 will be called
         * before the delay is applied. Useful for holding an animation
         * at its initial state while waiting for the delay to end.
         * 
         * Defaults to false.
         */
        public bool firstStepBeforeDelay { get; private set; }

        /**
         * If true, the associated Runner will repeat until stopped.
         * 
         * Defaults to false.
         */
        public bool repeats { get; private set; }

        public bool pingPong { get; private set; }

        /**
         * Create a new Interpolator with no actions.
         **/
        public Interpolation()
        {
            //Constructor empty
        }

        /**
         * Convenience initializer for a new Interpolator with a single action. 
         **/
        public Interpolation(Action<float> action)
        {
            stepActions = new Action<float>[] { action };
        }

        Interpolation(Interpolation animationBlock)
        {
            stepActions = animationBlock.stepActions;
            onComplete = animationBlock.onComplete;
            onStop = animationBlock.onStop;

            duration = animationBlock.duration;
            delay = animationBlock.delay;
            repeats = animationBlock.repeats;
            pingPong = animationBlock.pingPong;

            easingFunction = animationBlock.easingFunction;

            realTime = animationBlock.realTime;
            firstStepBeforeDelay = animationBlock.firstStepBeforeDelay;
        }

        /**
         * Create a clone of Interpolation object with a specified duration.
         * \param duration Duration of interpolation in seconds.
         **/
        public Interpolation Duration(float duration)
        {
            Interpolation block = new Interpolation(this);
            block.duration = duration;
            return block;
        }

        /**
         * Create a clone of Interpolation object with a specified delay before execution.
         * \param delay Delay of interpolation in seconds.
         **/
        public Interpolation Delay(float delay)
        {
            Interpolation block = new Interpolation(this);
            block.delay = delay;
            return block;
        }

        /**
         * Create a clone of Interpolation object with a callback to be invoked
         * when the interpolation has run to completion.
         * The completion action will not be called if the interpolation is stopped manually.
         * \param onComplete Action to be called when interpolation completes.
         **/
        public Interpolation Completion(Action onComplete)
        {
            Interpolation block = new Interpolation(this);
            block.onComplete = onComplete;
            return block;
        }

        /**
         * Create a clone of Interpolation object will a callback to be invoked
         * when the interpolation has been stopped manually.
         * \param onStop Action to be called if interpolation is stopped.
         **/
        public Interpolation OnStop(Action<float> onStop)
        {
            Interpolation block = new Interpolation(this);
            block.onStop = onStop;
            return block;
        }

        /**
         * Create a clone of Interpolation object with an easing function.
         * \param easingFunction Easing function, such as <tt>Mathf.SmoothStep</tt>
         **/
        public Interpolation EasingFunction(Func<float, float, float, float> easingFunction)
        {
            Interpolation block = new Interpolation(this);
            block.easingFunction = easingFunction;
            return block;
        }

        /**
         * Create a clone of Interpolation ojbect with real time enabled or disabled.
         * \param realTime Should the interpolation run in real time?
         **/
        public Interpolation RealTime(bool realTime)
        {
            Interpolation block = new Interpolation(this);
            block.realTime = realTime;
            return block;
        }

        /**
         * Create a clone of Interpolation object with firstStepBeforeDelay enabled or disabled.
         * \param firstStepBeforeDelay Should the interpolation act on t=0 before performing the delay?
         **/
        public Interpolation FirstStepBeforeDelay(bool firstStepBeforeDelay)
        {
            Interpolation block = new Interpolation(this);
            block.firstStepBeforeDelay = firstStepBeforeDelay;
            return block;
        }

        /**
         * Create a clone of Interpolation with repeating enabled or disabled.
         * \param repeats Should the interpolation repeat?
         **/
        public Interpolation Repeats(bool repeats)
        {
            Interpolation block = new Interpolation(this);
            block.repeats = repeats;
            return block;
        }

        public Interpolation PingPong(bool pingPong)
        {
            Interpolation block = new Interpolation(this);
            block.pingPong = pingPong;
            block.repeats = true;
            return block;
        }

        /**
         * Create a clone of Interpolation object, with an additional action to be called
         * each time step. All existing actions registered to the Interpolation will be 
         * preserved.
         * \param stepAction Action to be called at each time step in the interpolation.
         **/
        public Interpolation AddAction(Action<float> stepAction)
        {
            List<Action<float>> actions = new List<Action<float>>(this.stepActions);
            actions.Add(stepAction);

            Interpolation block = new Interpolation(this);
            block.stepActions = actions.ToArray();

            return block;
        }

        internal void PerformStep(float val)
        {
            for (int i = 0; i < stepActions.Length; i++)
                stepActions[i](val);
        }
    }
}
