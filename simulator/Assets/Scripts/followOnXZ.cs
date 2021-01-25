using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Follow the driver on x and z coordinates but keeps y and rotation the same 
 */
public class followOnXZ : MonoBehaviour
{
    public GameObject driver;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = driver.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
