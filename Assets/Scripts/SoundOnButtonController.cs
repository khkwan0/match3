using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOnButtonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(TurnOffSound);
	}

    private void TurnOffSound()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").gameObject.GetComponent<GameController>();
        gc.ToggleSoundButton();
    }
}
