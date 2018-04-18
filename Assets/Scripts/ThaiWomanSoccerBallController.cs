using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThaiWomanSoccerBallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Kick()
    {
        transform.parent.GetComponent<ThaiWomanSoccerBallSceneController>().DoKick();
    }   
}
