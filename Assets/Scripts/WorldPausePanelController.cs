using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldPausePanelController : MonoBehaviour {

    public GameObject soundOn;
    public GameObject soundOff;
    public GameObject musicOn;
    public GameObject musicOff;

    GameController gc;
	// Use this for initialization
	void Start () {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        gc.PlayMusic(-1);
        SetButtonStatus();
        
        soundOn.GetComponent<Button>().onClick.AddListener(ToggleSound);
        soundOff.GetComponent<Button>().onClick.AddListener(ToggleSound);
        musicOn.GetComponent<Button>().onClick.AddListener(ToggleMusic);
        musicOff.GetComponent<Button>().onClick.AddListener(ToggleMusic);

	}

    private void SetButtonStatus()
    {
        if (gc.SoundFXStatus)
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
        else
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }

        if (gc.MusicStatus)
        {
            musicOn.SetActive(true);
            musicOff.SetActive(false);
        }
        else
        {
            musicOn.SetActive(false);
            musicOff.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ToggleSound()
    {
        gc.ToggleSoundButton();
        SetButtonStatus();
    }

    private void ToggleMusic()
    {
        gc.ToggleMusicButton();
        SetButtonStatus();
    }
}
