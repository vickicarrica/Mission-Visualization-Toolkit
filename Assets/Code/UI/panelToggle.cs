using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelToggle : MonoBehaviour
{
    public GameObject defaultOpen, defaultClose;
    public float time, moveLength;

    public void open()
    {
        StartCoroutine(move(-1, defaultOpen));
        StartCoroutine(move(2, defaultClose));
    }

    public void close()
    {
        StartCoroutine(move(-2, defaultClose));
        StartCoroutine(move(1, defaultOpen));
    }

    private IEnumerator move(float multi, GameObject go)
    {
        // ideal length: 1107
        for (float i = 0; i < 100; i++)
        {
            go.transform.position += new Vector3(moveLength * 0.01f * multi * (Screen.width / 1107f), 0, 0);
            yield return new WaitForSecondsRealtime(time * 0.01f);
        }
    }
}
