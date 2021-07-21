using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helperMono : MonoBehaviour
{
    public static GameObject inst(GameObject go, Vector3 pos, Quaternion rot, Transform parent = null) => Instantiate(go, pos, rot, parent);
    public static lineConnector instLineConnector(PLANETCLASS start, PLANETCLASS end, float startR, float endR)
    {
        GameObject go = Instantiate(helper.linePrefab, start.representation.transform);
        go.tag = "physicalUI/lineConnection";
        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.material = helper.lineDiff;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[2]{
            start.representation.transform.position,
            end.representation.transform.position
        });
        lr.startWidth = 0.005f;
        lr.endWidth = 0.005f;

        lineConnector lc = new lineConnector(lr, start, end, startR, endR);

        return lc;
    }

    public void Start()
    {
        helper.mono = this.GetComponent<helperMono>();
    }
}
