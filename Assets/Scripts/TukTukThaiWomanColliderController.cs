using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TukTukThaiWomanColliderController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "thai_woman")
        {
            other.gameObject.SetActive(false);
            WorldSceneController wsc = GameObject.Find("WorldScene").GetComponent<WorldSceneController>();
            wsc.DeSpawnThaiWoman();
        }
    }
}
