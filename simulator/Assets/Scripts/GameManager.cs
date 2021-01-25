using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject glider;
    public GameObject world;
    public GameObject enterIPCanvas;

    private GameObject gliderModel;
    private bool isRunning;

    private WebSensorHandler sensors;

    // Start is called before the first frame update
    void Start()
    {
        this.sensors = FindObjectOfType<WebSensorHandler>();
        this.gliderModel = glider.transform.GetChild(0).gameObject;
        this.isRunning = true;

        stopGame();
    }

    private void stopGame() {
        gliderModel.SetActive(false); // disable the pglider model
        glider.GetComponent<GliderMovement>().enabled = false; // disable the glider movement
        world.SetActive(false); // disable the world
        enterIPCanvas.SetActive(true); // enable connection UI
    }

    // Update is called once per frame
    void Update()
    {
        if(!sensors.isConnectionActive() && isRunning) {
            // if connection is lost
            stopGame();
            isRunning = false;
        } else if(sensors.isConnectionActive()) {
            // if connection with the sensors is working again
            if (!isRunning) {
                gliderModel.SetActive(true); // enable glider model
                glider.GetComponent<GliderMovement>().enabled = true; // enable the glider movement
                world.SetActive(true); // enable the world
                enterIPCanvas.SetActive(false); // enable the connection UI
            }

            isRunning = true;
        }

        // if back is pressed on the remote, return to last known scene
        if(OVRInput.Get(OVRInput.RawButton.Back)) {
            SceneManager.LoadScene("mainMenu");
        }
    }
}
