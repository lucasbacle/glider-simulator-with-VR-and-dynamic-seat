using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
    Controls the info panel in front of the camera if the gyro is getting recalibrated
 */

public class InfoPanel : MonoBehaviour
{
    private Canvas canvas;
    private WebSensorHandler webSensorHandlerScript;
    public GameObject webSensorHandler;

    public Text uiText;

    private Boolean recalibrating;

    void Start()
    {
        this.canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        recalibrating = false;
        webSensorHandlerScript = webSensorHandler.GetComponent<WebSensorHandler>();
    }

    void Update()
    {
        //if the recalibration button is pressed
        if(webSensorHandlerScript.gyroRecalibration) {
            recalibrating = true;
        }
        
        if(recalibrating) {
            canvas.enabled = true;
            uiText.text = "Recalibration du gyroscope... \n \n Gardez la barre immobile";
            if(webSensorHandlerScript.gyro != null) {
                recalibrating = false;
                canvas.enabled = false;
            }
        }
    }
}
