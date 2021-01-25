using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    Manages the ip UI to conenct to the server, both the headset and the sensors server must be connectd on the same network
 */
public class EnterIPUI : MonoBehaviour
{
    public Text ip;
    public Text statusText;
    private WebSensorHandler sensors;

    // Start is called before the first frame update
    void Start()
    {
        this.sensors = FindObjectOfType<WebSensorHandler>();
        ip.text = sensors.adresse_ip;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onKeyboardButtonClick(Button b) {
        statusText.text = "";
        string buttonText = b.GetComponentInChildren<Text>().text;

        if(buttonText == "<=") {
            if(ip.text.Length > 0) {
                ip.text = ip.text.Substring(0, ip.text.Length - 1);
            }
        } else {
            ip.text += buttonText;
        }
    }

    public void onClick() {
        string ip = this.ip.text;
        IPAddress ipAdress;
        int timeout = 2;
        
        // if the text is a valid ip address
        if(IPAddress.TryParse(ip, out ipAdress)) {
            sensors.adresse_ip = ipAdress.ToString();

            statusText.text = "Connecting to server...";

            Invoke("connectionStatusMessage", timeout);
        } else {
            statusText.text = "Please enter a valid IP address";
        }
    }

    private void connectionStatusMessage() {
        if (!sensors.isConnectionActive())
        {
            statusText.text = "Cannot connect to the server";
        } else {
            statusText.text = "Connected !";
        }
    }
}
