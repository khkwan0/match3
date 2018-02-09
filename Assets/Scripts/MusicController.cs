using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public List<GameObject> tracksPrefabs;
    private GameObject currentTrack;

    [SerializeField]
    private bool musicOn;

    public bool MusicOn
    {
        get { return musicOn; }
        set { musicOn = value; }
    }

    void Start () {
        musicOn = true;
		
	}

    public void PlayTrack(int track)
    {
        currentTrack = GameObject.Instantiate(tracksPrefabs[track]);        
        currentTrack.GetComponent<AudioSource>().Play();
        if (!musicOn)
        {
            currentTrack.GetComponent<AudioSource>().Pause();
        }
    }

    public void ToggleMusicButton()
    {
        if (musicOn)
        {
            musicOn = false;
            if (currentTrack)
            {
                currentTrack.GetComponent<AudioSource>().Pause();
            }
        }
        else
        {
            musicOn = true;
            currentTrack.GetComponent<AudioSource>().UnPause();
        }
        SetMusicButtonState(musicOn);
    }

    public void SetMusicButtonState(bool on)
    {
        BoardCanvasController bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
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
