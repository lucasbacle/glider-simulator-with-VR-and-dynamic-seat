using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldManager : MonoBehaviour
{ 
    public GameObject[] maps;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maps.Length; i++)
            maps[i].SetActive(false);

        //maps[MultiSceneValues.worldID].SetActive(true);
    }

}
