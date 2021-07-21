using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using Newtonsoft.Json;
using SFB;
using System.Linq;

public class uiController : MonoBehaviour
{
    public uiTimeController tController;
    void Update()
    {
        Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0);
        foreach (PLANETCLASS pc in master.planets.Values)
        {
            GameObject go = pc.representation.tag;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pc.representation.transform.position) - (screenSize / 2f);
            screenPos /= this.GetComponent<Canvas>().scaleFactor;
            go.GetComponent<RectTransform>().anchoredPosition = screenPos;
            // (values are arb)
            // 5 - 36 (5000 -> 100) distance
            // 0.45 - 1 (50_000 -> 400_000) radius
            float fontSize = 0;
            float d = helper.distance(pc.representation.transform.position, Camera.main.transform.position);
            float r = pc.radius;
            if ((d > 10 && r < 500) || screenPos.z < 0 || d + 2 < pc.radius / master.unitMultiplier) fontSize = 0;
            else
            {
                d = Mathf.Min(Mathf.Max(d, 100), 5000);
                r *= r;
                r = Mathf.Min(Mathf.Max(r, 50_000), 400_000);
                fontSize = -0.00632653f * d + 36.6327f;
                fontSize *= 0.00000157f * r + 0.37142f;
            }
            TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
            t.fontSize = fontSize;
            //t.text = $"{pc.name.Split('-')[1]} ({d * master.unitMultiplier}km)";
        }
    }

    public void importCallback()
    {
        loadFile();
    }

    private void loadFile(bool lookAt = true)
    {
        List<string> paths = new List<string>();
        try
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Select Data File", "", "json", true).ToList();
            foreach (string path in paths)
            {
                load(path, lookAt);
            }
        }
        catch
        {
            StartCoroutine(sendError("Invalid File"));
        }
    }

    private void load(string path, bool lookAt)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string json = sr.ReadToEnd();
            string[] directories = path.Split(Path.DirectorySeparatorChar);
            string name = directories[directories.Length - 1].Split('.')[0];
            if (master.buildings.ContainsKey(name) || master.planets.ContainsKey(name))
            {
                StartCoroutine(sendError("File is already loaded/in storage"));
            }
            else
            {
                string type = name.Split('-')[0];
                if (type == "planet")
                {
                    planetData pd = JsonConvert.DeserializeObject<planetData>(json);
                    PLANETCLASS p = new PLANETCLASS(pd);
                    if (lookAt) StartCoroutine(timeout(2, p));

                    if (tController.timeEnabled) tController.toggleTimeEnabled();
                }
                else if (type == "facility")
                {
                    facilityData fd = JsonConvert.DeserializeObject<facilityData>(json);
                    new FACILITYCLASS(fd);
                }
            }
        }
    }

    public static void forceLookAtPlanet(PLANETCLASS p)
    {
        Camera.main.transform.parent.transform.position = p.representation.go.transform.position;
        Camera.main.transform.localPosition = new Vector3(Mathf.Max(p.radius / master.unitMultiplier, 0.01f) * 2 + 1, 0, 0);
        Camera.main.transform.LookAt(p.representation.go.transform);
    }

    public void exportCallback()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save Data Folder", "", "MVT_Data", "");
        Directory.CreateDirectory(path);
        foreach (planetData pd in systemData.pData.Values)
        {
            string json = JsonConvert.SerializeObject(pd, Formatting.Indented);
            // assumes running in unity editor
            File.WriteAllText(path + Path.DirectorySeparatorChar + $"{pd.name}.json", json);
        }
        foreach (facilityData fd in systemData.bData.Values)
        {
            string json = JsonConvert.SerializeObject(fd, Formatting.Indented);
            File.WriteAllText(path + Path.DirectorySeparatorChar + $"{fd.name}.json", json);
        }
    }   
    public void createCallback()
    {

    }

    public void compassCallback()
    {
        playerController pc = Camera.main.transform.gameObject.GetComponent<playerController>();
        pc.setFollow(pc.follow);
    }

    public void perspectiveCallback()
    {
        Debug.Log("called");
    }

    public void infoCallback()
    {
        
    }


    public void playCallback()
    {
        tController.toggleTimeEnabled();
    }

    public void forwardCallback()
    {
        tController.addTime();
    }

    public void backCallback()
    {
        tController.subTime();
        
    }

    public IEnumerator sendError(string text)
    {
        GameObject go = GameObject.FindGameObjectWithTag("UI/errorMsg");
        TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
        t.text = text;
        StartCoroutine(fade(1, 25, t));
        yield return new WaitForSeconds(3f);
        StartCoroutine(fade(-1, 25, t));
    }

    private IEnumerator fade(float totalChange, float time, TextMeshProUGUI text)
    {
        for (float i = 0; i < time; i++)
        {
            text.color = new Color(text.color.r, text.color.b, text.color.b, text.color.a + (totalChange / time));
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator timeout(int frames, PLANETCLASS p)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        uiController.forceLookAtPlanet(p);
    }
}
