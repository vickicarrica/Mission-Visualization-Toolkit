using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class helper
{
    public readonly static GameObject planetPrefab = (GameObject) Resources.Load("prefabs/models/Default");
    public readonly static GameObject textPrefab = (GameObject) Resources.Load("prefabs/textPrefab");
    public readonly static GameObject facilityPrefab = (GameObject) Resources.Load("prefabs/facilityModels/facilityPrefab");
    public readonly static GameObject linePrefab = (GameObject) Resources.Load("prefabs/physicalLineRendererPrefab");
    public readonly static Material lineDiff = (Material) Resources.Load("materials/lineDiff");
    public readonly static Material coneTexture = (Material) Resources.Load("materials/coneTexture");
    public readonly static GameObject conePrefab = (GameObject) Resources.Load("prefabs/conePrefab");

    // dont question it
    public static helperMono mono;

    public readonly static float c = 299792.458f;
    public readonly static float AU = 150_000_000;
    public readonly static float G = 6.674f * Mathf.Pow(10, -11); // Gravitational constant
    public static Vector2 quadrant(float angle)
    {
        if (angle < 0) angle += 360f; // make sure all angles are positive

        if (angle < 90f) return Vector2.one;
        else if (angle < 180f) return new Vector2(-1, 1);
        else if (angle < 270f) return new Vector2(-1, -1);
        return new Vector2(1, -1);
    }

    public static float toOne(float value) => (value < 0) ? -1 : 1;

    public static Vector3 v2Tov3(Vector2 v) => new Vector3(v.x, 0, v.y);

    public static Vector3 minSize(Vector3 v) => new Vector3(Mathf.Max(0.01f, v.x / master.unitMultiplier), Mathf.Max(0.01f, v.y / master.unitMultiplier), Mathf.Max(0.01f, v.z / master.unitMultiplier));

    public static Vector3 geoToCart(float lat, float lon, float alt, float radius) // assumes in degrees
    {
        // alt is disstance from surface -> typically will be 0
        lat *= Mathf.Deg2Rad;   // convert to radians
        lon *= Mathf.Deg2Rad;
        radius = (0.5f + alt / (radius * 2f));

        float x = radius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float y = radius * Mathf.Cos(lat) * Mathf.Sin(lon);
        float z = radius * Mathf.Sin(lat);

        return new Vector3(x, z, y); // flip z and y axises to match unity
    }

    // calculated in unity system
    // not working
    public static Vector2 cartToGeo(Vector3 v, float r)
    {
        v = new Vector3(v.x, v.z, v.y); // flip z and y axis to exit unity system

        float lat = Mathf.Asin(v.z / r);
        float lon = Mathf.Atan2(v.y, v.x);


        return new Vector2(lat * Mathf.Rad2Deg, lon * Mathf.Rad2Deg);
    }

    public static float distance(Vector3 v1, Vector3 v2) => Mathf.Sqrt(
        ((v1.x - v2.x) * (v1.x - v2.x)) +
        ((v1.y - v2.y) * (v1.y - v2.y)) +
        ((v1.z - v2.z) * (v1.z - v2.z)));

    public static List<Vector3> parseCSV(string path)
    {
        TextAsset data = (TextAsset) Resources.Load(path);
        List<string> formatted = data.ToString().Split('\n').ToList();
        List<Vector3> returnList = new List<Vector3>();

        foreach (string f in formatted)
        {
            List<String> _f = f.Split(',').ToList();
            if (_f.Count != 3) continue; // happens sometimes at end of file
            returnList.Add(new Vector3(
                (float) Convert.ToDouble(_f[0]),
                (float) Convert.ToDouble(_f[1]),
                (float) Convert.ToDouble(_f[2])));
        }

        return returnList;
    }

    public static facilityData facilityToData(FACILITYCLASS f)
    {
        facilityData fd = new facilityData();
        fd.lat = f.lat;
        fd.alt = f.alt;
        fd.lon = f.lon;
        fd.name = f.name;
        fd.parent = f.parent.name;
        return fd;
    }

    public static kOrbitData kOrbitToData(KEPLERORBIT o)
    {
        kOrbitData od = new kOrbitData();
        if (o.isEmpty()) return od;
        od.e = o.e;
        od.a = o.a;
        od.i = o.i;
        od.longOfAscNode = o.longOfAscNode;
        od.argOfPeriapsis = o.argOfPeriapsis;
        od.T = o.T;
        od.periastron = o.periastron;
        od.active = o.active;
        return od;
    }

    public static pOrbitData pOrbitToData(POINTSORBIT o)
    {
        pOrbitData od = new pOrbitData();
        if (o.isEmpty()) return od;
        od.positions = new List<Vector3Json>();
        o.positions.ForEach(x => od.positions.Add(new Vector3Json(x)));
        od.timeIncrement = o.timeIncrement;
        od.maxTime = o.maxTime;
        od.minTime = o.minTime;
        od.active = o.active;
        return od;
    }

    public static planetData planetToData(PLANETCLASS p)
    {
        planetData pd = new planetData();
        if (p.orbit.selectedOrbit == 0) pd.orbitType = "kepler";
        else if (p.orbit.selectedOrbit == 1) pd.orbitType = "point";
        else pd.orbitType = "none";
        pd.name = p.name;
        pd.parent = ReferenceEquals(p.parent, null)?"":p.parent.name;
        pd.children = new List<string>();
        // recursion pog!!!!!
        p.children.ForEach(x => pd.children.Add(x.name));
        pd.buildings = new List<string>();
        p.buildings.ForEach(x => pd.buildings.Add(x.name));
        pd.radius = p.radius;
        pd.mass = p.mass;
        pd.obliquity = p.obliquity;
        pd.rotationalPeriod = p.rotationalPeriod;
        pd.isStatic = p.isStatic;
        pd.spawnPosition = new Vector3Json(p.spawnPosition);
        pd.texturePath = p.representation.texturePath;
        pd.modelPath = p.representation.modelPath;
        pd.kOrbit = helper.kOrbitToData(pd.orbitType == "kepler"?(KEPLERORBIT) p.orbit.info:KEPLERORBIT.empty()); // max line length be damned
        pd.pOrbit = helper.pOrbitToData(pd.orbitType == "point"?(POINTSORBIT) p.orbit.info:POINTSORBIT.empty());
        return pd;
    }
}
