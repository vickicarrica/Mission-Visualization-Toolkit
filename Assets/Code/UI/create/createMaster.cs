using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System;
using System.IO;
using SFB;

public class createMaster : MonoBehaviour
{
    public TMP_Dropdown modelDrop, textureDrop, orbitTypeDrop, axisDrop, parentDrop;
    public TMP_InputField e, a, i, orbitalPeriod, radius, tilt, revolutionPeriod, timeIncrement, givenName;
    public bool keplerSelected = true;
    public Button generate, importFile, resetFile;
    public string csv;
    public Toggle inverted;

    public void Start()
    {
        givenName.onValueChanged.AddListener(delegate {validateName();});

        initModels(new List<TMP_Dropdown>() {modelDrop});
        initTextures(new List<TMP_Dropdown>() {textureDrop});
        master.planets.OnChange += planetAdditionCallback;

        orbitTypeDrop.AddOptions(new List<string>() {"Kepler", "Points"});
        axisDrop.AddOptions(new List<string>() {"XYZ", "ZYX", "XZY", "ZXY", "YXZ", "YZX"});

        generate.onClick.AddListener(delegate {generateCallback();});
        importFile.onClick.AddListener(delegate {fileCallback();});
        resetFile.onClick.AddListener(delegate {resetFileCallback();});
    }

    public void validateName()
    {
        List<char> blacklist = new List<char>() {
            '\\', '/', '-', ' '
        };

        StringBuilder newName = new StringBuilder();
        foreach (char c in givenName.text)
        {
            if (blacklist.Any(x => c == x)) newName.Append('_');
            else newName.Append(c);
        }

        givenName.SetTextWithoutNotify(newName.ToString());
    }

    public void initModels(List<TMP_Dropdown> dds)
    {
        List<string> names = new List<string>();
        Resources.LoadAll("prefabs/models", typeof(GameObject)).ToList().ForEach(x => names.Add(((GameObject) x).name));

        foreach (TMP_Dropdown dd in dds)
        {
            dd.ClearOptions();
            dd.AddOptions(names);
        }
    }

    public void initTextures(List<TMP_Dropdown> dds)
    {
        List<string> names = new List<string>();
        Resources.LoadAll("materials/textures", typeof(Material)).ToList().ForEach(x => names.Add(((Material) x).name));

        foreach (TMP_Dropdown dd in dds)
        {
            dd.ClearOptions();
            dd.AddOptions(names);
        }
    }

    public void planetAdditionCallback(object sender, EventArgs e)
    {
        parentDrop.ClearOptions();

        List<string> names = new List<string>() {"-"};
        foreach (string n in master.planets.Keys) names.Add(n.Split('-')[1]);
        parentDrop.AddOptions(names);
    }

    public bool attemptGeneratePlanet()
    {
        List<TMP_InputField> allReq = new List<TMP_InputField>()
        {
            radius, tilt, revolutionPeriod, givenName
        };

        List<TMP_InputField> keplerReq = new List<TMP_InputField>()
        {
            e, a, i, orbitalPeriod
        };

        List<TMP_InputField> pointReq = new List<TMP_InputField>()
        {
            timeIncrement
        };

        foreach (TMP_InputField inp in allReq) {if (!inputValid(inp)) { return false;}}
        if (keplerSelected) {foreach (TMP_InputField kInp in keplerReq) { if (!inputValid(kInp)) { return false;}}}
        else {foreach (TMP_InputField pInp in pointReq) { if (!inputValid(pInp)) { return false;}}}
        if (!keplerSelected && csv == "") return false;
        
        ORBIT o = ORBIT.empty();
        if (!keplerSelected)
        {
            // change axis into XZY
            Dictionary<string, Vector3> axises = new Dictionary<string, Vector3>()
            {
                {"XYZ", new Vector3(0, 1, 2)},
                {"ZYX", new Vector3(2, 1, 0)},
                {"XZY", new Vector3(0, 2, 1)},
                {"ZXY", new Vector3(2, 0, 1)},
                {"YXZ", new Vector3(1, 0, 2)},
                {"YZX", new Vector3(1, 2, 0)}
            };

            List<Vector3> positions = helper.parseCSV(csv);
            List<Vector3> filtered = new List<Vector3>();
            Vector3 ax = axises[axisDrop.options[axisDrop.value].ToString()];
            float multi = (inverted.isOn)?-1:1;

            Debug.Log("verify that the coord system is correct here");
            foreach (Vector3 pos in positions)
            {
                filtered.Add(new Vector3(
                    pos[(int) ax.x] * multi,
                    pos[(int) ax.y] * multi,
                    pos[(int) ax.z] * multi
                ));
            }

            o = new ORBIT(new POINTSORBIT(filtered, (float) Convert.ToDecimal(timeIncrement.text)));
        }
        else
        {
            o = new ORBIT(
                (float) Convert.ToDecimal(e.text),
                (float) Convert.ToDecimal(a.text),
                (float) Convert.ToDecimal(i.text),
                0,
                0,
                (float) Convert.ToDecimal(orbitalPeriod.text)
            );
        }

        PLANETCLASS pc = new PLANETCLASS(
            givenName.text,
            (float) Convert.ToDecimal(radius.text),
            Vector3.zero,
            0,
            (float) Convert.ToDecimal(tilt.text),
            (float) Convert.ToDecimal(revolutionPeriod.text),
            o,
            parent: (parentDrop.value == 0)?null:master.planets[$"planet-{parentDrop.options[parentDrop.value]}"],
            texturePath: textureDrop.options[textureDrop.value].ToString(),
            modelPath: modelDrop.options[modelDrop.value].ToString()
        );

        // planet creation

        return true;
    }

    public void generateCallback()
    {
        if (!attemptGeneratePlanet())
        {
            // do something
            Debug.Log("invalid generation attempt");
        }
    }

    private bool inputValid(TMP_InputField inp)
    {
        return (inp.text != "") && (inp.text != "-");
    }

    public void fileCallback()
    {
        string path = StandaloneFileBrowser.OpenFilePanel("Select Data File", "", "csv", false).ToList()[0];
        // do some validation
        if (path != "")
        {
            TextMeshProUGUI txt = importFile.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshProUGUI>();
            List<string> _p = path.Split(Path.DirectorySeparatorChar).ToList();
            string p = _p[_p.Count - 1];

            txt.text = p;
            csv = path;
        }
    }

    public void resetFileCallback()
    {
        TextMeshProUGUI txt = importFile.transform.GetChild(0).transform.gameObject.GetComponent<TextMeshProUGUI>();
        txt.text = "Upload File";
        csv = "";
    }
}
