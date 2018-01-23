using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSoundTrigger : MonoBehaviour {

    public GameObject boingPrefab;
    private GameObject boing;

	// Use this for initialization
	void Start () {
        boing = GameObject.Instantiate(boingPrefab);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        boing.GetComponent<AudioSource>().Play();
    }
}
