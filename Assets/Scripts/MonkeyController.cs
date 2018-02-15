using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyController : MonoBehaviour {

    public GameObject bananaPrefab;
    public float throwRepeatDelay;
    private GameObject monkeyArm;
	// Use this for initialization
	void Start () {
        monkeyArm = transform.Find("ArmPivot").gameObject;
        InvokeRepeating("DoThrow", 0f, throwRepeatDelay);
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void DoThrow()
    {
        monkeyArm.GetComponent<MonkeyArmController>().Throw();
    }
}
