using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvasController : MonoBehaviour {

    public Button pauseButton;
    private bool showPause;
    public GameObject pausePanel;
    public GameObject levelPanel;
    Vector3 originalPos;
    Vector3 originalLevelPanelPos;
    public float lerpTime;

	// Use this for initialization
	void Start () {
        originalPos = pausePanel.GetComponent<RectTransform>().position;
        originalLevelPanelPos = levelPanel.transform.position;
        showPause = false;
        pauseButton.GetComponent<Button>().onClick.AddListener(TogglePauseMenu);
	}

    private void TogglePauseMenu()
    {
        showPause = !showPause;
        ShowPause();

    }

    private void ShowPause()
    {
        if (showPause)
        {
            pausePanel.GetComponent<RectTransform>().position = new Vector3(0f, 150f, 0f);
        } else
        {
            pausePanel.GetComponent<RectTransform>().position = originalPos;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowLevel(int level, GameObject levelMarker)
    {
        levelPanel.GetComponent<LevelPanelController>().GetLevelData(level);
        levelPanel.GetComponent<LevelPanelController>().LevelMarker = levelMarker;
        Vector3 destination = new Vector3(Screen.width/2f, Screen.height/2f, 0f);
        if (destination != levelPanel.transform.position)
        {
            StartCoroutine(Slide(levelPanel, originalLevelPanelPos, destination, lerpTime));
        }
    }

    public void HideLevel()
    {
        StartCoroutine(Slide(levelPanel, levelPanel.transform.position, originalLevelPanelPos, lerpTime));
    }

    IEnumerator Slide(GameObject obj, Vector3 start, Vector3 finish, float lerpTime)
    {
        float startTime = Time.time;
        float perc;
        while ((Time.time - startTime) < lerpTime)
        {
            perc = (Time.time - startTime) / lerpTime;
            obj.transform.position = Vector3.Lerp(start, finish, perc);
            yield return null;
        }
        obj.GetComponent<RectTransform>().position = finish;
    }
}
