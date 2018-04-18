using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeViewController : MonoBehaviour {

    public GameObject tuktuk;
    public List<GameObject> spawnPostion;
    public List<BansheeGz.BGSpline.Curve.BGCurve> pathCurve;

	void Start () {
        InvokeRepeating("SpawnTukTuk", 0f, 2f);
	}

	void Update () {
		
	}

    void SpawnTukTuk()
    {
        int pose = Random.Range(0, spawnPostion.Count);        
        tuktuk.GetComponent<TukTukController>().speed = Random.Range(0.025f, 0.25f);
        tuktuk.GetComponent<TukTukController>().pathCurve = pathCurve[pose];
        GameObject.Instantiate(tuktuk, spawnPostion[pose].transform.position, Quaternion.identity);
    }
}
