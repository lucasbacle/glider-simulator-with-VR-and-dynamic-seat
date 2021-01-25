using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Follow_Camera : MonoBehaviour
{
    [Header("Basic Follow Camera Properties")]
    public Transform target;
    public float distance = 10f;
    public float height = 6f;
    public float smoothSpeed = 0.5f;

    private Vector3 smoothVelocity;

    void FixedUpdate()
    {
        if (target)
        {
            HandleCamera();
        }
    }

    protected virtual void HandleCamera()
    {
        Vector3 wantedPosition = target.position + (-target.forward * distance) + (Vector3.up * height);
        transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref smoothVelocity, smoothSpeed);

        transform.LookAt(target);
    }
}
