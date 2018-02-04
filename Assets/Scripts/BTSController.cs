using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSController : MonoBehaviour {

    public GameObject sfxPrefab;
    private GameObject sfx;

	// Use this for initialization
	void Start () {
        sfx = GameObject.Instantiate(sfxPrefab);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += Vector3.right * .125f;
	}
}
