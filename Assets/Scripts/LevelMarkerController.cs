using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMarkerController : MonoBehaviour {

    public float bounceSpeed = 2f;
    public int level = 0;
    public enum _status { on, off, bouncing};
    public _status status;
    public float cameraZoomSpeed = 3f;

    public GameObject LatestLevel;
    public GameObject UnreachedLevel;
    public GameObject Star;
    public GameObject FinishedLevel;

    private int latestLevel;
    WorldCanvasController wcc;
    WorldSceneController wsc;
    // Use this for initialization

    private void Awake()
    {

    }

    void Start () {
        wsc = GameObject.Find("WorldScene").GetComponent<WorldSceneController>();
        wcc = GameObject.Find("WorldControllerCanvas").GetComponent<WorldCanvasController>();
        wsc.allowMove = true;
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        latestLevel = gc.PlayerLastLevel;
        if ((latestLevel + 1) < level)
        {
            status = _status.off;
        }
        if (latestLevel > level)
        {
            status = _status.on;
        }
        if ((latestLevel + 1) == level)
        {
            status = _status.bouncing;
        }
        if (latestLevel == level)
        {
            GameObject boat = GameObject.Find("boat");
            boat.transform.position = transform.position;

        }
        switch (status)
        {
            case _status.bouncing: GameObject.Instantiate(LatestLevel, transform); break;
            case _status.on: GameObject.Instantiate(FinishedLevel, transform); break;
            default: GameObject.Instantiate(UnreachedLevel, transform); break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (status == _status.bouncing)
        {
            transform.position += transform.up * Mathf.Sin(Time.time * bounceSpeed) * 0.01f;
        }
	}

    public void OnMouseDown()
    {
        if (status != _status.off)
        {
            GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
            gc.LevelClicked = level;

            if (level == (latestLevel + 1))
            {
                if (level == 20)  //special case for scene specific event
                {
                    wsc.SpawnThaiWoman();
                }
                else
                {
                    if (wsc && level < 20)
                    {

                        wsc.RotateTowardsTarget(gameObject);
                    }
                }
            }
            wcc.ShowLevel(level, gameObject);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        int levelClicked = gc.LevelClicked;
        if (other.name == "boat" && levelClicked == level && status != _status.off)
        {
            if (wsc)
            {
                wsc.allowMove = false;
            }
            //StartCoroutine(StartLevel(other));
        }


    }

    public void StartLevel()
    {
        StartCoroutine(DoStartLevel());
    }

    IEnumerator DoStartLevel()
    {
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        float perc = 0f;
        float startTime;
        Vector3 originalPos = cam.transform.position;       

        startTime = Time.time;
        GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
        if (gc)
        {
            gc.PlayChooseLevel();
        }
        while ((Time.time - startTime) < cameraZoomSpeed)
        {
            perc = (Time.time - startTime) / cameraZoomSpeed;
            Debug.Log(cam.transform.position);
            cam.transform.position = Vector3.Lerp(originalPos, transform.position, perc);
            yield return null;
        }

        if (gc)
        {
            gc.LoadLevel(level);
        }
    }

}
