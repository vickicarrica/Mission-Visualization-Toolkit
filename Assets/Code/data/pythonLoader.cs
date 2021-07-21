using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Python;

public static class pythonLoader
{
    public static string pullKeplerElements()
    {
        PythonRunner.RunFile($"{Application.dataPath}/Code/external/python/keplerRetriever.py");

        return "keplerRetriever";
    }

    public static string pullPoints()
    {
        PythonRunner.RunFile($"{Application.dataPath}/Code/external/python/pointRetriever.py");

        return "pointRetriever";
    }

    public static string pullTle()
    {
        PythonRunner.RunFile($"{Application.dataPath}/Code/external/python/tle.py");

        return "tleLoader";
    }
}