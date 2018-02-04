using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBoardController : MonoBehaviour {

    public float lerpTime;
    public Vector3 newPosition = new Vector3(0f, 0f, -1f);
    public Vector3 outPosition = new Vector3(0f, 12f, -1f);

    // Use this for initialization
    void Start () {
        StartCoroutine(SlideUp());
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().PauseMenuEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator SlideUp()
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
            yield return 1;
        }
    }

    public void Disappear()
    {
        StartCoroutine(SlideOut());
    }

    IEnumerator SlideOut()
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
            yield return 1;
        }
        Destroy(gameObject);
    }
}
