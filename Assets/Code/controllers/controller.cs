using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.IO;
using System.Text;


/*
==========================================================================
THIS IS TEMP
THIS IS JUST FOR TESTING THE OBJECTS
DO NOT REFERENCE CONTROLLER ANYWHERE ELSE -> IT WILL BE EASIER TO REPLACE
MIGRATE TO AN OFFICAL CONTROLLER
==========================================================================
*/

public class controller : MonoBehaviour
{
    public float timeYears;
    public float timeDays;
    public bool startTime;
    public float timeIncrement = 86400f; // every second, how much time in game passes
    [Range(-1, 1)] public int timeOffset;
    private int _timeOffset;
    public bool clearTrails = false;
    public bool generateData;
    public float scale;
    public bool AU, AU100, AU1000;
    public GameObject follow;
    public bool showConnections;
    private bool drawnConnections = false;
    public GameObject connector;
    public bool forceUpdate = false;

    void Update()
    {
        if (AU) scale = helper.AU;
        if (AU100) scale = helper.AU / 100;
        if (AU1000) scale = helper.AU / 1000;
        timeYears = master.globalTime / 31536000f;
        timeDays = master.globalTime / 86400f;
        master.unitMultiplier = scale;

        if (_timeOffset != timeOffset)
        {
            _timeOffset = timeOffset;
            if (timeOffset != 0) clearTrails = true;
        }

        if (startTime) master.globalTime += timeIncrement * Time.deltaTime * timeOffset;

        if (forceUpdate)
        {
            forceUpdate = false;
            master.forceUpdateGlobalVars();
        }

        if (clearTrails)
        {
            clearTrails = false;
            foreach (PLANETCLASS planet in master.planets.Values) planet.lr.positionCount = 0;
        }

        if (showConnections && !drawnConnections)
        {
            if (connector != null)
            {
                drawnConnections = true;
                planetClassDisplay pcd = connector.GetComponent<planetClassDisplay>();
                if (pcd != null)
                {
                    // max line length is a myth
                    if (pcd.parent.parent != null) pcd.parent.connectors.Add(helperMono.instLineConnector(
                        pcd.parent, pcd.parent.parent, pcd.parent.radius, pcd.parent.parent.radius));
                    foreach (PLANETCLASS p in pcd.parent.children) pcd.parent.connectors.Add(helperMono.instLineConnector(
                        pcd.parent, p, pcd.parent.radius, p.radius));
                }
            }
        } else {
            if (connector != null && drawnConnections && !showConnections)
            {
                drawnConnections = false;
                List<GameObject> gos = GameObject.FindGameObjectsWithTag("physicalUI/lineConnection").ToList();
                foreach (GameObject go in gos) Destroy(go);
                connector.GetComponent<planetClassDisplay>().parent.connectors = new List<lineConnector>();
            }
        }
    }

    public static void init()
    {
        // https://nssdc.gsfc.nasa.gov/planetary/factsheet/
        // https://simple.wikipedia.org/wiki/Argument_of_periapsis

        // textures: https://www.solarsystemscope.com/textures/
        //          (textures are just here so i dont have to look at a white ball for hours on end)

        master.planets.OnChange += systemLoader.updatePlanetQueue;
        master.planets.OnChange += systemLoader.updateFacilityQueue;
        master.buildings.OnChange += systemLoader.updateFacilityQueue;

        pythonOutput.OnDataGenerated += keplerCallback;
        pythonOutput.OnDataGenerated += pointCallback;
        pythonOutput.OnDataGenerated += tleCallback;

        //earthAboutMoonPreset();
        //keplerSystemPreset();
        SoManySats();

        //pythonLoader.pullKeplerElements();
        //pythonLoader.pullPoints();
        //pythonLoader.pullTle();

        //Debug.Log(helper.cartToGeo(helper.geoToCart(60, 120, 0, 6378.1f), 6378.1f));

        new cone(2, 1f, 1000, Vector3.zero, Quaternion.identity);
    }

    private static void SoManySats()
    {
      PLANETCLASS earth = new PLANETCLASS(
          "earth",
          6378.1f,
          Vector3.zero,
          0,
          23.44f * Mathf.Deg2Rad,
          86164.2f,
          new ORBIT(0.0167f, 149.596f * Mathf.Pow(10, 6), 0 * Mathf.Deg2Rad, 18.272f * Mathf.Deg2Rad, 85.901f * Mathf.Deg2Rad, 31_536_000),
          texturePath: "materials/textures/earth");

      PLANETCLASS moon = new PLANETCLASS(
          "moon",
          1737.1f,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT(0.0549f, 0.3844f * Mathf.Pow(10, 6), 28.58f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 27.322f * 24f * 60f * 60f),
          parent:earth,
          texturePath: "materials/textures/moon");

      PLANETCLASS AIM = new PLANETCLASS(
          "AIM",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/AIM", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS AQUA = new PLANETCLASS(
          "AQUA",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/AQUA", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS AURA = new PLANETCLASS(
              "AURA",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/AURA", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS FGST = new PLANETCLASS(
              "FGST",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/FGST", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES1 = new PLANETCLASS(
                  "GOES 1",
                  1,
                  Vector3.zero,
                  0,
                  0f,
                  0f,
                  new ORBIT("data/SATS/GOES 1", 60),
                  parent: earth,
                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES2 = new PLANETCLASS(
                  "GOES 2",
                  1,
                  Vector3.zero,
                  0,
                  0f,
                  0f,
                  new ORBIT("data/SATS/GOES 2", 60),
                  parent: earth,
                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES3 = new PLANETCLASS(
                      "GOES 3",
                      1,
                      Vector3.zero,
                      0,
                      0f,
                      0f,
                      new ORBIT("data/SATS/GOES 3", 60),
                      parent: earth,
                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES4 = new PLANETCLASS(
                      "GOES 4",
                      1,
                      Vector3.zero,
                      0,
                      0f,
                      0f,
                      new ORBIT("data/SATS/GOES 4", 60),
                      parent: earth,
                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES5 = new PLANETCLASS(
                                  "GOES 5",
                                  1,
                                  Vector3.zero,
                                  0,
                                  0f,
                                  0f,
                                  new ORBIT("data/SATS/GOES 5", 60),
                                  parent: earth,
                                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES6 = new PLANETCLASS(
                                  "GOES 6",
                                  1,
                                  Vector3.zero,
                                  0,
                                  0f,
                                  0f,
                                  new ORBIT("data/SATS/GOES 6", 60),
                                  parent: earth,
                                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES7 = new PLANETCLASS(
                                      "GOES 7",
                                      1,
                                      Vector3.zero,
                                      0,
                                      0f,
                                      0f,
                                      new ORBIT("data/SATS/GOES 7", 60),
                                      parent: earth,
                                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES8 = new PLANETCLASS(
                                      "GOES 8",
                                      1,
                                      Vector3.zero,
                                      0,
                                      0f,
                                      0f,
                                      new ORBIT("data/SATS/GOES 8", 60),
                                      parent: earth,
                                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES9 = new PLANETCLASS(
                  "GOES 9",
                  1,
                  Vector3.zero,
                  0,
                  0f,
                  0f,
                  new ORBIT("data/SATS/GOES 9", 60),
                  parent: earth,
                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES10 = new PLANETCLASS(
                  "GOES 10",
                  1,
                  Vector3.zero,
                  0,
                  0f,
                  0f,
                  new ORBIT("data/SATS/GOES 10", 60),
                  parent: earth,
                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES11 = new PLANETCLASS(
                      "GOES 11",
                      1,
                      Vector3.zero,
                      0,
                      0f,
                      0f,
                      new ORBIT("data/SATS/GOES 11", 60),
                      parent: earth,
                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES12 = new PLANETCLASS(
                      "GOES 12",
                      1,
                      Vector3.zero,
                      0,
                      0f,
                      0f,
                      new ORBIT("data/SATS/GOES 12", 60),
                      parent: earth,
                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES13 = new PLANETCLASS(
                                  "GOES 13",
                                  1,
                                  Vector3.zero,
                                  0,
                                  0f,
                                  0f,
                                  new ORBIT("data/SATS/GOES 13", 60),
                                  parent: earth,
                                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES14 = new PLANETCLASS(
                                  "GOES 14",
                                  1,
                                  Vector3.zero,
                                  0,
                                  0f,
                                  0f,
                                  new ORBIT("data/SATS/GOES 14", 60),
                                  parent: earth,
                                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES15 = new PLANETCLASS(
                                      "GOES 15",
                                      1,
                                      Vector3.zero,
                                      0,
                                      0f,
                                      0f,
                                      new ORBIT("data/SATS/GOES 15", 60),
                                      parent: earth,
                                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES16 = new PLANETCLASS(
                                      "GOES 16",
                                      1,
                                      Vector3.zero,
                                      0,
                                      0f,
                                      0f,
                                      new ORBIT("data/SATS/GOES 16", 60),
                                      parent: earth,
                                      modelPath: "prefabs/models/AURA");

      PLANETCLASS GOES17 = new PLANETCLASS(
                  "GOES 17",
                  1,
                  Vector3.zero,
                  0,
                  0f,
                  0f,
                  new ORBIT("data/SATS/GOES 17", 60),
                  parent: earth,
                  modelPath: "prefabs/models/AURA");

      PLANETCLASS GPM_CORE = new PLANETCLASS(
          "GPM_CORE",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/GPM_CORE", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS GRACEFO1 = new PLANETCLASS(
          "GRACE FO1",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/GRACE-FO1", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS GRACEFO2 = new PLANETCLASS(
              "GRACE FO2",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/GRACE-FO2", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS HST = new PLANETCLASS(
              "HST",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/HST", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS ICESAT2 = new PLANETCLASS(
          "ICESAT 2",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/ICESAT-2", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS ICON = new PLANETCLASS(
          "ICON",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/ICON", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS IRIS = new PLANETCLASS(
              "IRIS",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/IRIS", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS ISS = new PLANETCLASS(
              "ISS",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/ISS", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS LANDSAT7 = new PLANETCLASS(
              "LANDSAT 7",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/LANDSAT-7", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS LANDSAT8 = new PLANETCLASS(
              "LANDSAT 8",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/LANDSAT-8", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS METOPA = new PLANETCLASS(
          "METOP A",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/METOP A", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS METOPB = new PLANETCLASS(
          "METOP B",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/METOP B", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS METOPC = new PLANETCLASS(
              "METOP C",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/METOP_C", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS MMS1 = new PLANETCLASS(
              "MMS 1",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/MMS 1", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS MMS2 = new PLANETCLASS(
         "MMS 2",
         1,
         Vector3.zero,
         0,
         0f,
         0f,
         new ORBIT("data/SATS/MMS 2", 120),
         parent: earth,
         modelPath: "prefabs/models/AURA");

      PLANETCLASS MMS3 = new PLANETCLASS(
             "MMS 3",
             1,
             Vector3.zero,
             0,
             0f,
             0f,
             new ORBIT("data/SATS/MMS 3", 120),
             parent: earth,
             modelPath: "prefabs/models/AURA");

      PLANETCLASS MMS4 = new PLANETCLASS(
             "MMS 4",
             1,
             Vector3.zero,
             0,
             0f,
             0f,
             new ORBIT("data/SATS/MMS 4", 120),
             parent: earth,
             modelPath: "prefabs/models/AURA");

      PLANETCLASS NUSTAR = new PLANETCLASS(
         "NUSTAR",
         1,
         Vector3.zero,
         0,
         0f,
         0f,
         new ORBIT("data/SATS/NUSTAR", 120),
         parent: earth,
         modelPath: "prefabs/models/AURA");

      PLANETCLASS OCO2 = new PLANETCLASS(
         "OCO 2",
         1,
         Vector3.zero,
         0,
         0f,
         0f,
         new ORBIT("data/SATS/OCO-2", 120),
         parent: earth,
         modelPath: "prefabs/models/AURA");

      PLANETCLASS SCISAT1 = new PLANETCLASS(
             "SCISAT 1",
             1,
             Vector3.zero,
             0,
             0f,
             0f,
             new ORBIT("data/SATS/SCISAT-1", 120),
             parent: earth,
             modelPath: "prefabs/models/AURA");

      PLANETCLASS SDO = new PLANETCLASS(
             "SDO",
             1,
             Vector3.zero,
             0,
             0f,
             0f,
             new ORBIT("data/SATS/SDO", 120),
             parent: earth,
             modelPath: "prefabs/models/AURA");

      PLANETCLASS SEAHAWK1 = new PLANETCLASS(
          "SEAHAWK 1",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/SEAHAWK-1", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS SMAP = new PLANETCLASS(
              "SMAP",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/SMAP", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS SOLARB = new PLANETCLASS(
              "SOLAR B",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/SOLAR-B", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS SOYUZ = new PLANETCLASS(
          "SOYUZ",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/SOYUZ", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS STPSAT3 = new PLANETCLASS(
          "STPSat 3",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/STPSat-3", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS STPSAT4 = new PLANETCLASS(
              "STPSat 4",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/STPSat-4", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS STPSAT5 = new PLANETCLASS(
              "STPSat5",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/STPSat-5", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS SWIFT = new PLANETCLASS(
          "SWIFT",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/SWIFT", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS3 = new PLANETCLASS(
              "TDRS 3",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-3", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS5 = new PLANETCLASS(
              "TDRS 5",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-5", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS6 = new PLANETCLASS(
          "TDRS 6",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/TDRS-6", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS7 = new PLANETCLASS(
          "TDRS 7",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/TDRS-7", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS8 = new PLANETCLASS(
              "TDRS 8",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-8", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS9 = new PLANETCLASS(
              "TDRS 9",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-9", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS10 = new PLANETCLASS(
              "TDRS 10",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-10", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS11 = new PLANETCLASS(
              "TDRS 11",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TDRS-11", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS12 = new PLANETCLASS(
          "TDRS 12",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/TDRS-12", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS TDRS13 = new PLANETCLASS(
          "TDRS 13",
          1,
          Vector3.zero,
          0,
          0f,
          0f,
          new ORBIT("data/SATS/TDRS-13", 120),
          parent: earth,
          modelPath: "prefabs/models/AURA");

      PLANETCLASS TERRA = new PLANETCLASS(
              "TERRA",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/TERRA", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS THEMISA = new PLANETCLASS(
              "THEMIS A",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/THEMIS_A", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS THEMISD = new PLANETCLASS(
              "THEMIS D",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/THEMIS_D", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");

      PLANETCLASS THEMISE = new PLANETCLASS(
              "THEMIS E",
              1,
              Vector3.zero,
              0,
              0f,
              0f,
              new ORBIT("data/SATS/THEMIS_E", 120),
              parent: earth,
              modelPath: "prefabs/models/AURA");
    }

    private static void earthAboutMoonPreset()
    {
        PLANETCLASS moon = new PLANETCLASS(
            "moon",
            1737.1f,
            Vector3.zero,
            0,
            0f,
            0f,
            new ORBIT(0.0549f, 0.3844f * Mathf.Pow(10, 6), 28.58f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 27.322f * 24f * 60f * 60f),
            texturePath: "materials/textures/moon");

        PLANETCLASS earth = new PLANETCLASS(
            "earth",
            6378.1f,
            Vector3.zero,
            0,
            23.44f * Mathf.Deg2Rad,
            86164.2f,
            new ORBIT("data/Epos", 3600),
            parent: moon,
            texturePath: "materials/textures/earth");

        FACILITYCLASS f1 = new FACILITYCLASS("facility-f1", earth);
        FACILITYCLASS f2 = new FACILITYCLASS("facility-f2", moon);
    }

    private static void keplerSystemPreset()
    {
        PLANETCLASS sun = new PLANETCLASS(
            "sun",
            1_390_000,
            Vector3.zero,
            0,
            0,
            0,
            ORBIT.empty(),
            null,
            texturePath: "materials/textures/sun");
        sun.isStatic = true;

        PLANETCLASS earth = new PLANETCLASS(
            "earth",
            6378.1f,
            Vector3.zero,
            0,
            23.44f * Mathf.Deg2Rad,
            86164.2f,
            new ORBIT(0.0167f, 149.596f * Mathf.Pow(10, 6), 0 * Mathf.Deg2Rad, 18.272f * Mathf.Deg2Rad, 85.901f * Mathf.Deg2Rad, 31_536_000),
            parent: sun,
            texturePath: "materials/textures/earth");

        PLANETCLASS moon = new PLANETCLASS(
            "moon",
            1737.1f,
            Vector3.zero,
            0,
            0f,
            0f,
            new ORBIT(0.0549f, 0.3844f * Mathf.Pow(10, 6), 28.58f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad, 27.322f * 24f * 60f * 60f),
            parent: earth,
            texturePath: "materials/textures/moon");

        // geosynchronous
        PLANETCLASS syncom2 = new PLANETCLASS(
            "syncom_2",
            1,
            Vector3.zero,
            0,
            0,
            0,
            new ORBIT(0, 35827f + earth.radius / 2f, 33.6f * Mathf.Deg2Rad, 0, 0, 1436.4f * 60f),
            parent: earth,
            modelPath: "prefabs/models/GSAT_AURA"
        );

        // mid earth
        PLANETCLASS lcs1 = new PLANETCLASS(
            "lcs_1",
            1,
            Vector3.zero,
            0,
            0,
            0,
            new ORBIT(0, 2793f + earth.radius / 2f, 32.14f * Mathf.Deg2Rad, 0, 0, 145.56f * 60),
            parent: earth,
            modelPath: "prefabs/models/GSAT_AURA"
        );

        // low earth
        PLANETCLASS cosmos2463 = new PLANETCLASS(
            "cosmos_2463",
            1,
            Vector3.zero,
            0,
            0,
            0,
            new ORBIT(0, 994f + earth.radius / 2f, 83f * Mathf.Deg2Rad, 0, 0, 105f * 60f),
            parent: earth,
            modelPath: "prefabs/models/GSAT_AURA"
        );

        FACILITYCLASS f1 = new FACILITYCLASS("facility-f1", earth);
        FACILITYCLASS f2 = new FACILITYCLASS("facility-f2", moon);
    }

    private static void tleCallback(object sender, EventArgs e)
    {
        (dynamic, bool) output = pythonOutput.retrieve("tleLoader");
        if (!output.Item2) return;

        List<Vector3> positions = new List<Vector3>();
        dynamic data = output.Item1;

        StringBuilder csv = new StringBuilder();
        int i = 0;
        foreach (dynamic v in data)
        {
            positions.Add(new Vector3(
                (float) Convert.ToDouble(v[0]),
                (float) Convert.ToDouble(v[2]),
                (float) Convert.ToDouble(v[1])));
            i++;

            csv.AppendLine($"{v[0]}, {v[1]}, {v[2]}");
        }

        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "tle.csv", csv.ToString());

        PLANETCLASS earth = new PLANETCLASS(
            "earth",
            6378.1f,
            Vector3.zero,
            0,
            23.44f * Mathf.Deg2Rad,
            86164.2f,
            new ORBIT(0.0167f, 149.596f * Mathf.Pow(10, 6), 0 * Mathf.Deg2Rad, 18.272f * Mathf.Deg2Rad, 85.901f * Mathf.Deg2Rad, 31_536_000),
            texturePath: "materials/textures/earth");

        PLANETCLASS tle = new PLANETCLASS(
            "tle",
            1,
            Vector3.zero,
            0,
            0f,
            0f,
            new ORBIT(new POINTSORBIT(positions, 60)),
            parent: earth,
            modelPath: "prefabs/models/GSAT_AURA");
    }

    private static void keplerCallback(object sender, EventArgs e)
    {
        (dynamic, bool) output = pythonOutput.retrieve("keplerRetriever");
        if (!output.Item2) return;

        dynamic data = output.Item1;

        KEPLERORBIT k = new KEPLERORBIT(
            (float) Convert.ToDouble(data[0][0]),
            (float) Convert.ToDouble(data[0][1]),
            (float) Convert.ToDouble(data[0][2]),
            (float) Convert.ToDouble(data[0][3]),
            (float) Convert.ToDouble(data[0][4]),
            (float) Convert.ToDouble(data[0][5]));

        GameObject go = Instantiate(helper.linePrefab);
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < 24 * 1; i++)
        {
            positions.Add(k.atTime(i * 3600) / master.unitMultiplier / 100);
        }

        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
        lr.startWidth = 25;
        lr.endWidth = 50;

        go.name = "kepler";

        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Eccentricity, Semi-Major, Inclination, Long of Asc Node, Arg of Peri, Period, Mean Anomaly, True Anomaly");
        foreach (dynamic l in data)
        {
            csv.AppendLine($"{l[0]}, {l[1]}, {l[2]}, {l[3]}, {l[4]}, {l[5]}, {l[6]}, {l[7]}");
        }

        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "kepler.csv", csv.ToString());
    }

    private static void pointCallback(object sender, EventArgs e)
    {
        (dynamic, bool) output = pythonOutput.retrieve("pointRetriever");
        if (!output.Item2) return;

        List<Vector3> positions = new List<Vector3>();
        dynamic data = output.Item1;

        StringBuilder csv = new StringBuilder();
        csv.AppendLine("x, y, z, vx, vy, vz");
        int i = 0;
        foreach (dynamic v in data)
        {
            positions.Add(new Vector3(
                (float) Convert.ToDouble(v[0]),
                (float) Convert.ToDouble(v[2]),
                (float) Convert.ToDouble(v[1])));
            i++;

            csv.AppendLine($"{v[0]}, {v[1]}, {v[2]}, {v[3]}, {v[4]}, {v[5]}");
        }

        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "points.csv", csv.ToString());

        /*
        GameObject go = Instantiate(helper.linePrefab);

        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
        lr.startWidth = 25;
        lr.endWidth = 50;
        lr.material = helper.lineDiff;
        go.name = "point";
        return;
        */

        PLANETCLASS earth = new PLANETCLASS(
            "earth",
            6378.1f,
            Vector3.zero,
            0,
            23.44f * Mathf.Deg2Rad,
            86164.2f,
            new ORBIT(0.0167f, 149.596f * Mathf.Pow(10, 6), 0 * Mathf.Deg2Rad, 18.272f * Mathf.Deg2Rad, 85.901f * Mathf.Deg2Rad, 31_536_000),
            texturePath: "materials/textures/earth");

        PLANETCLASS moon = new PLANETCLASS(
            "moon",
            1737.1f,
            Vector3.zero,
            0,
            0f,
            0f,
            new ORBIT(new POINTSORBIT(positions, 3600)),
            parent: earth,
            texturePath: "materials/textures/moon");

        // todo: implement in POINTSORBIT as paramter
        List<Vector3> _points = helper.parseCSV("data/earthSat");
        List<Vector3> points = new List<Vector3>();
        foreach (Vector3 _v in _points)
        {
            Vector3 v = new Vector3(_v.x, _v.z, _v.y);
            v *= -1;
            points.Add(v);
        }
        PLANETCLASS earthSat = new PLANETCLASS(
            "earth_sat",
            1,
            Vector3.zero,
            0,
            0,
            0,
            new ORBIT(new POINTSORBIT(points, 60)),
            parent: earth,
            modelPath: "prefabs/models/GSAT_AURA"
        );
    }
}
