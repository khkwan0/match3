using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {

    // Use this for initialization

    public float lerpTime;
    public Vector3 newPosition = new Vector3();
    public Vector3 outPosition = new Vector3();
    private GameController gc;
    public GameObject confirmPrefab;
    private GameObject cp = null;

	void Start () {
        StartCoroutine(MoveToMiddle());
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.PauseMenuEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MoveToMiddle()
    {
        float startTime = Time.time;
        float currentLerp = 0.0f;
        while (Time.time - startTime <= lerpTime)
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;
            transform.position = Vector3.Lerp(transform.position, newPosition, perc);
            yield return null;
        }
        gc.PauseMenuEnabled = true;
        gc.PauseGame();
    }

    public void Close()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.PauseMenuEnabled = false;
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        float startTime = Time.time;
        float currentLerp = 0.0f;
        while (Time.time - startTime <= lerpTime)
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;
            transform.position = Vector3.Lerp(transform.position, outPosition, perc);
            yield return null;
        }
        gc.PauseMenuEnabled = true;
        gc.DestroyPauseMenu();
    }

    public void Confirm()
    {
        Close();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.PauseMenuEnabled = false;
        cp = GameObject.Instantiate(confirmPrefab);
        StartCoroutine(MoveToMiddleGeneric(cp));

    }

    IEnumerator MoveToMiddleGeneric(GameObject go)
    {
        gc.PlaySwishUp();
        float startTime = Time.time;
        float currentLerp = 0.0f;
        while (Time.time - startTime <= lerpTime)
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;
            go.transform.position = Vector3.Lerp(go.transform.position, newPosition, perc);
            yield return null;
        }
    }
}
