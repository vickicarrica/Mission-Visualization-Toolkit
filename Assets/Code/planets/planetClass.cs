using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System;
using TMPro;

/// <summary>
/// Main class of planets and their information.
/// </summary>
/// <remarks>
/// Related classes: <see cref="KEPLERORBIT"/>, <see cref="PHYSICALPLANET"/>, <see cref="POINTSORBIT"/>
/// </remarks>
public class PLANETCLASS
{
    public readonly string name;
    /// <summary> Radius of a planet (km) </summary>
    public readonly float radius;
    /// <summary> Mass of a planet (kg) </summary>
    public readonly float mass;
    /// <summary> Offset of a planet's axis (rad) </summary>
    public readonly float obliquity;
    /// <summary> Time for a planet to reach the same starting point (s) </summary> 
    public readonly float rotationalPeriod;
    /// <summary> Position that the planet was initalized in (km) </summary>
    public readonly Vector3 spawnPosition;
    /// <summary> Representational data for this planet </summary>
    public PHYSICALPLANET representation {get; private set;}
    /// <summary> Class which controls the orbit of this planet via keplerian elements </summary>
    public readonly ORBIT orbit; 
    /// <summary> Personal LineRenderer used for drawing the planets trail </summary>
    public LineRenderer lr {get; private set;}
    /// <summary> Controls whether the planet is allowed to change position </summary>
    public bool isStatic = false;
    /// <summary> The <see cref="PLANETCLASS"/>that this planet orbits </summary>
    public PLANETCLASS parent;
    /// <summary> The <see cref="PLANETCLASS"/>' that orbits this planet </summary>
    public List<PLANETCLASS> children = new List<PLANETCLASS>();
    public List<FACILITYCLASS> buildings = new List<FACILITYCLASS>();
    public List<lineConnector> connectors = new List<lineConnector>();
    /// <summary> Determines if this planet has finished initalizing </summary>
    public bool ready {get; private set;}

    /// <summary>
    /// Main constructor for <see cref="PLANETCLASS"/>
    /// </summary>
    public PLANETCLASS(string name, float radius, Vector3 position, float mass, float tilt, float revolutionTime, ORBIT orbit, PLANETCLASS parent = null, string texturePath = null, string modelPath = null)
    {
        // copy given values
        this.name = "planet-" + name;
        this.radius = radius;
        this.mass = mass;
        this.spawnPosition = position;
        this.orbit = orbit;
        this.obliquity = tilt;
        this.rotationalPeriod = revolutionTime;
        this.parent = parent;
        
        // child setup
        if (!ReferenceEquals(parent, null)) parent.registerChild(this);

        // create representation
        this.loadPhysicalData(texturePath, modelPath);

        this.addToReferences();
    }

    public PLANETCLASS(planetData pd)
    {
        this.name = pd.name;
        this.radius = pd.radius;
        this.mass = pd.mass;
        this.spawnPosition = pd.spawnPosition;
        if (pd.orbitType == "kepler") this.orbit = new ORBIT(new KEPLERORBIT(pd.kOrbit));
        else this.orbit = new ORBIT(new POINTSORBIT(pd.pOrbit));
        this.obliquity = pd.obliquity;
        this.rotationalPeriod = pd.rotationalPeriod;

        this.isStatic = pd.isStatic;

        this.loadPhysicalData(pd.texturePath, pd.modelPath);
        this.addToReferences();

        pd.children.ForEach(x => systemLoader.tryRegisterPlanetChild(this.name, x));
        pd.buildings.ForEach(x => systemLoader.tryRegisterFacility(this.name, x));
    }

    private void loadPhysicalData(string texturePath, string modelPath)
    {
        Material texture = (Material) Resources.Load(texturePath);
        GameObject go = (modelPath == null)?helper.planetPrefab:(GameObject) Resources.Load(modelPath);
        representation = new PHYSICALPLANET(helperMono.inst(go, this.spawnPosition, Quaternion.identity), texturePath, modelPath, name);
        if (texture != null) representation.mr.material = texture;

        lr = representation.go.GetComponent<LineRenderer>();
        representation.go.GetComponent<planetClassDisplay>().parent = this;
        representation.transform.rotation = Quaternion.Euler(obliquity * Mathf.Rad2Deg, 0, 0);
        representation.transform.parent = GameObject.FindGameObjectWithTag("parents/objects").transform;
        representation.go.name = this.name;
    }

    private void addToReferences()
    {
        // assign events
        master.onGlobalTimeChanged += OnTimeUpdate;
        master.onUnitMultiplierChanged += OnUnitMultiChange;

        // add to master
        master.planets.Add(this.name, this);

        // finish init
        ready = true;

        // call events to sync representation with values
        master.forceUpdateGlobalVars();

        // save data
        systemData.pData[this.name] = helper.planetToData(this);
    }

    public void registerChild(PLANETCLASS child)
    {
        this.children.Add(child);
        child.parent = this;
        systemData.pData[this.name].children.Add(child.name);
    }

    public void registerBuilding(FACILITYCLASS building)
    {
        if (this.buildings.Contains(building)) return;
        this.buildings.Add(building);
        building.parent = this;

        building.loadPhysicalData();
    }

    /// <summary> Subscriber to <see cref="master.onGlobalTimeChanged"/></summary>
    /// <remarks> Primary way for a planet to change position </remarks>
    public void OnTimeUpdate(object sender, EventArgs args) // currently both args will be null/not contain any info
    {
        if (!ready) return; // only run after init has finished
        if (!orbit.active) return;
        if (!isStatic) 
        {
            representation.transform.position = orbit.position / master.unitMultiplier;
            if (master.referenceFrame != null) representation.transform.position -= master.referenceFrame.transform.position;
            if (!ReferenceEquals(parent, null)) syncPosition();

            lr.positionCount += 1;
            lr.SetPosition(lr.positionCount - 1, representation.transform.position); // draw trail

            // update connectors
            foreach (lineConnector lc in connectors) lc.lr.SetPositions(new Vector3[2]{lc.sPos, lc.ePos});
        }

        // im not very clear on how quaternions work, so this could prob be simplified
        representation.transform.rotation = Quaternion.Euler(obliquity * Mathf.Rad2Deg, 0, 0);
        representation.transform.rotation = Quaternion.AngleAxis(((master.globalTime % rotationalPeriod) / rotationalPeriod) * 360f, representation.transform.up) * representation.transform.rotation;

        // tell children to sync
        if (!ReferenceEquals(children, null)) foreach (PLANETCLASS child in children) child.syncPosition();
    }

    /// <summary> Subscriber to <see cref="master.onUnitMultiplierChanged"/></summary>
    /// <remarks> Changes the in-game position of the planet- does not change any backend values </remarks>
    public void OnUnitMultiChange(object sender, EventArgs args)
    {
        if (!ready) return; // only run after init has finished
        // increase in scale linearly increases unit size
        // this means that scale is basically just changing the radius of the sphere
        Transform parent = representation.transform.parent;
        representation.transform.parent = null;
        // unparenting so that localScale will be global scale
        representation.transform.localScale = helper.minSize(new Vector3(radius, radius, radius));
        if (orbit.active) representation.transform.localPosition = orbit.position / master.unitMultiplier;
        representation.transform.parent = parent;

        // clear trail
        lr.positionCount = 0;
        float width = representation.transform.localScale.x * 0.75f;
        lr.widthCurve = AnimationCurve.Linear(0, width, 1, width);
    }

    /// <summary> Updates current position to match/follow parents position </summary>
    public void syncPosition()
    {
        representation.transform.position += parent.representation.transform.position;
    }

}


public readonly struct ORBIT
{
    private readonly KEPLERORBIT kOrbit;
    private readonly POINTSORBIT pOrbit;
    public readonly int selectedOrbit; // 0 = kepler 1 = points
    public readonly bool active;
    public readonly dynamic info
    {
        get
        {
            if (this.selectedOrbit == 0) return kOrbit;
            else return pOrbit;
        }
    }
    public readonly Vector3 position
    {
        get 
        {
            if (selectedOrbit == 0) return kOrbit.position;
            else return pOrbit.position;
        }
    }

    public List<Vector3> requestPoints(float startTime, int amount, float timeStep)
    {
        List<Vector3> positions = new List<Vector3>();
        float time = startTime;
        for (int i = 0; i < amount; i++)
        {
            if (selectedOrbit == 0) positions.Add(kOrbit.atTime(time));
            else positions.Add(pOrbit.atTime(time));

            time += timeStep;
        }

        return positions;
    }

    public ORBIT(float e, float a, float i, float longOfAscNode, float argOfPeriapsis, float orbitalPeriod)
    {
        selectedOrbit = 0;
        kOrbit = new KEPLERORBIT(e, a, i, longOfAscNode, argOfPeriapsis, orbitalPeriod);
        pOrbit = new POINTSORBIT();
        active = true;
    }

    public ORBIT(string path, float timeIncrement)
    {
        selectedOrbit = 1;
        kOrbit = new KEPLERORBIT();
        pOrbit = new POINTSORBIT(path, timeIncrement);
        active = true;
    }
    public ORBIT(KEPLERORBIT k)
    {
        selectedOrbit = 0;
        kOrbit = k;
        pOrbit = new POINTSORBIT();
        active = k.active;
    }
    public ORBIT(POINTSORBIT p)
    {
        selectedOrbit = 1;
        kOrbit = new KEPLERORBIT();
        pOrbit = p;
        active = p.active;
    }

    public ORBIT(int i = 0)
    {
        active = false;
        selectedOrbit = -1;
        kOrbit = new KEPLERORBIT();
        pOrbit = new POINTSORBIT();
    }

    public static ORBIT empty() => new ORBIT();
}

// https://www.windows2universe.org/our_solar_system/planets_orbits_table.html
// math based upon kelpers laws of planetary motion
// assumes all bodies start at periapsis (periastron is 0)
/// <summary> 
/// Idealized orbit of a planet calculated with Keplerian elements
/// </summary>
/// <remarks>
///     <para>Due to how Keplerian orbits work, this class is not advised to be used in multi-body situtations </para>
///     <para>Related classes: <see cref="POINTSORBIT"/>, <see cref="PLANETCLASS"/></para>
/// </remarks>
public struct KEPLERORBIT
{
    /// <summary> Eccentricity </summary>
    public readonly float e;
    /// <summary> Semi-major axis (km) </summary>
    public readonly float a; 
    /// <summary> Inclination, with respect to the x plane in-game (rad) </summary>
    public readonly float i; 
    /// <summary> Longitude of the ascending node, currently is arbitary (rad) </summary>
    public readonly float longOfAscNode; 
    /// <summary> Argument of the periapsis, currently is arbitary (rad) </summary>
    public readonly float argOfPeriapsis; 
    /// <summary> Rotational/orbital period (s) </summary>
    public readonly float T;
    /// <summary> Current time elpased in orbit (s) </summary>
    public readonly float t {get {return this.internalTime % T;}}
    /// <summary> Mean anomaly (rad) </summary>
    public readonly float m {get {return (2f * Mathf.PI * (t - periastron)) / T;}}

    private float internalTime;

    /// <summary> True anomaly (rad) </summary>
    public readonly float v
    {
        get
        {
            // http://www.csun.edu/~hcmth017/master/node16.html
            float E = m;
            for (int i = 0; i < 50; i++)
            {
                E = (m + e * Mathf.Sin(E));
            }
            // https://drive.google.com/file/d/1so93guuhCO94PEU8vFvDLv_-k9vJBcFs/view?usp=sharing
            float trueAnomaly = 2f * Mathf.Atan(Mathf.Sqrt((1f + e) / (1f - e)) * Mathf.Tan(E / 2f));
            return trueAnomaly;
        }
    }
    /// <summary> Distance to focus (km) </summary>
    public readonly float r {get {return a * (1 - (e * e)) / (1 + e * Mathf.Cos(v));}}
    /// <summary> Current position at time <see cref="master.globalTime"/> (km) </summary>
    /// <remarks> This is recalculated every time it is called, so try not to call it often </remarks>
    public Vector3 position {get{return this.atTime(master.globalTime);}}

    public Vector3 atTime(float time)
    {
        this.internalTime = time;

        // http://www.stargazing.net/kepler/ellipse.html#twig04
        float vpo = v + longOfPeriapsis;
        float x = r * (Mathf.Cos(longOfAscNode) * Mathf.Cos(vpo) - Mathf.Sin(longOfAscNode) * Mathf.Sin(vpo) * Mathf.Cos(i));
        float y = r * (Mathf.Sin(longOfAscNode) * Mathf.Cos(vpo) + Mathf.Cos(longOfAscNode) * Mathf.Sin(vpo) * Mathf.Cos(i));
        float z = r * (Mathf.Sin(vpo) * Mathf.Sin(i));

        return new Vector3(x, z, y); // switch z and y to match unity coords
    }

    public void setInternalTime(float time)
    {
        this.internalTime = time;
    }

    /// <summary> Periastron, is currently set to 0 (rad) </summary>
    public readonly float periastron;
    /// <summary> Longitude of the periapsis (rad) </summary>
    public readonly float longOfPeriapsis;
    /// <summary> Semi-minor axis (km) </summary>
    public readonly float b;
    /// <summary> Controls if the orbit is initalized/should be used to calculated positions </summary>
    public readonly bool active;
    /// <summary> Main constructor for <see cref="ORBIT"/></summary>
    /// <remarks> argOfPeriapsis and longOfAscNode are currently arbitary </remarks>
    public KEPLERORBIT(float e, float a, float i, float longOfAscNode, float argOfPeriapsis, float orbitalPeriod)
    {
        this.e = e;
        this.a = a;
        this.i = i;
        this.longOfAscNode = longOfAscNode;
        this.argOfPeriapsis = argOfPeriapsis;
        this.T = orbitalPeriod;
        this.internalTime = master.globalTime;

        this.periastron = 0;

        // https://en.wikipedia.org/wiki/Longitude_of_the_periapsis
        float A = Mathf.Cos(argOfPeriapsis) * Mathf.Cos(longOfAscNode) - Mathf.Sin(argOfPeriapsis) * Mathf.Sin(longOfAscNode) * Mathf.Cos(i);
        float obliquity = 23.43929111f * Mathf.Deg2Rad;
        float aa = Mathf.Cos(argOfPeriapsis) * Mathf.Sin(longOfAscNode) + Mathf.Sin(argOfPeriapsis) * Mathf.Cos(longOfAscNode) * Mathf.Cos(i);
        float B = Mathf.Cos(obliquity) * (aa) - Mathf.Sin(obliquity) * Mathf.Sin(argOfPeriapsis) * Mathf.Sin(i);
        float C = Mathf.Sin(obliquity) * (aa) + Mathf.Cos(obliquity) * Mathf.Sin(argOfPeriapsis) * Mathf.Sin(i);

        float alpha = Mathf.Atan(B / A);
        if (A < 0) alpha += 180 * Mathf.Deg2Rad;
        float delta = Mathf.Asin(C);

        this.longOfPeriapsis = Mathf.Atan((Mathf.Sin(alpha) * Mathf.Cos(obliquity) + Mathf.Tan(delta) * Mathf.Sin(delta)) / Mathf.Cos(alpha));
        if (Mathf.Cos(alpha) < 0) this.longOfPeriapsis += 180 * Mathf.Deg2Rad;

        this.b = a * Mathf.Sqrt(1 - (e * e));
        active = true;
    }
    public KEPLERORBIT(kOrbitData k)
    {
        this.e = k.e;
        this.a = k.a;
        this.i = k.i;
        this.longOfAscNode = k.longOfAscNode;
        this.argOfPeriapsis = k.argOfPeriapsis;
        this.T = k.T;
        this.periastron = k.periastron;
        this.active = k.active;
        this.internalTime = master.globalTime;

        float A = Mathf.Cos(argOfPeriapsis) * Mathf.Cos(longOfAscNode) - Mathf.Sin(argOfPeriapsis) * Mathf.Sin(longOfAscNode) * Mathf.Cos(i);
        float obliquity = 23.43929111f * Mathf.Deg2Rad;
        float aa = Mathf.Cos(argOfPeriapsis) * Mathf.Sin(longOfAscNode) + Mathf.Sin(argOfPeriapsis) * Mathf.Cos(longOfAscNode) * Mathf.Cos(i);
        float B = Mathf.Cos(obliquity) * (aa) - Mathf.Sin(obliquity) * Mathf.Sin(argOfPeriapsis) * Mathf.Sin(i);
        float C = Mathf.Sin(obliquity) * (aa) + Mathf.Cos(obliquity) * Mathf.Sin(argOfPeriapsis) * Mathf.Sin(i);

        float alpha = Mathf.Atan(B / A);
        if (A < 0) alpha += 180 * Mathf.Deg2Rad;
        float delta = Mathf.Asin(C);

        this.longOfPeriapsis = Mathf.Atan((Mathf.Sin(alpha) * Mathf.Cos(obliquity) + Mathf.Tan(delta) * Mathf.Sin(delta)) / Mathf.Cos(alpha));
        if (Mathf.Cos(alpha) < 0) this.longOfPeriapsis += 180 * Mathf.Deg2Rad;

        this.b = a * Mathf.Sqrt(1 - (e * e));
    }
    /// <summary> Constructor to initalize an orbit as empty </summary>
    /// <remarks> Singular parameter is used in order to avoid CS0568 </remarks>
    public KEPLERORBIT(int i = 0)
    {
        this.e = 0;
        this.a = 0;
        this.i = 0;
        this.longOfAscNode = 0;
        this.argOfPeriapsis = 0;
        this.T = 0;
        this.periastron = 0;
        this.b = 0;
        this.longOfPeriapsis = 0;
        this.active = false;
        this.internalTime = 0;
    }

    /// <summary> Shorthand for initalizing an empty orbit </summary>
    public static KEPLERORBIT empty() => new KEPLERORBIT();

    public bool isEmpty()
    {
        // please find a better way to do this
        // implement a custom hashcode or something
        return this.e == 0 &&
            this.a == 0 &&
            this.i == 0 &&
            this.longOfAscNode == 0 &&
            this.argOfPeriapsis == 0 &&
            this.T == 0 &&
            this.periastron == 0 &&
            this.b == 0 &&
            this.longOfPeriapsis == 0 &&
            this.active == false;
    }
}

/// <summary>
/// Class that holds the data needed to display the planet in-game
/// </summary>
/// <remarks> 
///     <para>As such, data may be scaled/inaccurate</para>
///     <para>Should only be created by <see cref="PLANETCLASS"/></para>
///     <para>Related classes: <see cref="PLANETCLASS"/></para>
/// </remarks>
public readonly struct PHYSICALPLANET
{
    /// <summary> Gameobject that is being presented as the planet </summary>
    public readonly GameObject go;
    /// <summary> Shorthand for getting the Transform component of <see cref="PHYSICALPLANET.go"/></summary>
    public readonly Transform transform;
    /// <summary> Shorthand for getting the MeshRenderer component of <see cref="PHYSICALPLANET.go"/></summary>
    public readonly MeshRenderer mr;
    public readonly GameObject tag;
    public readonly string texturePath, modelPath;

    /// <summary> Main constructor for <see cref="PHYSICALPLANET"/></summary>
    public PHYSICALPLANET(GameObject go, string texturePath, string modelPath, string name)
    {
        this.go = go;
        this.go.AddComponent<LineRenderer>();
        this.go.AddComponent<planetClassDisplay>();
        this.transform = go.transform;
        this.modelPath = modelPath;
        this.mr = go.GetComponent<MeshRenderer>();
        this.texturePath = texturePath;

        this.tag = helperMono.inst(helper.textPrefab, Vector3.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("UI/tags").transform);
        this.tag.GetComponent<TextMeshProUGUI>().text = name.Split('-')[1];
    }
}

public readonly struct POINTSORBIT
{
    public readonly List<Vector3> positions;
    private readonly lagrangeInterp x, y, z;
    public readonly float timeIncrement;
    public readonly bool active;
    public readonly float maxTime, minTime;
    public Vector3 position
    {
        get 
        {
            if (master.globalTime > this.maxTime || master.globalTime < this.minTime)
            {
                Debug.Log("Warning: out of time");
                return Vector3.zero;
            }
            Vector3 pos = new Vector3(
                x.at(master.globalTime),
                y.at(master.globalTime),
                z.at(master.globalTime));
            return pos;
        }
    }

    // expects a csv file
    public POINTSORBIT(string path, float timeIncrement, float minTime = 0)
    {
        this.positions = helper.parseCSV(path);
        this.timeIncrement = timeIncrement;
        this.minTime = minTime;

        List<List<Vector2>> points = new List<List<Vector2>>() {new List<Vector2>(), new List<Vector2>(), new List<Vector2>()};
        int index = 0;
        foreach (Vector3 v in this.positions)
        {
            float time = index * timeIncrement;
            points[0].Add(new Vector2(time, v.x));
            points[1].Add(new Vector2(time, v.y));
            points[2].Add(new Vector2(time, v.z));

            index++;
        }
        this.maxTime = (index - 1) * timeIncrement;
        this.x = new lagrangeInterp(points[0], timeIncrement);
        this.y = new lagrangeInterp(points[1], timeIncrement);
        this.z = new lagrangeInterp(points[2], timeIncrement);

        active = true;
    }

    public POINTSORBIT(List<Vector3> data, float timeIncrement, float minTime = 0)
    {
        this.positions = data;
        this.timeIncrement = timeIncrement;
        this.minTime = minTime;

        List<List<Vector2>> points = new List<List<Vector2>>() {new List<Vector2>(), new List<Vector2>(), new List<Vector2>()};
        int index = 0;
        foreach (Vector3 v in this.positions)
        {
            float time = index * timeIncrement;
            points[0].Add(new Vector2(time, v.x));
            points[1].Add(new Vector2(time, v.y));
            points[2].Add(new Vector2(time, v.z));

            index++;
        }
        this.maxTime = (index - 1) * timeIncrement;
        this.x = new lagrangeInterp(points[0], timeIncrement);
        this.y = new lagrangeInterp(points[1], timeIncrement);
        this.z = new lagrangeInterp(points[2], timeIncrement);

        active = true;
    }

    public POINTSORBIT(pOrbitData p)
    {
        List<Vector3> positions = new List<Vector3>();
        p.positions.ForEach(x => positions.Add(x));
        this.positions = positions;
        this.timeIncrement = p.timeIncrement;
        this.maxTime = p.maxTime;
        this.active = p.active;
        this.minTime = p.minTime;

        List<List<Vector2>> points = new List<List<Vector2>>() {new List<Vector2>(), new List<Vector2>(), new List<Vector2>()};
        int index = 0;
        foreach (Vector3 v in this.positions)
        {
            float time = index * timeIncrement;
            points[0].Add(new Vector2(time, v.x));
            points[1].Add(new Vector2(time, v.y));
            points[2].Add(new Vector2(time, v.z));

            index++;
        }
        this.x = new lagrangeInterp(points[0], timeIncrement);
        this.y = new lagrangeInterp(points[1], timeIncrement);
        this.z = new lagrangeInterp(points[2], timeIncrement);
    }

    public Vector3 atTime(float time)
    {
        return new Vector3(
            x.at(time),
            y.at(time),
            z.at(time)
        );
    }

    public POINTSORBIT(int i = 0)
    {
        this.positions = new List<Vector3>();
        this.x = new lagrangeInterp(new List<Vector2>(), 0);
        this.y = new lagrangeInterp(new List<Vector2>(), 0);
        this.z = new lagrangeInterp(new List<Vector2>(), 0);
        this.timeIncrement = 0;
        this.maxTime = 0;
        this.minTime = 0;
        active = false;
    }

    public static POINTSORBIT empty() => new POINTSORBIT();

    public bool isEmpty()
    {
        return ReferenceEquals(this.positions, null) &&
            this.timeIncrement == 0 &&
            this.maxTime == 0 &&
            this.active == false;
    }
}