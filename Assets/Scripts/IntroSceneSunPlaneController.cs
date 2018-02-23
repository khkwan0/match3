using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneSunPlaneController : MonoBehaviour {

    public GameObject sun;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.name == "monkey_sm")
        {
            sun.GetComponent<Rigidbody>().velocity = new Vector3(0f, 5f, 0f);
        }
    }
}
