using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public List<string> tracks;
    [SerializeField]
    private bool musicOn;

    public bool MusicOn
    {
        get { return musicOn; }
        set { musicOn = value; }
    }

    void Awake () {
        musicOn = true;
		
	}

    public void PlayTrack(int track)
    {
        AudioClip clip = (AudioClip)Resources.Load("Music/" + tracks[track]);
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        if (!musicOn)
        {
            GetComponent<AudioSource>().Pause();
        }
    }

    public void PlayRandomTrack()
    {
        AudioClip clip = (AudioClip)Resources.Load("Music/" + tracks[Random.Range(0, tracks.Count)]);
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        if (!musicOn)
        {
            GetComponent<AudioSource>().Pause();
        }
    }

    public void PlayCreep()
    {
        AudioClip creep = (AudioClip)Resources.Load("Music/creep");
        GetComponent<AudioSource>().clip = creep;
        GetComponent<AudioSource>().Play();
        if (!musicOn)
        {
            GetComponent<AudioSource>().Pause();
        }
    }

    public void PlaySpring()
    {
        AudioClip clip = (AudioClip)Resources.Load("Music/Spring_In_My_Step");
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        if (!musicOn)
        {
            GetComponent<AudioSource>().Pause();
        }
    }

    public void ToggleMusicButton()
    {
        if (musicOn)
        {
            musicOn = false;
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().Pause();
            }
        }
        else
        {
            musicOn = true;
            GetComponent<AudioSource>().UnPause();
        }
        SetMusicButtonState(musicOn);
    }

    public void SetMusicButtonState(bool on)
    {
        GameObject bc = GameObject.FindGameObjectWithTag("BoardCanvas");
        if (bc)
        {
            BoardCanvasController bcc = bc.GetComponent<BoardCanvasController>();
            if (bcc)
            {
                if (on)
                {
                    bcc.transform.Find("MusicOn").gameObject.SetActive(true);
                    bcc.transform.Find("MusicOff").gameObject.SetActive(false);
                }
                else
                {
                    bcc.transform.Find("MusicOn").gameObject.SetActive(false);
                    bcc.transform.Find("MusicOff").gameObject.SetActive(true);
                }
            }
        }
    }
}
