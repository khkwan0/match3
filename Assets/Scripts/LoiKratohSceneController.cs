using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoiKratohSceneController : MonoBehaviour {

    public float spawnTimeSecs;
    public float repeatSpawnTimeSecs;
    public GameObject skyLanternPrefab;
    public GameObject planePrefab;

    private GameObject skyLantern;
    private GameObject plane;
	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnLantern", 0f, 1f);
    }

    public void SpawnLantern()
    {
        skyLantern = GameObject.Instantiate(skyLanternPrefab);
        Destroy(skyLantern, 90f);
    }
}
