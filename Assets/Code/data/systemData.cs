using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class systemData
{
    public static Dictionary<string, planetData> pData = new Dictionary<string, planetData>();
    public static Dictionary<string, facilityData> bData = new Dictionary<string, facilityData>();
}

[Serializable]
public struct planetData
{
    public string orbitType, name, parent;
    public List<string> children;
    public List<string> buildings; // get a better name
    public float radius, mass, obliquity, rotationalPeriod;
    public bool isStatic;
    public Vector3Json spawnPosition;
    public kOrbitData kOrbit;
    public pOrbitData pOrbit;
    public string texturePath, modelPath;
}

[Serializable]
public struct kOrbitData
{
    public float e, a, i, longOfAscNode, argOfPeriapsis, T, periastron, m;
    public bool active;
}

[Serializable]
public struct pOrbitData
{
    public List<Vector3Json> positions;
    public float timeIncrement, maxTime, minTime;
    public bool active;
}

[Serializable]
public struct facilityData
{
    public float lat, lon, alt;
    public string name, parent;
}

// json wrapper for vector 3
[Serializable]
public struct Vector3Json
{
    public float x, y, z;
    public Vector3Json(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public static Vector3 toVector3(Vector3Json v) => new Vector3(v.x, v.y, v.z);
    public static implicit operator Vector3(Vector3Json v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
}

