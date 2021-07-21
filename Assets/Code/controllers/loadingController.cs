using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadingController : MonoBehaviour
{
    void Start()
    {
        // helper mono init itself
        controller.init();

        StartCoroutine(sleep()); // hack
    }

    private IEnumerator sleep()
    {
        yield return new WaitForEndOfFrame();
        master.forceUpdateGlobalVars();
    }
}
