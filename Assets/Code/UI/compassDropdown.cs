using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class compassDropdown : MonoBehaviour
{
    public void Awake()
    {
        master.planets.OnChange += update;
    }

    public TMP_Dropdown dd;
    public int type;
    public Dictionary<int, string> options = new Dictionary<int, string>();
    public void update(object sender, EventArgs e)
    {
        dd.ClearOptions();
        options = new Dictionary<int, string>() {
            {0, "-"}
        };

        int index = 1;
        foreach (string name in master.planets.Keys)
        {
            options.Add(index, name.Split('-')[1]);
            index++;
        }

        dd.AddOptions(options.Values.ToList());
    }

    // type: 0
    public void onSelectFollow(int value)
    {
        playerController pc = Camera.main.transform.gameObject.GetComponent<playerController>();
        if (value == 0) pc.removeFollow();
        else pc.setFollow(master.planets[$"planet-{options[value]}"]);
    }

    // type: 1
    public void onSelectFrame(int value)
    {
        foreach (PLANETCLASS planet in master.planets.Values) planet.lr.positionCount = 0;
        if (value == 0) master.referenceFrame = null;
        else master.referenceFrame = master.planets[$"planet-{options[value]}"].representation.go;
    }
}
