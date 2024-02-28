using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EasingFunctions
{
    /// <summary>
    /// Slow in the beginning and speeds up
    /// </summary>
    /// <param name="t"></param>
    /// <param name="exponential">How fast it speeds up</param>
    /// <returns></returns>
    public static float EaseIn(float t, float exponential)
    {
        return Mathf.Pow(t, exponential);
    }

    /// <summary>
    /// Fast in the beginning and slows down
    /// </summary>
    /// <param name="t"></param>
    /// <param name="exponential">How fast it speeds up</param>
    /// <returns></returns>
    public static float EaseOut(float t, float exponential = 4)
    {
        return 1.0f - Mathf.Pow(1.0f - t, exponential);
    }

    /// <summary>
    /// Slow in the beginning and slow in the end
    /// </summary>
    /// <param name="t"></param>
    /// <param name="exponential">How fast it is not in the middle</param>
    /// <returns></returns>
    public static float EaseInOut(float t, float exponential)
    {
        return t < 0.5 ?  Mathf.Pow(2, exponential - 1) * Mathf.Pow(t, exponential) : 1 - Mathf.Pow(-2 * t + 2, exponential) / 2;
    }

    /// <summary>
    /// EaseInOut with overshooting
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float EaseInOutBack(float x) 
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
    
        return x< 0.5f
            ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f
            : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x* 2f - 2f) + c2) + 2f) / 2f;
    }

    /// <summary>
    /// Bounces in and out
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float EaseInOutElastic(float x)
    {
        float c5 = (2 * Mathf.PI) / 4.5f;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : x < 0.5
          ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
    }
}
