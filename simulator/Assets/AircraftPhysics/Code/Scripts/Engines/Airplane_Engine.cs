using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_Engine : MonoBehaviour
{
    public float maxForce = 200f;
    public float maxRPM = 2550f;
    public AnimationCurve powerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Propeller")]
    public Airplane_Propeller propeller;

    public Vector3 CalculateForce(float throttle)
    {
        // compute power
        float finalThrottle = Mathf.Clamp01(throttle);
        finalThrottle = powerCurve.Evaluate(finalThrottle);
        float finalPower = finalThrottle * maxForce;

        // compute rpm
        if (propeller)
        {
            float currentRPM = finalThrottle * maxRPM;
            propeller.HandlePropeller(currentRPM);
        }

        // compute force
        Vector3 finalForce = transform.forward * finalPower;

        return finalForce;
    }
}
