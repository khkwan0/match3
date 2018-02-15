using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerballController : MonoBehaviour {

    private Rigidbody rb;
    public float min, max;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Kick()
    {
        rb.AddForce(Random.Range(1f, 5f), Random.Range(1f, 5f), 0f);
        rb.velocity = new Vector3(Random.Range(min, max), Random.Range(min, max), 0f);
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayKickSound();
    }

    public void OnCollisionEnter(Collision collision)
    {

    }
}
