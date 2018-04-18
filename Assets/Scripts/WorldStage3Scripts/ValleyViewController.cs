using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValleyViewController : MonoBehaviour {

    public Material skyboxDark;
    public Material skyboxBright;
    public GameObject lighting;

    public ParticleSystem lightningParticle;
    public AudioSource thunderAudioSource;
    public List<AudioClip> thunderSounds = new List<AudioClip>();
    public AudioClip windSound;
    public AudioSource windAudioSource;
    private GameController gc;

    private float timeout;
	void Start () {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        gc.PlayWindSound();
        gc.PlayRainSound();
	}

    private void OnDisable()
    {
        lighting.GetComponent<Light>().color = Color.white;
        RenderSettings.skybox = skyboxBright;
    }

    private void OnEnable()
    {
        RenderSettings.skybox = skyboxDark;
        lighting.GetComponent<Light>().color = Color.black;
    }

    private void Update()
    {
        if (lightningParticle.particleCount > 0)
        {
            if (timeout < 0)
            {
                gc.PlayThunderSound();
                thunderAudioSource.PlayOneShot(thunderSounds[Random.Range(0, thunderSounds.Count)]);
                timeout = 3f;
            }
        }
        timeout -= Time.deltaTime;
    }
}
