using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Airplane_Setup_Tools
{
    public static void BuildDefaultAirplane(string airplaneName)
    {
        // create the root GameObject
        GameObject rootGameObject = new GameObject(airplaneName, typeof(Airplane_Controller));

        // add the center of gravity GameObject
        GameObject cogGameObject = new GameObject("CenterOfGravity");
        cogGameObject.transform.SetParent(rootGameObject.transform, false);

        // create the base components or find them
        BaseAirplane_Input input = rootGameObject.GetComponent<BaseAirplane_Input>();
        Airplane_Controller controller = rootGameObject.GetComponent<Airplane_Controller>();
        Airplane_Characteristics characteristics = rootGameObject.GetComponent<Airplane_Characteristics>();

        // setup the airplane
        if (controller)
        {
            // assign core components
            controller.input = input;
            controller.characteristics = characteristics;
            controller.centerOfGravity = cogGameObject.transform;

            // create structure
            GameObject graphicsGroup = new GameObject("Graphics_GRP");
            GameObject collisionsGroup = new GameObject("Collisions_GRP");
            GameObject controlSurfacesGroup = new GameObject("ControlSurfaces_GRP");
            graphicsGroup.transform.SetParent(rootGameObject.transform, false);
            collisionsGroup.transform.SetParent(rootGameObject.transform, false);
            controlSurfacesGroup.transform.SetParent(rootGameObject.transform, false);

            // create the first engine
            GameObject engineGameObject = new GameObject("Engine", typeof(Airplane_Engine));
            engineGameObject.transform.SetParent(rootGameObject.transform, false);

            Airplane_Engine engine = engineGameObject.GetComponent<Airplane_Engine>();
            controller.engines.Add(engine);

            // create the base airplane
            GameObject defaultAirplane = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AircraftPhysics/Art/Objects/Airplanes/Indie-Pixel_Airplane/IndiePixel_Airplane.fbx", typeof(GameObject));
            if(defaultAirplane) {
                GameObject.Instantiate(defaultAirplane, graphicsGroup.transform);
            }
        }

        // select the airplane setup
        Selection.activeGameObject = rootGameObject;
    }

    public static void BuildGlider(string airplaneName)
    {
        Debug.Log("not yet implemented");
    }
}
