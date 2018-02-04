using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    public List<GameObject> tracksPrefabs;
    private GameObject currentTrack;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayTrack(int track)
    {
        currentTrack = GameObject.Instantiate(tracksPrefabs[track]);
        currentTrack.GetComponent<AudioSource>().Play();
    }
}
