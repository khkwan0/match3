using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // Use this for initialization
    private DataController dataController;
    public GameObject boardManager;
    public Camera mainCamera;

    private Camera cam;
    private GameObject board;

    private int startLevel;

    private PlayerDataController playerDataController;

	void Awake () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        cam = Camera.Instantiate(mainCamera);
        playerDataController = transform.GetComponent<PlayerDataController>();
        playerDataController.LoadPlayerData();
        dataController = transform.GetComponent<DataController>();
        dataController.LoadGameData();
        StartBoard();
        if (playerDataController.playerData.lastLevel < 0)
        {
            startLevel = 0;
        } else
        {
            startLevel = playerDataController.GetComponent<PlayerData>().lastLevel + 1;
        }            
    }

    public PlayerData GetPlayerData()
    {
        return playerDataController.playerData;
    }

    public void LevelWin(int level, int score, int timestamp, int stars)
    {
        playerDataController.PlayerSaveWin(level, score, timestamp, stars);
    }

    public void StartBoard()
    {
        board = GameObject.Instantiate(boardManager);
        board.GetComponent<Board>().SetGameData(dataController.gameData);
        board.GetComponent<Board>().SetGameController(this);
        board.GetComponent<Board>().SetBoardSize(new Vector2(cam.aspect * 2f * cam.orthographicSize, 2f * cam.orthographicSize));
        StartLevel(startLevel);
    }

    private void Start()
    {
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
