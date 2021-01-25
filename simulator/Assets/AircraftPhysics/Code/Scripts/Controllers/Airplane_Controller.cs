using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Airplane_Characteristics))]
public class Airplane_Controller : BaseRigidBody_Controller
{
    [Header("Airplane Preset")]
    public Airplane_Preset airplanePreset;

    [Header("Base Airplane Properties")]
    public BaseAirplane_Input input;
    public Airplane_Characteristics characteristics;
    public Transform centerOfGravity;
    [Tooltip("Weight is in kilos.")]
    public float airplaneWeight = 300f;

    [Header("Engines")]
    public List<Airplane_Engine> engines = new List<Airplane_Engine>();

    [Header("Wheels")]
    public List<Airplane_Wheel> wheels = new List<Airplane_Wheel>();

    [Header("Control Surfaces")]
    public List<Airplane_Control_Surface> controlSurfaces = new List<Airplane_Control_Surface>();

    public override void Start()
    {
        LoadAirplanePreset();

        base.Start();

        if (rb)
        {
            rb.mass = airplaneWeight;
            if (centerOfGravity)
            {
                rb.centerOfMass = centerOfGravity.localPosition;
            }

            characteristics = GetComponent<Airplane_Characteristics>();
            if (characteristics)
            {
                characteristics.InitCharacteristics(rb, input);
            }
        }

        if (wheels != null && wheels.Count > 0)
        {
            foreach (Airplane_Wheel wheel in wheels)
            {
                wheel.InitWheel();
            }
        }
    }

    protected override void HandlePhysics()
    {
        if (input)
        {
            HandleEngines();
            HandleCharacteristics();
            HandleControlSurfaces();
            HandleWheels();
            HandleAltitude();
        }
    }

    void HandleEngines()
    {
        if (engines != null && engines.Count > 0)
        {
            foreach (Airplane_Engine engine in engines)
            {
                rb.AddForce(engine.CalculateForce(input.Throttle));
            }
        }
    }

    void HandleCharacteristics()
    {
        if (characteristics)
        {
            characteristics.UpdateCharacteristics();
        }
    }

    void HandleControlSurfaces()
    {
        if (controlSurfaces.Count > 0)
        {
            foreach (Airplane_Control_Surface controlSurface in controlSurfaces)
            {
                controlSurface.HandleControlSurface(input);
            }
        }
    }

    void HandleWheels()
    {
        if (wheels.Count > 0)
        {
            foreach (Airplane_Wheel wheel in wheels)
            {
                wheel.HandleWheel(input);
            }
        }
    }

    void HandleAltitude()
    {

    }

    void LoadAirplanePreset()
    {
        if (airplanePreset)
        {
            airplaneWeight = airplanePreset.airplaneWeight;
            centerOfGravity.localPosition = airplanePreset.centerOfGravity;

            if (characteristics)
            {
                characteristics.maxKMH = airplanePreset.maxKMH;
                characteristics.rbLerpSpeed = airplanePreset.rbLerpSpeed;
                characteristics.maxLiftPower = airplanePreset.maxLiftPower;
                characteristics.liftCurve = airplanePreset.liftCurve;
                characteristics.flapsLiftPower = airplanePreset.flapsLiftPower;
                characteristics.dragFactor = airplanePreset.dragFactor;
                characteristics.angularDragFactor = airplanePreset.angularDragFactor;
                characteristics.flapDragFactor = airplanePreset.flapDragFactor;
                characteristics.pitchSpeed = airplanePreset.pitchSpeed;
                characteristics.rollSpeed = airplanePreset.rollSpeed;
                characteristics.yawSpeed = airplanePreset.yawSpeed;
                characteristics.controlSurfacesEfficiency = airplanePreset.controlSurfacesEfficiency;
            }
        }
    }
}
