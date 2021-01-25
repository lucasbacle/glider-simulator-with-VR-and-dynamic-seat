using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicalControllerAirplane_Input))]
public class PhysicalControllerAirplaneInput_Editor : Editor
{
    private PhysicalControllerAirplane_Input targetInput;

    void OnEnable()
    {
        targetInput = (PhysicalControllerAirplane_Input)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // custom editor code
        EditorGUILayout.Space();

        string debugInfo = "";

        debugInfo += "Pitch = " + targetInput.Pitch + "\n";
        debugInfo += "Roll = " + targetInput.Roll + "\n";
        debugInfo += "Yaw = " + targetInput.Yaw + "\n";
        debugInfo += "Throttle = " + targetInput.Throttle + "\n";
        debugInfo += "Airbrake = " + targetInput.Airbrake + "\n";
        debugInfo += "Brake = " + targetInput.Brake + "\n";
        debugInfo += "Flaps = " + targetInput.NormalizedFlaps + "\n";

        EditorGUILayout.TextArea(debugInfo);
        EditorGUILayout.Space();

        Repaint();
    }
}