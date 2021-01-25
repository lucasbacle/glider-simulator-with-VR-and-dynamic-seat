using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAirplane_Input : MonoBehaviour
{
    [SerializeField] protected int maxFlapsIncrements = 3;

    protected float pitch = 0f;
    protected float roll = 0f;
    protected float yaw = 0f;
    protected float throttle = 0f;
    protected float airbrake = 0f;
    protected float brake = 0f;
    protected int flaps = 0;

    public float Pitch
    {
        get { return pitch; }
    }

    public float Roll
    {
        get { return roll; }
    }

    public float Yaw
    {
        get { return yaw; }
    }

    public float Throttle
    {
        get { return throttle; }
    }

    public float Flaps
    {
        get { return flaps; }
    }

    public float NormalizedFlaps
    {
        get
        {
            if (maxFlapsIncrements <= 0)
            {
                return 0;
            }

            return flaps / (float)maxFlapsIncrements;
        }
    }

    public float Brake
    {
        get { return brake; }
    }

    public float Airbrake
    {
        get { return airbrake; }
    }

    void Start()
    {

    }

    void Update()
    {
        HandleInput();
    }

    protected abstract void HandleInput();
}


