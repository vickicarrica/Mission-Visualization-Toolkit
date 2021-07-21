using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System;

public class FACILITYCLASS
{
    public GameObject representation;
    public PLANETCLASS parent;
    public readonly string name;
    public float lat
    {
        get {return _lat;}
        set {updateGeo(value, ref _lat);}
    }
    public float lon
    {
        get {return _lon;}
        set {updateGeo(value, ref _lon);}
    }
    public float alt
    {
        get {return _alt;}
        set {updateGeo(value, ref _alt);}
    }

    private float _lat, _lon, _alt;

    public FACILITYCLASS(string name, PLANETCLASS parent)
    {
        this.name = name;
        master.buildings.Add(this.name, this);
        parent.registerBuilding(this);
    }

    public FACILITYCLASS(facilityData fd)
    {
        this.name = fd.name;
        this._lat = fd.lat;
        this._lon = fd.lon;
        this._alt = fd.alt;

        master.buildings.Add(this.name, this);

        systemLoader.tryRegisterFacility(fd.parent, this.name);
    }

    public void loadPhysicalData()
    {
        Quaternion startRotation = Quaternion.identity;
        startRotation.eulerAngles = new Vector3(0,90,90);
        representation = helperMono.inst(helper.facilityPrefab, Vector3.zero, startRotation, parent.representation.transform);
        representation.GetComponent<facilityClassDisplay>().parent = this;
        representation.transform.localPosition = helper.geoToCart(lat, lon, alt, parent.radius);
        representation.name = this.name;

        systemData.bData[this.name] = helper.facilityToData(this);
    }

    private void updateGeo(float newValue, ref float toUpdate)
    {
        toUpdate = newValue;
        representation.transform.localPosition = helper.geoToCart(lat, lon, alt, parent.radius);
        systemData.bData[this.name] = helper.facilityToData(this);
    }
}

