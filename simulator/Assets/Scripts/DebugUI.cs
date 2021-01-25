using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

/*
    This class allows to show a small text panel in front of the glider to enable simple debugging in VR
 */
public class DebugUI : MonoBehaviour
{

    public Text uiText;
    private static string debugText;

    // Start is called before the first frame update
    void Start()
    {
        if (uiText != null)
        {
            uiText.supportRichText = false;
        }

        debugText = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (uiText != null)
        {
            uiText.text = debugText;
        }
    }

    public static void setText(string text) {
        debugText = text;
    }
}
