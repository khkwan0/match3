using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostPanelController : MonoBehaviour {

    public GameObject backToWorldButtonPrefab;
    private GameObject backToWorldBUtton;

	void Start () {
        backToWorldBUtton = GameObject.Instantiate(backToWorldButtonPrefab, transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
