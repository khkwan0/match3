using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdColliderScriptForWorldScene : MonoBehaviour {

    private string target;

    public string Target
    {
        get { return target; }
        set { target = value; }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnTriggerEnter(Collider other)
    {
        if (other.name == target)
        {
            BirdController bird = other.GetComponent<BirdController>();
            bird.checkBounds = false;
            bird.rotate_90 = false;
            bird.Descend(20f, 60f, 0.05f);
        }
    }
}
