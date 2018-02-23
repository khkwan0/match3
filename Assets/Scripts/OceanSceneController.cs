using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanSceneController : MonoBehaviour {

    public List<GameObject> fishPrefabs;
    public List<GameObject> crabPrefabs;

    public int maxFish;
    public int maxCrabs;

    private GameObject sun;
    private GameObject sunset;
    private GameObject moon;

    public float cloudSpeed;
    public float sunsetSpeed;
    private Vector3 originalCloudPosition;
    private Vector3 originalSunPosition;
    private Vector3 originalMoonPosition;
    public float sunDisplacementYScale;

    public float sunDownY;
    [SerializeField]
    private float sunDiff;
    private float startSunDiff;
    public float moonSpeed;

    private float startTime;
    private bool firstFrame;
    private Color newColor;

    private bool showMoon;
    private bool moonStarted;


	// Use this for initialization
	void Start () {
        for (int i = 0; i < maxFish; i++)
        {
            SpawnFish(i);
        }
        for (int i = 0; i < maxCrabs; i++)
        {
            SpawnCrab();
        }
        sun = transform.Find("sun").gameObject;
        sunset = transform.Find("sunset").gameObject;
        moon = transform.Find("moon").gameObject;
        startSunDiff = sunDiff = sun.transform.position.y - sunDownY;
        originalSunPosition = sun.transform.position;
        originalMoonPosition = moon.transform.position;
        firstFrame = true;
        showMoon = false;
        moonStarted = false;
	}

    private void Update()
    {
        if (firstFrame)
        {
            startTime = Time.time;
            firstFrame = false;
        }
        sun.transform.position = originalSunPosition + Vector3.up * sunDisplacementYScale * Mathf.Cos((Time.time - startTime) * sunsetSpeed) - Vector3.up;
        sunDiff = sun.transform.position.y - sunDownY;
        newColor = sunset.GetComponent<SpriteRenderer>().color;
        newColor.a = sunDiff / sunDownY;
        sunset.GetComponent<SpriteRenderer>().color = newColor;
        if (sunDiff < 0.3f && !moonStarted)
        {

            showMoon = true;
            moonStarted = true;
        }
        if (showMoon)
        {
            moon.transform.position += Vector3.right * moonSpeed;
        }
        if (moon.transform.position.x > 10f)
        {
            moon.transform.position = originalMoonPosition;
            showMoon = false;
            moonStarted = false;
        }
    }

    private void SpawnFish(int i)
    {
        GameObject fish;

        fish = GameObject.Instantiate(fishPrefabs[Random.Range(0, fishPrefabs.Count)], transform);
        //fish.GetComponent<FishController>().StartFish();            
    }

    private void SpawnCrab()
    {
        GameObject crab;
        crab = GameObject.Instantiate(crabPrefabs[Random.Range(0, crabPrefabs.Count)], transform);
        crab.GetComponent<CrabController>().StartCrab();
    }
}
