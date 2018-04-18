using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishermanSceneController : MonoBehaviour {

    public List<GameObject> fish;
    [SerializeField]
    private int fishCount;
    public int maxFish;


	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnFish", 0f, 1f);
	}

    private void SpawnFish()
    {
        if (fishCount <= maxFish)
        {
            GameObject _fish = GameObject.Instantiate(fish[Random.Range(0, fish.Count)], new Vector3(-2f, Random.Range(-1.4f, 1.4f), 5f), Quaternion.Euler(0f, -90f, 0f), transform);
            _fish.GetComponent<FishController>().CanRise = true;
            _fish.GetComponent<FishController>().riseAmount = 0.2f;
            if (_fish)
            {
                fishCount++;
            }
        }
    }
}
