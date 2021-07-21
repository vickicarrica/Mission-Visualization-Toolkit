using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class cone
{
    public float resolution, scale, length;
    public Vector3 position;
    public Quaternion rotation;
    private float xOffset = 10;
    private GameObject representation;
    private List<Vector3> outline;

    public cone(float resolution, float scale, float length, Vector3 pos, Quaternion rot)
    {
        this.resolution = resolution;
        this.position = pos;
        this.rotation = rot;
        this.scale = scale;
        this.length = length;

        representation = helperMono.inst(helper.conePrefab, Vector3.zero, Quaternion.identity);
        redraw();
    }

    public List<GameObject> findInsides()
    {
        HashSet<GameObject> rGos = new HashSet<GameObject>();

        // check if object is on edge
        foreach (Vector3 v in outline)
        {
            List<RaycastHit> hits = Physics.RaycastAll(position, v, Mathf.Infinity, (1 << 6) | (1 << 7)).ToList();
            foreach (RaycastHit rch in hits)
            {
                GameObject hGo = rch.transform.gameObject;
                rGos.Add(findBestParent(hGo.transform));
            }
        }

        // check if object is inside
        // optimize -> check parents only, and skip children if parents are not applicable
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (!go.scene.isLoaded || !go.activeInHierarchy) continue;
            if (go.layer != 6 && go.layer != 7) continue;

            if (insideCone(go.transform.position)) rGos.Add(findBestParent(go.transform));
        }

        return rGos.ToList();
    }

    // iterate upwards until a visiblityCheckable object is reached
    private GameObject findBestParent(Transform checking)
    {
        while (true)
        {
            if (checking.parent == null) return null;
            if (checking.gameObject.layer == 7) return checking.gameObject;

            checking = checking.parent.transform;
        }
    }

    private List<Vector3> drawOutline()
    {
        List<Vector3> positions = new List<Vector3>();
        for (float i = 0; i < 360f; i += this.resolution)
        {
            float angle = i * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(xOffset, Mathf.Cos(angle) * scale, Mathf.Sin(angle) * scale);
            // set length
            float distance = helper.distance(position, pos);
            float dMulti = length / distance;
            pos *= dMulti;
            // rotate
            pos = rotation * pos;
            // offset
            pos += position;

            positions.Add(pos);
        }

        return positions;
    }

    private void drawMesh(List<Vector3> _vertices)
    {
        int[] tri = new int[_vertices.Count * 3];

        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < _vertices.Count; i++)
        {
            vertices.Add(position);
            vertices.Add(_vertices[i]);
            vertices.Add(_vertices[(i + 1 >= _vertices.Count?0:i + 1)]);
        }

        for (int i = 0; i < vertices.Count; i+=3)
        {
            tri[i] = i+2;
            tri[i+1] = i+1;
            tri[i+2] = i;
        }

        Mesh m = new Mesh();

        m.vertices = vertices.ToArray();
        m.triangles = tri;

        representation.GetComponent<MeshFilter>().mesh = m;
        m.RecalculateNormals();
        m.Optimize();

        representation.GetComponent<MeshRenderer>().material = helper.coneTexture;
        if (representation.GetComponent<coneDisplay>() == null) representation.AddComponent<coneDisplay>().parent = this;
    }

    private bool insideCone(Vector3 point)
    {
        // center on (0,0) and remove rotation
        point -= position;
        point = Quaternion.Inverse(rotation) * point;

        float slope = scale / xOffset;

        float radius = Mathf.Max(slope * point.x, 0);

        if (radius == 0) return false;

        // get vertex at radius
        Vector3 vert = new Vector3(point.x, 0, 0);

        float distance = helper.distance(vert, point);

        // outside circle
        if (distance > radius) return false;
        return true;
    }

    public void redraw()
    {
        this.outline = drawOutline();
        drawMesh(this.outline);
    }
}
