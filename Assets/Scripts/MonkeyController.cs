using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyController : MonoBehaviour {

    public GameObject bananaPrefab;
    public float throwRepeatDelay;
    private GameObject monkeyArm;

    GameController.DelegateHandleSceneEvent handler;
	// Use this for initialization
	void Start () {
        monkeyArm = transform.Find("ArmPivot").gameObject;
        InvokeRepeating("DoThrow", 0f, throwRepeatDelay);
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (gc)
        {
            handler = DoThrow;
            gc.RegisterHandler(handler);
        }
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void DoThrow()
    {
        monkeyArm.GetComponent<MonkeyArmController>().Throw();
    }
}
