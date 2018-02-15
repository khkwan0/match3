using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerSceneController : MonoBehaviour {

    private GameObject ball;
    public float kickDelay;
	// Use this for initialization
	void Start () {
        ball = transform.Find("soccerball2").gameObject;
        InvokeRepeating("DoKick", 0f, kickDelay);
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void DoKick()
    {
        ball.GetComponent<SoccerballController>().Kick();
    }

}
