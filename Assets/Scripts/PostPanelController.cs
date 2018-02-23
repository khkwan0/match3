using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostPanelController : MonoBehaviour {

    public GameObject gui;
    public GameObject starsPanel;



    public void SetText(string textString)
    {
        if (textString != null && gui)
        {
            gui.GetComponent<TextMeshProUGUI>().text = textString;
        }
    }


}
