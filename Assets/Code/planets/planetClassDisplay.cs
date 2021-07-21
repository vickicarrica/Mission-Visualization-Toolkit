using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class planetClassDisplay : MonoBehaviour
{
    public PLANETCLASS parent;
    public float eccentricity, orbitalPeriod, semiMajor, semiMinor, trueAnomaly, longOfPeriapsis, radius, meanAnomaly;
    public List<string> children;
    public string parentName;
    public bool isStatic;
    public bool test;

    public void Update()
    {
        if (ReferenceEquals(parent, null)) return;

        children = new List<string>();
        parent.children.ForEach(x => children.Add(x.name));
        parentName = ReferenceEquals(parent.parent, null)?"":parent.parent.name;

        var k = parent.orbit.info;
        if (k is KEPLERORBIT)
        {
            eccentricity = k.e;
            orbitalPeriod = k.T;
            semiMajor = k.a;
            semiMinor = k.b;
            isStatic = parent.isStatic;
            trueAnomaly = k.v * Mathf.Rad2Deg;
            longOfPeriapsis = k.longOfPeriapsis * Mathf.Rad2Deg;
            radius = k.r;
            meanAnomaly = k.m * Mathf.Rad2Deg;
        }
        else 
        {
            // test func
            POINTSORBIT p = parent.orbit.info;
            LineRenderer lr = this.GetComponent<LineRenderer>();
            if (test)
            {
                test = false;
                lr.positionCount = p.positions.Count;
                List<Vector3> pos = new List<Vector3>();
                for (float i = 0; i < p.maxTime; i += p.timeIncrement)
                {
                    pos.Add(p.atTime(i) / master.unitMultiplier);
                    //Debug.Log(p.atTime(i) / master.unitMultiplier);
                }
                lr.SetPositions(pos.ToArray());
            }
        }
    }
}
