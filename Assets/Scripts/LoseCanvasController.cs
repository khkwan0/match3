using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseCanvasController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.Find("Panel").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/square_panel_256x256");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
