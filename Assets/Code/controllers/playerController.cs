using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class playerController : MonoBehaviour
{
    public float playerSpeed = 10;
    public PLANETCLASS follow {get; private set;}
    public TextMeshProUGUI movementDisplay;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Transform c = Camera.main.transform;
            c.Rotate(0, Input.GetAxis("Mouse X") * 500 * Time.deltaTime, 0);
            c.Rotate(-Input.GetAxis("Mouse Y") * 500 * Time.deltaTime, 0, 0);
            c.localEulerAngles = new Vector3(c.localEulerAngles.x, c.localEulerAngles.y, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        if (Input.GetKey("w")) Camera.main.transform.localPosition += forward * playerSpeed * Time.deltaTime;
        if (Input.GetKey("s")) Camera.main.transform.localPosition -= forward * playerSpeed * Time.deltaTime;
        if (Input.GetKey("d")) Camera.main.transform.localPosition += right * playerSpeed * Time.deltaTime;
        if (Input.GetKey("a")) Camera.main.transform.localPosition -= right * playerSpeed * Time.deltaTime;

        if (Input.mouseScrollDelta.y != 0)
        {
            if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift))
            {
                playerSpeed += Input.mouseScrollDelta.y * 50f;
                playerSpeed = Mathf.Max(Mathf.Min(playerSpeed, 10_000), 10);
            }
            else Camera.main.transform.localPosition += forward * Input.mouseScrollDelta.y / 10f;
        }

        movementDisplay.text = $"Movement Speed: {playerSpeed}";
    }

    void Awake()
    {
        master.onGlobalTimeChanged += updateCameraFollow;
    }

    public void updateCameraFollow(object sender, EventArgs e)
    {
        if (follow != null) Camera.main.transform.parent.transform.position = follow.representation.go.transform.position;
    }

    public event EventHandler followingChanged = delegate {};

    public void setFollow(PLANETCLASS p)
    {
        followingChanged(null, EventArgs.Empty);
        follow = p;
    }

    public void removeFollow()
    {
        followingChanged(null, EventArgs.Empty);
        follow = null;
    }
}
