using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingScoreController : MonoBehaviour {

    // Use this for initialization
    private Vector3 currentPos;
    public float upSpeed;

	void Start () {
        currentPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        currentPos.y += upSpeed;
        transform.position = currentPos;
	}
}
