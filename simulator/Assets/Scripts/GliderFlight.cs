using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderFlight : MonoBehaviour
{
    public GameObject webSensorHandler;
    private WebSensorHandler sensors;
    private Rigidbody rb;

    public float thrust; // acceleration
    public float acceleration; // thrust change rate
    public float minThrust, maxThrust; // bornes du thrust
    public float aireAiles; // coefficient de portance des ailes
    public float mass; // masse (en kg)
    public float drag; // resistance pour la position
    public float angularDrag; // resistance pour la rotation
    public float defaultHeight; // default height for the glider

    public float torqueMultipier;

    private float speed;             // vitesse de l'avion relative au sol//
    private float relativeSpeed;     // vitesse de l'avion locale //

    private float portanceAiles;    // portance
    private float rollAxis, pitchAxis, yawAxis;  // axes de rotation de l'avion
    private float desiredThrust;


    // Use this for initialization
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = drag;

        sensors = webSensorHandler.GetComponent<WebSensorHandler>();
        sensors.calibrerGyro();
    }

    // Update is called once per frame
    void Update()
    {
        float[] gyroValues = sensors.gyro;

        if (gyroValues == null || gyroValues.Length == 0)
        {
            // if gyro can't be read, return to a stable place
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = new Vector3(transform.localPosition.x, defaultHeight, transform.localPosition.z);
        } else {
            rollAxis = (gyroValues[2] / 3.14f); // from -1 to 1
            pitchAxis = (gyroValues[1] / 3.14f); // from -1 to 1
            yawAxis = (gyroValues[0] / 3.14f); // from -1 to 1
        }

        //rollAxis = pitchAxis = yawAxis = 0;

        float angle = 1f - (sensors.potentiometre / 360f);
        desiredThrust = Mathf.Lerp(minThrust, maxThrust, angle);
    }

    void FixedUpdate()
    {
        Vector3 speedV3 = rb.velocity;
        speed = speedV3.magnitude * 3.6f; // vitesse de l'avion en km/h

        Vector3 relativeSpeedV3 = Vector3.Project(rb.velocity, transform.forward);
        relativeSpeed = relativeSpeedV3.magnitude * 3.6f; // vitesse de l'avion dans l'axe de déplacement

        //Gizmos de debug
        Debug.DrawRay(transform.position, speedV3, Color.blue);
        Debug.DrawRay(transform.position, relativeSpeedV3, Color.red);

        // ROLL
        float speedCube = relativeSpeed * relativeSpeed;
        float roll = rollAxis * torqueMultipier; // définition de torque
        float pitch = pitchAxis * torqueMultipier; // définition de pitch

        //float incidence = Mathf.Abs(Vector3.Dot(transform.forward, Vector3.up));
        portanceAiles = speedCube * aireAiles;

        //Empeche la portance d'être plus forte que la gravité
        if(portanceAiles > Physics.gravity.magnitude) {
            portanceAiles = Physics.gravity.magnitude;
        }

        // set the trust to the correct value if close enough
        if (Math.Abs(thrust - desiredThrust) <= acceleration)
            thrust = desiredThrust;

        // smoothly approaches the desired thrust
        if (thrust < desiredThrust) {
            thrust += acceleration;
        } else if (thrust > desiredThrust) {
            thrust -= acceleration;
        }

        Vector3 totalForces = (transform.forward * thrust) + (transform.up * portanceAiles);
        Vector3 totalTorque = (transform.forward * roll) + (transform.right * pitch);
        //totalTorque += (transform.up * roll); // rotate on the y axis

        transform.Rotate(new Vector3(0,-roll/20, 0));

        rb.AddForce(totalForces);
        rb.AddTorque(totalTorque);
    }

}
