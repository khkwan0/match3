using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public GameObject winSoundPrefab;
    private GameObject winSound = null;
    public GameObject tileDestroySoundPrefab;
    private GameObject tileDestroySound = null;
    public GameObject greatSoundPrefab;
    private GameObject greatSound;
    public GameObject loseSoundPrefab;
    private GameObject loseSound;
    public GameObject wooHooSoundPrefab;
    private GameObject wooHooSound;
    public GameObject swishUpPrefab;
    public GameObject swishDownPrefab;
    private GameObject swishUp;
    private GameObject swishDown;
    public GameObject kickSoundPrefab;
    private GameObject kickSound;
    public GameObject btsSoundPrefab;
    private GameObject btsSound;
    public GameObject starSoundPrefab;
    private GameObject starSound;
    public GameObject tadaSoundPrefab;
    private GameObject tadaSound;
    public GameObject boingSoundPrefab;
    private GameObject boingSound;

    [SerializeField]
    private bool soundFXOn;

    public bool SoundFXOn
    {
        get { return soundFXOn; }
        set { soundFXOn = value; }
    }

    public void Awake()
    {
        soundFXOn = true;
    }

    public void PlayWinSound()        
    {
        if (!winSound)
        {
            winSound = GameObject.Instantiate(winSoundPrefab);
        }
        DoPlay(winSound.GetComponent<AudioSource>());
    }

    public void PlayTileDestroySound(int cascadeCount)
    {
        if (!tileDestroySound)
        {
            tileDestroySound = GameObject.Instantiate(tileDestroySoundPrefab);
        }
        Debug.Log(tileDestroySound.GetComponent<AudioSource>().pitch);
        tileDestroySound.GetComponent<AudioSource>().pitch = 0.8f + (cascadeCount * 0.1f);
        DoPlay(tileDestroySound.GetComponent<AudioSource>());
    }

    public void PlayGreat()
    {
        if (!greatSound)
        {
            greatSound = GameObject.Instantiate(greatSoundPrefab);
        }
        DoPlay(greatSound.GetComponent<AudioSource>());
    }

    public void PlayKickSound()
    {
        if (!kickSound)
        {
            kickSound = GameObject.Instantiate(kickSoundPrefab);
        }
        DoPlay(kickSound.GetComponent<AudioSource>());
    }

    public void PlayBoing()
    {
        if (!boingSound)
        {
            boingSound = GameObject.Instantiate(boingSoundPrefab);
        }
        DoPlay(boingSound.GetComponent<AudioSource>());
    }
    public void PlayBTSSound()
    {
        if (!btsSound) 
        {
            btsSound = GameController.Instantiate(btsSoundPrefab);
        }
        DoPlay(btsSound.GetComponent<AudioSource>());
    }

    public void PlaySwishUp()
    {
        if (!swishUp)
        {
            swishUp = GameObject.Instantiate(swishUpPrefab);
        }
        DoPlay(swishUp.GetComponent<AudioSource>());
    }

    public void PlaySwishDown()
    {
        if (!swishDown)
        {
            swishDown = GameObject.Instantiate(swishDownPrefab);
        }
        DoPlay(swishDown.GetComponent<AudioSource>());
    }

    public void PlayStarSound()
    {
        if (!starSound)
        {
            starSound = GameObject.Instantiate(starSoundPrefab);
        }
        DoPlay(starSound.GetComponent<AudioSource>());
    }

    public void PlayWooHoo()
    {
        if (!wooHooSound)
        {
            wooHooSound = GameObject.Instantiate(wooHooSoundPrefab);
        }
        DoPlay(wooHooSound.GetComponent<AudioSource>());
    }

    public void PlayLose()
    {
        if (!loseSound)
        {
            loseSound = GameObject.Instantiate(loseSoundPrefab);
        }
        DoPlay(loseSound.GetComponent<AudioSource>());
    }

    public void DoPlay(AudioSource source)
    {
        if (soundFXOn)
        {
            source.Play();
        }
    }

    public void ToggleSoundButton()
    {
        if (soundFXOn)
        {
            soundFXOn = false;
            SetSoundButtonOff(soundFXOn);
        }
        else
        {
            soundFXOn = true;
            SetSoundButtonOff(soundFXOn);
        }
    }

    public void SetSoundButtonOff(bool on)
    {
        BoardCanvasController bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
        if (on)
        {
            bcc.transform.Find("SoundOn").gameObject.SetActive(true);
            bcc.transform.Find("SoundOff").gameObject.SetActive(false);
        }
        else
        {
            bcc.transform.Find("SoundOn").gameObject.SetActive(false);
            bcc.transform.Find("SoundOff").gameObject.SetActive(true);
        }
    }
}
