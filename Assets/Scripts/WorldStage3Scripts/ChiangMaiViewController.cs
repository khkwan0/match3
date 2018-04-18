using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiangMaiViewController : MonoBehaviour {

    public GameObject stStartPosition;
    public GameObject stStartPosition2;
    public GameObject st;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnTukTuk", 2f, 3f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnTukTuk()
    {
        GameObject.Instantiate(st, stStartPosition.transform.position, Quaternion.identity, transform);
        GameObject.Instantiate(st, stStartPosition2.transform.position, Quaternion.Euler(0f, 90f, 0f), transform);
    }
}
