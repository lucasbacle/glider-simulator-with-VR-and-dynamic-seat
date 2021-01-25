using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_Characteristics : MonoBehaviour
{
    [Header("Characteristics Properties")]
    [Tooltip("Forward speed is in meter per second.")]
    public float maxKMH = 175f;
    public float rbLerpSpeed = 0.03f;

    [Header("Lift Properties")]
    public float maxLiftPower = 800f;
    public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float flapsLiftPower = 12f;

    [Header("Drag Properties")]
    public float dragFactor = 0.0004f;
    public float angularDragFactor = 0.0004f;
    public float flapDragFactor = 0.0002f;

    [Header("Control Properties")]
    public float pitchSpeed = 2000f;
    public float rollSpeed = 2000f;
    public float yawSpeed = 2000f;
    public float bankFactor = 0.2f;
    public AnimationCurve controlSurfacesEfficiency = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Rigidbody rb;
    private BaseAirplane_Input input;
    private float forwardSpeed;
    private float kmh;
    private float startDrag;
    private float startAngularDrag;
    private float angleOfAttack;
    private float pitchAngle;
    private float rollAngle;
    private float maxMPS;
    private float normalizedKMH;
    private float csEfficiency;

    private const float mps2kmh = 3.6f;

    public void InitCharacteristics(Rigidbody currentRb, BaseAirplane_Input currentInput)
    {
        input = currentInput;
        rb = currentRb;
        startDrag = rb.drag;
        startAngularDrag = rb.angularDrag;

        maxMPS = maxKMH / mps2kmh;
    }

    public void UpdateCharacteristics()
    {
        if (rb)
        {
            HandleControlSurfaceEfficiency();

            HandlePitch();
            HandleRoll();
            HandleBanking();
            HandleYaw();

            CalculateForwardSpeed();
            CalculateLift();
            CalculateDrag();

            HandleRigidbodyTransform();
        }
    }

    void CalculateForwardSpeed()
    {
        // get air speed
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        forwardSpeed = Mathf.Max(0f, localVelocity.z);
        forwardSpeed = Mathf.Clamp(forwardSpeed, 0f, maxMPS);

        // convert to km/h
        kmh = forwardSpeed * mps2kmh;
        kmh = Mathf.Clamp(kmh, 0f, maxKMH);
        normalizedKMH = Mathf.InverseLerp(0f, maxKMH, kmh);
    }

    void CalculateLift()
    {
        angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);
        angleOfAttack *= angleOfAttack;

        Vector3 liftDir = transform.up;
        float liftPower = liftCurve.Evaluate(normalizedKMH) * maxLiftPower;
        float flapLiftPower = flapsLiftPower * input.NormalizedFlaps;

        Vector3 finalLiftForce = liftDir * (liftPower + flapLiftPower) * angleOfAttack;
        rb.AddForce(finalLiftForce);
    }

    void CalculateDrag()
    {
        // rigidbody drag
        float speedDrag = forwardSpeed * dragFactor;
        float flapDrag = input.Flaps * flapDragFactor;
        float finalDrag = startDrag + speedDrag + flapDrag;
        rb.drag = finalDrag;

        // angular drag
        //float speedAngularDrag = forwardSpeed * angularDragFactor;
        //float finalAngularDrag = startAngularDrag + speedAngularDrag;
        //rb.angularDrag = finalAngularDrag;

        rb.angularDrag = startAngularDrag * forwardSpeed;
    }

    void HandleControlSurfaceEfficiency()
    {
        csEfficiency = controlSurfacesEfficiency.Evaluate(normalizedKMH);
    }

    void HandlePitch()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;
        flatForward = flatForward.normalized;
        pitchAngle = Vector3.Angle(transform.forward, flatForward);

        Vector3 pitchTorque = input.Pitch * pitchSpeed * transform.right * csEfficiency;
        rb.AddTorque(pitchTorque);
    }

    void HandleRoll()
    {
        Vector3 flatRight = transform.right;
        flatRight.y = 0f;
        flatRight = flatRight.normalized;
        rollAngle = Vector3.SignedAngle(transform.right, flatRight, transform.forward);

        Vector3 rollTorque = -input.Roll * rollSpeed * transform.forward * csEfficiency;
        rb.AddTorque(rollTorque);
    }

    void HandleYaw()
    {
        Vector3 yawTorque = input.Yaw * yawSpeed * transform.up * csEfficiency;
        rb.AddTorque(yawTorque);
    }

    void HandleBanking()
    {
        float bankSide = Mathf.InverseLerp(-90f, 90f, rollAngle);
        float bankAmout = Mathf.Lerp(-1f, 1f, bankSide);
        Vector3 bankTorque = bankAmout * rollSpeed * transform.up * bankFactor;
        rb.AddTorque(bankTorque);
    }

    // Smooth movements
    void HandleRigidbodyTransform()
    {
        if (rb.velocity.magnitude > 1f)
        {
            Vector3 updatedVelocity = Vector3.Lerp(rb.velocity, transform.forward * forwardSpeed, forwardSpeed * angleOfAttack * Time.deltaTime * rbLerpSpeed);
            rb.velocity = updatedVelocity;

            Quaternion updatedRotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity.normalized, transform.up), Time.deltaTime * rbLerpSpeed);
            rb.MoveRotation(updatedRotation);
        }
    }
}
