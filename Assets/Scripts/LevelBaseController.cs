using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBaseController : MonoBehaviour {

    // Use this for initialization
    public int level;
    private GameController gc;

	void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseUp()
    {
        gc.LoadLevel(level);
        //Debug.Log(level);
    }
}
