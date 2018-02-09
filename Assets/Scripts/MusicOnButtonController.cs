using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicOnButtonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(TurnOffMusic);
    }

    private void TurnOffMusic()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").gameObject.GetComponent<GameController>();
        gc.ToggleMusicButton();
    }
}
