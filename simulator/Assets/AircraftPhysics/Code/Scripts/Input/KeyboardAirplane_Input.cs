using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAirplane_Input : BaseAirplane_Input
{
    public float throttleSpeed = 0.1f;

    protected override void HandleInput()
    {
        // process main inputs
        pitch = Input.GetAxis("Vertical");
        roll = Input.GetAxis("Horizontal");
        yaw = Input.GetAxis("Yaw");
        airbrake = Input.GetAxis("Airbrake");

        // process throttle lever
        float measuredThrottle = Input.GetAxis("Throttle");
        throttle += (measuredThrottle * throttleSpeed * Time.deltaTime);
        throttle = Mathf.Clamp01(throttle);

        // process wheel brake
        brake = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        // process flaps
        if (Input.GetKeyDown(KeyCode.F))
        {
            flaps += 1;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            flaps -= 1;
        }
        flaps = Mathf.Clamp(flaps, 0, maxFlapsIncrements);
    }
}