using System.Collections.Generic;
using UnityEngine;
using System;
/** \mainpage Interpolactic
 * Interpolactic aims to take the pain out of Coroutine-based animations by 
 * bundling the boilerplate of a time-based IEnumerator into a composable, 
 * reusable builder.
 * 
 * Animating a CanvasGroup's alpha from 0 to 1 can be done in just a single line:
 * \code{.cs}
 *  new IPInterpolator(t => canvasGroup.alpha = t).WithDuration(1).Execute();
 * \endcode
 * 
 * The beauty of Interpolactic is that the caller declares the implementation
 * by defining what happens at every time step. While many other plugins 
 * restrict interpolation to common types such as float and Color, Interpolactic
 * simply needs an action to perform with respect to time.
 * 
 * For example, we can use caller-defined interpolation across a string to add a
 * typing effect:
 * \code{.cs}
 * string str = "abcdefg";
 * 
 * new IPInterpolator(t => 
 *   {
 *      int substringLength = (int) t * str.Length;
 *      textField.text = str.Substring(0, substringLength));
 *   }
 * ).WithDuration(1).Execute(this);
 * \endcode
 */
namespace Interpolactic 
{
    /**
     * The IPInterpolator class acts as a builder for a 
     * for Coroutines that act on a callback with a time value from 0 to 1.
     *
     * The IPInterpolator doesn't generate a Coroutine until Execute() is called on it.
     * Therefore, for repeated animations an IPInterpolator can be cached and modified,
     * generating a new Coroutine with every Execute.
     **/
    public partial class IPInterpolator 
    {
        internal Action<float>[] stepActions = new Action<float>[0];
        internal Action<float> onCancel;
        internal Action onComplete;

        internal Func<float, float, float, float> easingFunction = Mathf.Lerp;

        /**
         * Length in seconds of the IPInterpolator interpolation over time.
         * 
         * Defaults to 0.
         **/
        public float duration { get; private set; }

        /**
         * Length in seconds that the IPInterpolator's coroutine will wait before
         * beginning interpolation.
         * 
         * Defaults to 0.
         **/
        public float delay { get; private set; }

        /**
         * If true, the IPInterpolator's interpolation will execute independently of 
         * Time.timeScale.
         * 
         * Defaults to false.
         */
        public bool realTime { get; private set; }

        public bool delayAfterFirstStep { get; private set; }

        public bool repeats { get; private set; }

        /**
         * Create a new IPInterpolator with no actions.
         **/
        public IPInterpolator()
        {
            //Constructor intentionally left empty
        }

        /**
         * Create a new IPInterpolator with a single action. 
         **/
        public IPInterpolator(Action<float> action)
        {
            stepActions = new Action<float>[] { action };
        }

        IPInterpolator(IPInterpolator animationBlock)
        {
            stepActions = animationBlock.stepActions;
            onComplete = animationBlock.onComplete;
            onCancel = animationBlock.onCancel;

            duration = animationBlock.duration;
            delay = animationBlock.delay;
            repeats = animationBlock.repeats;

            easingFunction = animationBlock.easingFunction;

            realTime = animationBlock.realTime;
            delayAfterFirstStep = animationBlock.delayAfterFirstStep;
        }

        /**
         * Create a clone of IPInterpolator object with a specified duration.
         **/
        public IPInterpolator WithDuration(float duration)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.duration = duration;
            return block;
        }

        /**
         * Create a clone of IPInterpolator object with a specified delay before execution.
         **/
        public IPInterpolator WithDelay(float delay)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.delay = delay;
            return block;
        }

        /**
         * Create a clone of IPInterpolator object with a callback to be invoked
         * when the interpolation has run to completion.
         * The completion action will not be called if the interpolation is cancelled.
         **/
        public IPInterpolator WithCompletion(Action onComplete)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.onComplete = onComplete;
            return block;
        }

        public IPInterpolator WithOnCancel(Action<float> onCancel)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.onCancel = onCancel;
            return block;
        }

        /**
         * Create a clone of IPInterpolator instance with an easing function such as
         * Mathf.SmoothStep.
         * \param easingFunction func(float from, float to, float t) —> float
         **/
        public IPInterpolator WithEasingFunction(Func<float, float, float, float> easingFunction)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.easingFunction = easingFunction;
            return block;
        }

        /**
         * Create a clone of IPInterpolator instance with real time enabled or disabled.
         **/
        public IPInterpolator WithRealTime(bool realTime)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.realTime = realTime;
            return block;
        }

        public IPInterpolator WithDelayAfterFirstStep(bool delayAfter)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.delayAfterFirstStep = delayAfter;
            return block;
        }

        public IPInterpolator WithRepeats(bool repeats)
        {
            IPInterpolator block = new IPInterpolator(this);
            block.repeats = repeats;
            return block;
        }

        /**
         * Create a clone of IPInterpolator instance, with an additional action to be called
         * each time step. All existing actions registered to the IPInterpolator will be 
         * preserved.
         **/
        public IPInterpolator WithAction(Action<float> stepAction)
        {
            List<Action<float>> actions = new List<Action<float>>(this.stepActions);
            actions.Add(stepAction);

            IPInterpolator block = new IPInterpolator(this);
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
