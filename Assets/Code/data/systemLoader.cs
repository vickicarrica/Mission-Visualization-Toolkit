using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class systemLoader
{
    private static List<(string parent, string child)> planetRegisterQueue = new List<(string parent, string self)>();
    private static List<(string parent, string self)> facilityRegisterQueue = new  List<(string parent, string self)>();

    public static void tryRegisterPlanetChild(string parentName, string childName)
    {
        // if a planet has already attempted to register, dont try to register again
        if (!(planetRegisterQueue.Contains((parentName, childName))))
        {
            if (master.planets.ContainsKey(parentName) && master.planets.ContainsKey(childName)) master.planets[parentName].registerChild(master.planets[childName]);
            else planetRegisterQueue.Add((parentName, childName));
        }
    }

    public static void updatePlanetQueue(object sender, EventArgs e)
    {
        foreach((string parent, string child) l in new List<(string, string)>(planetRegisterQueue))
        {
            if (master.planets.ContainsKey(l.parent) && master.planets.ContainsKey(l.child))
            {
                master.planets[l.parent].registerChild(master.planets[l.child]);
                planetRegisterQueue.Remove(l);
            }
        }
    }

    public static void tryRegisterFacility(string parentName, string self)
    {
        if (!(facilityRegisterQueue.Contains((parentName, self))))
        {
            facilityRegisterQueue.Add((parentName, self));
        }
        updateFacilityQueue(null, EventArgs.Empty);
    }

    public static void updateFacilityQueue(object sender, EventArgs e)
    {
        foreach((string parent, string self) l in new List<(string, string)>(facilityRegisterQueue))
        {
            if (master.planets.ContainsKey(l.parent) && master.buildings.ContainsKey(l.self))
            {
                master.planets[l.parent].registerBuilding(master.buildings[l.self]);
                facilityRegisterQueue.Remove(l);
            }
        }
    }
}
