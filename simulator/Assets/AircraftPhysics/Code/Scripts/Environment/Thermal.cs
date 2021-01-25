using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Thermal : MonoBehaviour
{
    public float thermalHeight = 50f;
    public float thermalDiameter = 20f;
    public float thermalPower = 2000f;

    BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(thermalDiameter, thermalHeight, thermalDiameter);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            other.attachedRigidbody.AddForce(Vector3.up * thermalPower);
        }
    }
}
