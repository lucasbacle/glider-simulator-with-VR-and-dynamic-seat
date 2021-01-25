using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Airplane_Menus
{
    [MenuItem("Airplane Tools/Create New Airplane/Default")]
    public static void CreateNewAirplane()
    {
        Airplane_Setup_Window.LaunchSetupWindow();
    }

    [MenuItem("Airplane Tools/Create New Airplane/Glider")]
    public static void CreateNewGlider()
    {
        Airplane_Setup_Tools.BuildGlider("New Glider");
    }
}
