# Interpolactic: Easy Animation Scripting in Unity
Interpolactic takes the pain out of Coroutine animations by bundling the boilerplate of a time-based IEnumerator into a composable, reusable builder. The actual Coroutine is then wrapped in a Runner object that offers a kit of utility methods to control playback. 

The beauty of Interpolactic is that the caller declares the implementation by defining what happens at every time step. All Interpolactic needs is a function or closure that acts upon a time value ranging from 0 to 1. 

## Features 
* **Animate anything:**   Interpolactic doesn't limit users to acting on common types such as float and color. 
* **Playback utilities:**   Pause and resume Interpolactic animations with ease. 
* **Fire and forget:** Keep animations going indefinitely with options to loop and ping-pong.  
* **Composable:** Animations are composed via method chaining, so you only need to pass the arguments you care about.


## Examples
**Fading in a CanvasGroup:**
```
new Interpolation(t => canvas.alpha = t)
  .Duration(0.5f)
  .Build(this).Play();
``` 
<br />

**Moving a Transform:**
```
new Interpolation(t => transform.position = Vector3.Lerp(start, end, t))
  .Duration(0.5f)
  .Build(this).Play();
```
<br />

**Ping-pong an animation indefinitely:**
```
new Interpolation(t => transform.position = Vector3.Lerp(start, end, t))
  .Duration(0.5f)
  .Repeats(true)
  .PingPong(true)
  .Build(this).Play();
```

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


