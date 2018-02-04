using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostPanelController : MonoBehaviour {

    private GameController gc;
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BackToWorld()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.BackToWorld();
    }
}
