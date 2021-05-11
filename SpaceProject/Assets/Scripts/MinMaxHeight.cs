using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxHeight
{

    public float min { get; private set; }
    public float max { get; private set; }

    public MinMaxHeight()
    {
        min = float.MaxValue;
        max = float.MinValue;
    }

    public void AddValue(float comparableHeight)
    {
        if (comparableHeight > max)
        {
            max = comparableHeight;
        }
        if (comparableHeight < min)
        {
            min = comparableHeight;
        }
    }
}
