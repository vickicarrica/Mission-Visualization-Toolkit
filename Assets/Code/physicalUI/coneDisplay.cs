using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coneDisplay : MonoBehaviour
{
    public cone parent;
    public float resolution, scale, length;
    public Vector3 position, rotation;
    public bool update = false, checkInside = false;

    public void Start()
    {
        this.resolution = parent.resolution;
        this.scale = parent.scale;
        this.length = parent.length;
        this.position = parent.position;
        this.rotation = parent.rotation.eulerAngles;
    }
    public void Update()
    {
        if (update)
        {
            update = false;
            parent.resolution = resolution;
            parent.scale = scale;
            parent.length = length;
            parent.position = position;
            parent.rotation = Quaternion.Euler(rotation);
            
            parent.redraw();
        }
        if (checkInside)
        {
            checkInside = false;
            List<GameObject> gos = parent.findInsides();

            foreach (GameObject go in gos)
            {
                Debug.Log(go.name);
            }
        }
    }
}
