using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class orbitType : MonoBehaviour
{
    public createMaster cm;
    public TMP_Dropdown dd;
    public GameObject kepler, point;

    public void onChange(int value)
    {
        // kepler = 0
        // point = 1
        if (value == 0)
        {
            cm.keplerSelected = true;
            point.SetActive(false);
            kepler.SetActive(true);
        }
        else
        {
            cm.keplerSelected = false;
            point.SetActive(true);
            kepler.SetActive(false);
        }
    }
}
