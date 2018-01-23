using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanelController : MonoBehaviour {

    public float lerpTime;
    public Vector3 outPosition = new Vector3();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConfirmNegative()
    {
        GameObject cp = GameObject.FindGameObjectWithTag("ConfirmPanel");
        if (cp)
        {
            StartCoroutine(DisappearGeneric(cp));
        }
    }

    IEnumerator DisappearGeneric(GameObject go)
    {
        float startTime = Time.time;
        float currentLerp = 0.0f;
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.PlaySwishDown();
        while (Time.time - startTime <= lerpTime)
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;
            go.transform.position = Vector3.Lerp(go.transform.position, outPosition, perc);
            yield return null;
        }

        gc.PauseMenuEnabled = true;
        Destroy(go);
    }
}
