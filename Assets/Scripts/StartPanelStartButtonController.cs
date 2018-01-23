using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelStartButtonController : MonoBehaviour {

    public void OnMouseUp()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().DisappearStartBoard();
        
    }
}
