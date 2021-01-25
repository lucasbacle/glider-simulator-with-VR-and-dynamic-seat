using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Airplane_Setup_Window : EditorWindow
{
    private string airplaneName;

    public static void LaunchSetupWindow() {
        Airplane_Setup_Window.GetWindow(typeof(Airplane_Setup_Window), true, "Airplane Setup").Show();
    }

    void OnGUI() {
        airplaneName = EditorGUILayout.TextField("Airplane Name:", airplaneName);
        if(GUILayout.Button("Create new airplane")) {
            Airplane_Setup_Tools.BuildDefaultAirplane(airplaneName);
            Close();
        }
    }
}
