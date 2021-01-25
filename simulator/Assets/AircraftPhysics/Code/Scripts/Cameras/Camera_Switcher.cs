using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Switcher : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera rearCamera;

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void ShowRearView()
    {
        firstPersonCamera.enabled = false;
        rearCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowCockpitView()
    {
        firstPersonCamera.enabled = true;
        rearCamera.enabled = false;
    }

    public void Start()
    {
        ShowRearView();
    }

    public void Update()
    {
        if (Input.GetAxis("CameraSwitch") == 1)
        {
            ShowCockpitView();
        } else {
            ShowRearView();
        }
    }
}
