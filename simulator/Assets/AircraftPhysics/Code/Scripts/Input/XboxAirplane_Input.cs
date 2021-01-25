using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxAirplane_Input : BaseAirplane_Input
{
    public float throttleSpeed = 0.1f;

    protected override void HandleInput()
    {
        // process main inputs
        pitch = Input.GetAxis("Vertical");
        roll = Input.GetAxis("Horizontal");
        yaw = Input.GetAxis("X_RH_Stick");
        airbrake = Input.GetAxis("Airbrake");

        // process throttle lever
        float measuredThrottle = Input.GetAxis("X_RV_Stick");
        throttle += (measuredThrottle * throttleSpeed * Time.deltaTime);
        throttle = Mathf.Clamp01(throttle);

        // process wheel brake
        brake = Input.GetAxis("Fire1");

        // process flaps
        if (Input.GetButtonDown("X_R_Bumper"))
        {
            flaps += 1;
        }
        if (Input.GetButtonDown("X_L_Bumper"))
        {
            flaps -= 1;
        }
        flaps = Mathf.Clamp(flaps, 0, maxFlapsIncrements);
    }
}

