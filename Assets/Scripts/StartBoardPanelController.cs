using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StartBoardPanelController : MonoBehaviour {


    public GameObject StartPanelStartButtonPrefab;
    public GameObject StartPanelExitButtonPrefab;

    private GameObject spsb;
    private GameObject speb;

	// Use this for initialization
	void Start () {
        spsb = GameObject.Instantiate(StartPanelStartButtonPrefab, transform);
        speb = GameObject.Instantiate(StartPanelExitButtonPrefab, transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    public void SetText(string text)
    {
        TextMeshProUGUI tmpgui = GameObject.FindGameObjectWithTag("StartPanelText").GetComponent<TextMeshProUGUI>();

        tmpgui.text = text;
    }
    
    public void AppendText(string text)
    {
        TextMeshProUGUI tmpgui = GameObject.FindGameObjectWithTag("StartPanelText").GetComponent<TextMeshProUGUI>();

        tmpgui.text = tmpgui.text + text;
    }

}
