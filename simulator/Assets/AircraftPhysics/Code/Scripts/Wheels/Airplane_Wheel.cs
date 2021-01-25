using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Airplane_Wheel : MonoBehaviour
{
    [Header("Wheel Properties")]
    public Transform wheelGraphic;
    public bool isBraking = false;
    public float brakePower = 5f;
    public bool isSteering = false;
    public float steerAngle = 20f;
    public float steerSmoothSpeed = 2f;

    private WheelCollider WheelCollider;
    private Vector3 worldPos;
    private Quaternion worldRot;
    private float finalBreakTorque;
    private float finalSteerAngle;

    void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    public void InitWheel()
    {
        if (WheelCollider)
        {
            // make the wheels collider able to roll freely
            WheelCollider.motorTorque = 0.0000000000001f;
        }
    }

    public void HandleWheel(BaseAirplane_Input input)
    {
        if (WheelCollider)
        {
            WheelCollider.GetWorldPose(out worldPos, out worldRot);

            if (wheelGraphic)
            {
                wheelGraphic.rotation = worldRot;
                wheelGraphic.position = worldPos;
            }

            if (isBraking)
            {
                if (input.Brake > 0.1f)
                {
                    finalBreakTorque = Mathf.Lerp(finalBreakTorque, input.Brake * brakePower, Time.deltaTime);
                    WheelCollider.brakeTorque = finalBreakTorque;
                }
                else
                {
                    // make the wheels collider able to roll freely
                    WheelCollider.motorTorque = 0.0000000000001f;
                    WheelCollider.brakeTorque = 0f;
                    finalBreakTorque = 0f;
                }
            }

            if (isSteering)
            {
                finalSteerAngle = Mathf.Lerp(finalSteerAngle,  -input.Yaw * steerAngle, Time.deltaTime * steerSmoothSpeed);
                WheelCollider.steerAngle = finalSteerAngle;
            }
        }
    }
}
