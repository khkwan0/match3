using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanelController : MonoBehaviour {

    public float lerpTime;
    public Vector3 outPosition = new Vector3();
    public Vector3 newPosition = new Vector3();
    private GameController gc;
    private void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StartCoroutine(MoveToMiddle());
    }

    public void ConfirmNegative()
    {
        GameObject cp = GameObject.FindGameObjectWithTag("ConfirmPanel");
        if (cp)
        {
            StartCoroutine(DisappearGeneric(cp));
        }
    }

    IEnumerator MoveToMiddle()
    {
        gc.PlaySwishUp();
        gc.PauseMenuEnabled = false;
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
        gc.PauseMenuEnabled = false;
    }

    IEnumerator DisappearGeneric(GameObject go)
    {
        float startTime = Time.time;
        float currentLerp = 0.0f;
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
        gc.ResumeGame();
        Destroy(go);
    }
}
