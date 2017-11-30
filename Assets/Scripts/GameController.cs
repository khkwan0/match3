using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    // Use this for initialization
    private DataController dataController;
    public GameObject boardManager;
    public Camera mainCamera;

    private Camera cam;
    private GameObject board;

    private PlayerDataController playerDataController;

	void Awake () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        cam = Camera.Instantiate(mainCamera);
        playerDataController = transform.GetComponent<PlayerDataController>();
        playerDataController.LoadPlayerData();
        dataController = transform.GetComponent<DataController>();
        dataController.LoadGameData();
        board = GameObject.Instantiate(boardManager);
        board.GetComponent<Board>().SetGameData(dataController.gameData);
        board.GetComponent<Board>().SetBoardSize(new Vector2(cam.aspect * 2f * cam.orthographicSize, 2f * cam.orthographicSize));
    }

    private void Start()
    {
        StartLevel(0);
    }

    public Camera GetCam()
    {
        return cam;
    }

    private void StartLevel(int level)
    {
        board.GetComponent<Board>().StartLevel(level);
    }

}
