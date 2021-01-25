using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Airplane_Controller))]
public class Airplane_Controller_Editor : Editor
{
    private Airplane_Controller targetController;

    void OnEnable()
    {
        targetController = (Airplane_Controller)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(15);
        if (GUILayout.Button("Attach existing airplane components", GUILayout.Height(20)))
        {
            targetController.engines.Clear();
            targetController.engines = FindAll<Airplane_Engine>();

            targetController.controlSurfaces.Clear();
            targetController.controlSurfaces = FindAll<Airplane_Control_Surface>();

            targetController.wheels.Clear();
            targetController.wheels = FindAll<Airplane_Wheel>();
        }

        if (GUILayout.Button("Save Airplane Preset", GUILayout.Height(20)))
        {
            string filePath = EditorUtility.SaveFilePanel("Save Airplane Preset", "Assets", "New_Airplane_Preset", "asset");
            SaveAirplanePreset(filePath);
        }
        GUILayout.Space(15);
    }

    private List<T> FindAll<T>()
    {
        var foundComponents = new List<T>();

        if (targetController)
        {
            foundComponents = targetController.transform.GetComponentsInChildren<T>(true).ToList();
        }

        return foundComponents;
    }

    void SaveAirplanePreset(string path)
    {
        if (targetController && !string.IsNullOrEmpty(path))
        {
            string appPath = Application.dataPath;
            string finalPath = "Assets" + path.Substring(appPath.Length);

            // create new preset
            Airplane_Preset newPreset = ScriptableObject.CreateInstance<Airplane_Preset>();
            
            // save airplane controller properties
            newPreset.airplaneWeight = targetController.airplaneWeight;

            if (targetController.centerOfGravity)
            {
                newPreset.centerOfGravity = targetController.centerOfGravity.localPosition;
            }

            // save airplane characteristic controller
            if (targetController.characteristics)
            {
                newPreset.maxKMH = targetController.characteristics.maxKMH;
                newPreset.rbLerpSpeed = targetController.characteristics.rbLerpSpeed;
                newPreset.maxLiftPower = targetController.characteristics.maxLiftPower;
                newPreset.liftCurve = targetController.characteristics.liftCurve;
                newPreset.flapsLiftPower = targetController.characteristics.flapsLiftPower;
                newPreset.dragFactor = targetController.characteristics.dragFactor;
                newPreset.angularDragFactor = targetController.characteristics.angularDragFactor;
                newPreset.flapDragFactor = targetController.characteristics.flapDragFactor;
                newPreset.pitchSpeed = targetController.characteristics.pitchSpeed;
                newPreset.rollSpeed = targetController.characteristics.rollSpeed;
                newPreset.yawSpeed = targetController.characteristics.yawSpeed;
                newPreset.controlSurfacesEfficiency = targetController.characteristics.controlSurfacesEfficiency;
            }

            // save preset
            AssetDatabase.CreateAsset(newPreset, finalPath);
        }
    }

}
