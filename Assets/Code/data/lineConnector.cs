using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct lineConnector
{
    public PLANETCLASS start, end;
    public Vector3 sPos {get {return start.representation.transform.position;}}
    public Vector3 ePos {get {return end.representation.transform.position;}}
    public float startRadius, endRadius;
    public LineRenderer lr;
    //public Vector2 startGeo {get {return helper.cartToGeo(Vector3.Lerp(sPos, ePos, (startRadius / master.unitMultiplier) / distance));}}
    //public Vector2 endGeo {get {return helper.cartToGeo(Vector3.Lerp(sPos, ePos, 1 - ((endRadius / master.unitMultiplier) / distance)));}}
    public float distance {get {return helper.distance(sPos, ePos);}}
    public lineConnector(LineRenderer lr, PLANETCLASS start, PLANETCLASS end, float startRadius, float endRadius)
    {
        this.lr = lr;
        this.start = start;
        this.end = end;
        this.startRadius = startRadius;
        this.endRadius = endRadius;
    }
}
