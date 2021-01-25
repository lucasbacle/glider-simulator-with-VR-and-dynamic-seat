using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendGliderHeight : MonoBehaviour
{

    public GameObject webSensorHandler;
    public int n;
    private WebSensorHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        handler = webSensorHandler.GetComponent<WebSensorHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // don't send request on every n th frame to avoid crashing the web server
        if(Time.frameCount % n == 0) {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, float.PositiveInfinity);
            float h = hit.distance;

            Debug.DrawLine(transform.position, hit.point);
        
            string ip = handler.adresse_ip;
            StartCoroutine(SendGetRequest("http://" + ip + "/gliderHeight?h=" + h));
        }
    }

    IEnumerator SendGetRequest(string uri)
    {
        UnityWebRequest req = UnityWebRequest.Get(uri);
        yield return req.SendWebRequest();        

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
    }
}
