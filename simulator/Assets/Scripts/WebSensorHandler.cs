using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

/*
    Handles the connection between unity and the webserver to read all sensors values
 */
public class WebSensorHandler : MonoBehaviour
{
    public String adresse_ip;

    public float[] gyro;
    public int potentiometre;
    public bool gyroRecalibration;

    public int nbFailedRequestMax;

    private String[] requetes = { "lirePotentiometre", "lireButton", "lireGyro" };
    private bool connectionActive;
    private int timeoutCount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequests());
    }

    public void calibrerGyro() {
        String uri = "http://" + adresse_ip + "/calibrerGyro";
        gyroRecalibration = true;
        UnityWebRequest.Get(uri);
    }

    public bool isConnectionActive() {
        return connectionActive;
    }

    IEnumerator GetRequests()
    {
        while(true) {
            for (int index_requete = 0; index_requete < requetes.Length; index_requete++)
            {

                String uri = "http://" + adresse_ip + '/' + requetes[index_requete];

                using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                {
                    webRequest.timeout = 1;

                    // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isNetworkError)
                    {
                        timeoutCount++;

                        // if multiple request fail, we consider the conenction inactive
                        if(timeoutCount >= nbFailedRequestMax && connectionActive) {
                            connectionActive = false;
                            timeoutCount = 0;
                            Debug.LogError("Multiple request unanswered, setting connection to inactive");
                        }
                    }
                    else
                    {
                        connectionActive = true;
                        switch (index_requete)
                        {
                            //POTENTIOMETRE
                            case 0:
                                this.potentiometre = int.Parse(webRequest.downloadHandler.text);
                                break;

                            //BOUTON
                            case 1:
                                this.gyroRecalibration = (webRequest.downloadHandler.text == "1");
                                break;

                            //GYROSCOPE
                            case 2:
                                String gyroString = webRequest.downloadHandler.text;

                                // if gyro can't be read
                                if(gyroString == "-1") {
                                    this.gyro = null;
                                } else {
                                    gyroString = gyroString.Substring(1, gyroString.Length - 2);
                                    String[] gyroStringArray = gyroString.Split(',');

                                    gyro = new float[3];
                                    for (int i = 0; i < 3; i++)
                                        this.gyro[i] = float.Parse(gyroStringArray[i], CultureInfo.InvariantCulture);
                                }

                                break;
                        }
                    }
                }
            }
        }        
    }

}
