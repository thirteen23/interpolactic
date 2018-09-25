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
 *  new IPRunner(t => canvasGroup.alpha = t).WithDuration(1).Execute();
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
 * new IPRunner(t => 
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
     * The IPRunner class acts as a builder for a 
     * for Coroutines that act on a callback with a time value from 0 to 1.
     *
     * The IPRunner doesn't generate a Coroutine until Execute() is called on it.
     * Therefore, for repeated animations an IPRunner can be cached and modified,
     * generating a new Coroutine with every Execute.
     **/
    public partial class IPRunner 
    {
        Action<float>[] stepActions = new Action<float>[0];
        Action onComplete;

        Func<float, float, float, float> easingFunction = Mathf.Lerp;

        /**
         * Length in seconds of the IPRunner interpolation over time.
         * 
         * Defaults to 0.
         **/
        public float duration { get; private set; }

        /**
         * Length in seconds that the IPRunner's coroutine will wait before
         * beginning interpolation.
         * 
         * Defaults to 0.
         **/
        public float delay { get; private set; }

        /**
         * If true, the IPRunner's interpolation will execute independently of 
         * Time.timeScale.
         * 
         * Defaults to false.
         */
        public bool realTime { get; private set; }

        public bool delayAfterFirstStep { get; private set; }

        /**
         * Create a new IPRunner with no actions.
         **/
        public IPRunner()
        {
            //Constructor intentionally left empty
        }

        /**
         * Create a new IPRunner with a single action. 
         **/
        public IPRunner(Action<float> action)
        {
            stepActions = new Action<float>[] { action };
        }

        IPRunner(IPRunner animationBlock)
        {
            stepActions = animationBlock.stepActions;
            onComplete = animationBlock.onComplete;

            duration = animationBlock.duration;
            delay = animationBlock.delay;

            easingFunction = animationBlock.easingFunction;

            realTime = animationBlock.realTime;
            delayAfterFirstStep = animationBlock.delayAfterFirstStep;
        }

        /**
         * Create a clone of IPRunner object with a specified duration.
         **/
        public IPRunner WithDuration(float duration)
        {
            IPRunner block = new IPRunner(this);
            block.duration = duration;
            return block;
        }

        /**
         * Create a clone of IPRunner object with a specified delay before execution.
         **/
        public IPRunner WithDelay(float delay)
        {
            IPRunner block = new IPRunner(this);
            block.delay = delay;
            return block;
        }

        /**
         * Create a clone of IPRunner object with a callback to be invoked
         * when the interpolation has run to completion.
         * The completion action will not be called if the interpolation is cancelled.
         **/
        public IPRunner WithCompletion(Action onComplete)
        {
            IPRunner block = new IPRunner(this);
            block.onComplete = onComplete;
            return block;
        }

        /**
         * Create a clone of IPRunner instance with an easing function such as
         * Mathf.SmoothStep.
         * \param easingFunction func(float from, float to, float t) —> float
         **/
        public IPRunner WithEasingFunction(Func<float, float, float, float> easingFunction)
        {
            IPRunner block = new IPRunner(this);
            block.easingFunction = easingFunction;
            return block;
        }

        /**
         * Create a clone of IPRunner instance with real time enabled or disabled.
         **/
        public IPRunner WithRealTime(bool realTime)
        {
            IPRunner block = new IPRunner(this);
            block.realTime = realTime;
            return block;
        }

        public IPRunner WithDelayAfterFirstStep(bool delayAfter)
        {
            IPRunner block = new IPRunner(this);
            block.delayAfterFirstStep = delayAfter;
            return block;
        }

        /**
         * Create a clone of IPRunner instance, with an additional action to be called
         * each time step. All existing actions registered to the IPRunner will be 
         * preserved.
         **/
        public IPRunner WithAction(Action<float> stepAction)
        {
            List<Action<float>> actions = new List<Action<float>>(this.stepActions);
            actions.Add(stepAction);

            IPRunner block = new IPRunner(this);
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
