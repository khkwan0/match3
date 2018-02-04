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
        gc.PauseGame();
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

    }

    public void Close(bool enablePauseMenuButton)
    {
        if (gc)
        {
            gc.PauseMenuEnabled = false;
            StartCoroutine(Disappear(enablePauseMenuButton));
        }
    }

    IEnumerator Disappear(bool toEnablePauseMenuButton)
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
        gc.PauseMenuEnabled = toEnablePauseMenuButton;
        gc.DestroyPauseMenu();
    }

    public void Confirm()
    {
        if (gc)
        {
            GameObject.Instantiate(confirmPrefab);
        }
        this.Close(false);
    }


}
