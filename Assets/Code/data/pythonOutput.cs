using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

// implement event
public class pythonOutput : MonoBehaviour
{
    private static Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();
    public static event EventHandler OnDataGenerated;

    public static (dynamic, bool) retrieve(string key)
    {
        if (data.ContainsKey(key))
        {
            dynamic output = data[key];
            data.Remove(key);
            return (output, true);
        }
        return (null, false);
    }

    public static void add(string key, dynamic value)
    {
        data.Add(key, value);
        EventHandler handler = OnDataGenerated;
        handler?.Invoke(null, EventArgs.Empty);
    }
}