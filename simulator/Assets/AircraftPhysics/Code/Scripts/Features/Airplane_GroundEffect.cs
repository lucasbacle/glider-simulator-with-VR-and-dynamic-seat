using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_GroundEffect : MonoBehaviour
{
    public float maxGroundEffectDistance = 3f;
    public float liftForce = 100f;
    public float maxSpeed = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxGroundEffectDistance = CalculateWingspan(rb);
    }

    void FixedUpdate()
    {
        if (rb)
        {
            HandleGroundEffect();
        }
    }

    // wingspan is usually the longest dimension of an airplane
    float CalculateWingspan(Rigidbody rb)
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in rb.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }

        return Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
    }

    void HandleGroundEffect()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "Ground" && hit.distance < maxGroundEffectDistance)
            {
                float currentSpeed = rb.velocity.magnitude;
                float normalizedSpeed = currentSpeed / maxSpeed;
                normalizedSpeed = Mathf.Clamp01(normalizedSpeed);

                float distance = maxGroundEffectDistance - hit.distance;
                float finalForce = liftForce * distance * normalizedSpeed;
                rb.AddForce(Vector3.up * finalForce);
            }
        }
    }
}
