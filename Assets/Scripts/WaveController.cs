using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public float speed = 0.005f;
    private Vector3 wavesPosition;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        wavesPosition = transform.position;
        wavesPosition.x = Mathf.Sin(Time.fixedTime) * speed;
        transform.position = wavesPosition;
    }
}
