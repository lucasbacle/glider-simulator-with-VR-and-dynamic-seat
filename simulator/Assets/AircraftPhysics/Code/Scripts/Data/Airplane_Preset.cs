using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Airplane Tools/Create Airplane Preset")]
public class Airplane_Preset : ScriptableObject
{
    [Header("Controller Properties")]
    public Vector3 centerOfGravity;
    public float airplaneWeight;

    [Header("Characteristics Properties")]
    public float maxKMH;
    public float rbLerpSpeed;
    public float maxLiftPower;
    public AnimationCurve liftCurve;
    public float flapsLiftPower;
    public float dragFactor;
    public float angularDragFactor;
    public float flapDragFactor;
    public float pitchSpeed;
    public float rollSpeed;
    public float yawSpeed;
    public AnimationCurve controlSurfacesEfficiency;
}
