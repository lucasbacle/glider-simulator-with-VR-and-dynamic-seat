using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane_Propeller : MonoBehaviour
{
    [Header("Propeller Properties")]
    public float minQuadRPM = 300f;
    public float minTextureSwapRPM = 600f;
    public GameObject mainProp;
    public GameObject blurredProp;

    [Header("Propeller Properties")]
    public Material blurredPropMat;
    public Texture2D blurLevel1;
    public Texture2D blurLevel2;


    void Start()
    {
        // set the correct propeller initial look
        HandleSwapping(0f);
    }

    public void HandlePropeller(float currentRPM)
    {
        float degreesPerSecond = ((currentRPM * 360f) / 60f) * Time.deltaTime;

        transform.Rotate(Vector3.forward, degreesPerSecond);

        HandleSwapping(currentRPM);
    }

    void HandleSwapping(float currentRPM)
    {
        if (mainProp && blurredProp)
        {
            if (currentRPM > minQuadRPM)
            {
                blurredProp.gameObject.SetActive(true);
                mainProp.gameObject.SetActive(false);

                if (blurredPropMat && blurLevel1 && blurLevel2)
                {
                    if (currentRPM > minTextureSwapRPM)
                    {
                        blurredPropMat.SetTexture("_MainTex", blurLevel2);

                    }
                    else
                    {
                        blurredPropMat.SetTexture("_MainTex", blurLevel1);
                    }
                }

            }
            else
            {
                blurredProp.gameObject.SetActive(false);
                mainProp.gameObject.SetActive(true);
            }
        }
    }
}
