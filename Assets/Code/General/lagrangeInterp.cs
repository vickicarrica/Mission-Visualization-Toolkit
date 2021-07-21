using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public readonly struct lagrangeInterp
{
    public readonly List<Vector2> values;
    public readonly float increment;
    public lagrangeInterp(List<Vector2> values, float increment)
    {
        this.values = values;
        this.increment = increment;
    }

    public float at(float x, int degree = -1)
    {
        int index = (int) (x / increment);
        float reminder = x % increment;

        if (reminder == 0) return values[index].y;
        return Mathf.Lerp(values[index].y, values[index + 1].y, reminder / increment);
        /*
        // TODO move values into dictionary?
        if (values.Exists(v => v.x == x)) return values.Find(v => v.x == x).y;

        // TODO actually implement lagrange input
        // currently the problem seems to be that the value gets to big/small and ends up becoming NaN
        // incorrect implementation of the alg?
        float total = 0;
        float index = 0;
        foreach (Vector2 point in values)
        {
            float term = 1;
            foreach (Vector2 _point in values) 
            {
                if (_point == point) continue;
                term *= (x - _point.x) / (point.x - _point.x);
            }
            total += term * point.y;

            if (degree != -1)
            {
                if (index == degree) break;
                index++;
            }
        }

        return total;
        */
    }
}
