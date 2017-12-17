﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // Use this for initialization
    private DataController dataController;
    public GameObject boardManager;
    public GameObject worldControllerPrefab;
    private GameObject worldController;
    public Camera mainCamera;

    private Camera cam;
    private GameObject board;

    private int startLevel;
    private int currentLevel;

    private PlayerDataController playerDataController;

    public enum _state { intro, world, board };
    private _state gameState;
    
	void Awake () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        cam = Camera.Instantiate(mainCamera);
        playerDataController = transform.GetComponent<PlayerDataController>();
        playerDataController.LoadPlayerData();
        dataController = transform.GetComponent<DataController>();
        dataController.LoadGameData();         
        gameState = _state.intro;
        Object.DontDestroyOnLoad(transform);
        Object.DontDestroyOnLoad(cam);
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
        StartLevel(currentLevel);
    }

    public Camera GetCam()
    {
        return cam;
    }

    private void StartLevel(int level)
    {
        board.GetComponent<Board>().StartLevel(level);
    }

    public void IntroOnClick()
    {
        SceneManager.LoadScene(1);
        gameState = _state.world;
    }

    public int NumLevels
    {
        get { return dataController.NumLevels; }
    }

    public int LastLevel
    {
        get { return playerDataController.playerData.lastLevel; }
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        SceneManager.LoadScene(2);
    }

    public void OnLevelWasLoaded(int scene)
    {
        if (scene == 2)
        {
            Debug.Log("sceneload");
            gameState = _state.board;
            StartBoard();
        }
        if (scene == 1)
        {
            if (board)
            {
                Destroy(board);
            }
            if (worldController == null)
            {
                worldController = GameObject.Instantiate(worldControllerPrefab);
            }
            worldController.GetComponent<WorldController>().DoRender();
        }
    }

    public void BackToWorld()
    {
        SceneManager.LoadScene(1);
        gameState = _state.world;
    }
}