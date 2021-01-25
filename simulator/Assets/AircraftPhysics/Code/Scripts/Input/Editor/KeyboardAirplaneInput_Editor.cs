using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KeyboardAirplane_Input))]
public class KeyboardAirplaneInput_Editor : Editor
{
    private KeyboardAirplane_Input targetInput;

    void OnEnable()
    {
        targetInput = (KeyboardAirplane_Input)target;
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
