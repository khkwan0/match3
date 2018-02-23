using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSController : MonoBehaviour {

    public GameObject sfxPrefab;
    public float speed;
    public float resetDistance;
    private Vector3 originalPos;

    private Vector3 moveVelocity;

    public float startDelay, repeatDelay;
    private float distanceTravelled;
	// Use this for initialization
	void Start () {
        originalPos = transform.position;
        moveVelocity = speed * Vector3.right;
        InvokeRepeating("RunBTS", startDelay, repeatDelay);

	}

    private void RunBTS()
    {
        StartCoroutine(DoBTS());
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PlayBTSSound();
    }

    IEnumerator DoBTS()
    {
        distanceTravelled = 0f;
        while (distanceTravelled < resetDistance)
        {
            transform.position += moveVelocity;
            distanceTravelled += speed;
            yield return null;
        }
        transform.position = originalPos;
    }
    
}
