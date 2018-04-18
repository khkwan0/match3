using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverViewController : MonoBehaviour {
    public GameObject elephant;

    void Start () {

        InvokeRepeating("Hind", 5f, 15f);
	}
	
	private void Hind()
    {
        elephant.GetComponent<ElephantController>().Hind();
    }
}
