using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonController : MonoBehaviour {

    // Use this for initialization
    private GameController gc;
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseUp()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.ShowPauseMenu();
    }
}
