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


    [SerializeField]
    private bool soundFXOn;

    public bool SoundFXOn
    {
        get { return soundFXOn; }
        set { soundFXOn = value; }
    }

    public void Start()
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

    public void PlayTileDestroySound()
    {
        if (!tileDestroySound)
        {
            tileDestroySound = GameObject.Instantiate(tileDestroySoundPrefab);
        }
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
            SetSoundButtonOff(true);
        }
        else
        {
            soundFXOn = true;
            SetSoundButtonOff(false);
        }
    }

    private void SetSoundButtonOff(bool off)
    {
        BoardCanvasController bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
        if (off)
        {
            bcc.transform.Find("SoundOn").gameObject.SetActive(false);
            bcc.transform.Find("SoundOff").gameObject.SetActive(true);
        }
        else
        {
            bcc.transform.Find("SoundOn").gameObject.SetActive(true);
            bcc.transform.Find("SoundOff").gameObject.SetActive(false);
        }
    }
}
