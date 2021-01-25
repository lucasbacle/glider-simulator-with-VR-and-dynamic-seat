using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_Camera : Basic_Follow_Camera
{
    [Header("Airplane Follow Camera Properties")]
    public float minHeightFromGround = 2f;
    private float originalHeight;

    void Start() {
        originalHeight = height;
    }

    protected override void HandleCamera()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.distance < minHeightFromGround && hit.transform.tag == "Ground")
            {
                float wantedHeight = originalHeight + (minHeightFromGround - hit.distance);
                height = wantedHeight;
            }
        }

        base.HandleCamera();
    }
}
