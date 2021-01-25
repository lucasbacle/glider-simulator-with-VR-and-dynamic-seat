using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
    This class is responsible for all the glider movement and reset the glider position when it crashes
*/

public class GliderMovement : MonoBehaviour
{
    public GameObject webSensorHandler;
    public GameObject crashedCameraTarget;

    public AudioSource engineSound;

    public float rotXSpeed, rotZSpeed;
    public float maxZRot;
    public float minSpeed, maxSpeed;
    public float sensibility;

    private enum gliderState { FLYING, CRASHED, PAUSED }
    private gliderState state;
    private bool isCrashed;
    private float speed;
    private Vector3 gliderRotation;
    private WebSensorHandler sensors;
    private GameObject camera;
    private float rotX, rotZ;

    private Vector3 defaultCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        state = gliderState.PAUSED;
        camera = transform.GetChild(1).gameObject;
        defaultCameraPosition = camera.transform.localPosition;
        gliderRotation = Quaternion.ToEulerAngles(transform.rotation);
        sensors = webSensorHandler.GetComponent<WebSensorHandler>();
    }

    void FixedUpdate()
    {
        float[] gyroValues = sensors.gyro;

        engineSound.pitch = 1 + (speed / maxSpeed)/2;

        // change the state depending on gyro values and isCrashed
        if(!isCrashed) {
            if (gyroValues == null || gyroValues.Length == 0)
            {
                state = gliderState.PAUSED;
            }
            else
            {
                state = gliderState.FLYING;
            }
        } else {
            state = gliderState.CRASHED;
        }

        switch (state) {
            case gliderState.FLYING:
                // moves the glider forward
                transform.Translate(0, 0, speed * Time.deltaTime, Space.Self);
                applyRotation();

                //set the speed according to the sensors
                float angle = 1f - (sensors.potentiometre / 360f);
                speed = Mathf.Lerp(minSpeed, maxSpeed, angle);
                break;

            case gliderState.CRASHED:
                speed = 0;
                float cameraMovementSpeed = 1.5f;
                Vector3 targetPoint = crashedCameraTarget.transform.position;

                Rigidbody rb = GetComponent<Rigidbody>();
                rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;

                // slowly move the camera to the target point
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPoint, cameraMovementSpeed * Time.deltaTime);
                if (Vector3.Distance(camera.transform.position, targetPoint) < 0.1f)
                {
                    resetPosition();
                    camera.transform.localPosition = defaultCameraPosition;
                    state = gliderState.FLYING;
                    isCrashed = false;
                }
                break;

            case gliderState.PAUSED:
                resetPosition();
                break;
        }
    }

    // keeps the same position on x and z but moves the glider 50 units up from the ground
    public void resetPosition() {
        gliderRotation = Vector3.zero;
        transform.rotation = Quaternion.Euler(gliderRotation);

        float x = transform.localPosition.x;
        float z = transform.localPosition.z;

        RaycastHit hit;
        bool worked = Physics.Raycast(new Vector3(x, 10000, z), Vector3.down, out hit, float.MaxValue);

        float defaultHeight;
        if (worked)
            defaultHeight = hit.point.y + 50;
        else {
            defaultHeight = WorldGenerator.defaultGliderHeight;
        }

        transform.localPosition = new Vector3(x, defaultHeight, z);
    }

    void OnCollisionEnter(Collision collision)
    {
        this.isCrashed = true;
    }

    private void applyRotation() {
        float[] gyroValues = sensors.gyro;

        //conversion en degres
        Vector3 gyroRotation = new Vector3((float) (gyroValues[0] * 180 / Math.PI), (float) (gyroValues[1] * 180 / Math.PI), (float) (gyroValues[2] * 180 / Math.PI));

        rotX = rotXSpeed * Time.deltaTime * Math.Abs(gyroRotation.y);
        rotZ = rotZSpeed * Time.deltaTime * Math.Abs(gyroRotation.z);

        //up down
        if (gyroRotation.y > sensibility)
        {
            gliderRotation.x += rotX;
        }
        else if (gyroRotation.y < -sensibility)
        {
            gliderRotation.x -= rotX;
        }

        //side to side
        if (gyroRotation.z > sensibility)
        {
            if(gliderRotation.z < maxZRot) {
                gliderRotation.z += rotZ * 2;
            } else {
                gliderRotation.z = maxZRot;
            }

            gliderRotation.y -= rotZ * 1;
        }
        else if (gyroRotation.z < -sensibility)
        {
            if (gliderRotation.z > -maxZRot) {
                gliderRotation.z -= rotZ * 2;
            } else {
                gliderRotation.z = -maxZRot;
            }

            gliderRotation.y += rotZ * 1;
        }


        transform.rotation = Quaternion.Euler(gliderRotation);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
