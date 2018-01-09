using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToWorldButtonController : MonoBehaviour {

    // Use this for initialization

    public void OnMouseDown()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.BackToWorld();
    }
}
