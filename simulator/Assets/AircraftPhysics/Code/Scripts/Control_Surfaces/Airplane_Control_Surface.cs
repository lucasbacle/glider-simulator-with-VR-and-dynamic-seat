using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlSurfaceType
{
    Rudder,
    Elevator,
    Aileron,
    Flap
}

public class Airplane_Control_Surface : MonoBehaviour
{
    [Header("Control Surface Properties")]
    public ControlSurfaceType type = ControlSurfaceType.Rudder;
    public float maxAngle = 30f;
    public Transform controlSurfaceGameObject;
    public Vector3 axis = Vector3.right;
    public float smoothSpeed = 4f;

    private float wantedAngle;

    void Update()
    {
        if (controlSurfaceGameObject)
        {
            Vector3 finalAngleAxis = axis * wantedAngle;
            controlSurfaceGameObject.localRotation = Quaternion.Slerp(controlSurfaceGameObject.localRotation, Quaternion.Euler(finalAngleAxis), Time.deltaTime * smoothSpeed);
        }
    }

    public void HandleControlSurface(BaseAirplane_Input input)
    {
        float inputValue = 0f;

        switch (type)
        {
            case ControlSurfaceType.Rudder:
                inputValue = input.Yaw;
                break;
            case ControlSurfaceType.Elevator:
                inputValue = input.Pitch;
                break;
            case ControlSurfaceType.Aileron:
                inputValue = input.Roll;
                break;
            case ControlSurfaceType.Flap:
                inputValue = input.NormalizedFlaps;
                break;
            default:
                break;
        }

        wantedAngle = maxAngle * inputValue;
    }
}
