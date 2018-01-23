using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelExitButtonController : MonoBehaviour {

    public void OnMouseDown()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.BackToWorld();
    }
}
