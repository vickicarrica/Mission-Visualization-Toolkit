using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facilityClassDisplay : MonoBehaviour
{
    public FACILITYCLASS parent;
    [Range(-360, 360)] public float lat;
    [Range(-360, 360)] public float lon;
    public float alt;

    public void OnValidate()
    {
        if (ReferenceEquals(parent, null)) return;
        parent.lat = lat;
        parent.lon = lon;
        parent.alt = alt;
    }
}
