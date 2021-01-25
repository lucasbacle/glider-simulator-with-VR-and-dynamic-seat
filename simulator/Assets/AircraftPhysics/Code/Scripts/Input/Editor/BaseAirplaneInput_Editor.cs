﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseAirplane_Input))]
public class BaseAirplaneInput_Editor : Editor
{
    private BaseAirplane_Input targetInput;

    void OnEnable()
    {
        targetInput = (BaseAirplane_Input)target;
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
        debugInfo += "Flaps = " + targetInput.Flaps + "\n";

        EditorGUILayout.TextArea(debugInfo);
        EditorGUILayout.Space();

        Repaint();
    }
}
