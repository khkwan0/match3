using System.Collections;
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
    private SoundController soundController;

    private Camera cam;
    private GameObject board;

    private int startLevel;
    private int currentLevel;

    private PlayerDataController playerDataController;

    public enum _state { intro, world, board };
    private _state gameState;

    public GameObject endBoardPanelPrefab;
    private GameObject ebp;
    BoardCanvasController bcc;
    private Vector2 boardSize;
    private GameObject lanternPrefab;

    public GameObject fireworkPrefab;

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
        soundController = GetComponent<SoundController>();
       
    }

    public void Start()
    {
        ShowStartButton();
        lanternPrefab = Resources.Load("Sprites/sky_lantern") as GameObject;
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }

    private void ShowStartButton()
    {
        StartCoroutine(PauseForSplash());
    }

    IEnumerator PauseForSplash()
    {
        yield return new WaitForSeconds(5f);
        GameObject.FindGameObjectWithTag("SplashCanvas").GetComponent<Canvas>().enabled = true;
    }

    private void LevelPostStart()
    {
        bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
    }

    public PlayerData GetPlayerData()
    {
        return playerDataController.playerData;
    }

    public void LevelWin(int level, int score, int timestamp, int stars)
    {
        playerDataController.PlayerSaveWin(level, score, timestamp, stars);
        //BackToWorld();
    }

    public void StartBoard()
    {
        board = GameObject.Instantiate(boardManager);
        board.GetComponent<Board>().SetGameData(dataController.gameData);
        board.GetComponent<Board>().SetGameController(this);
        boardSize = new Vector2(cam.aspect * 2f * cam.orthographicSize, 2f * cam.orthographicSize);
        board.GetComponent<Board>().SetBoardSize(boardSize);
        StartLevel(currentLevel);
    }

    public Camera GetCam()
    {
        return cam;
    }

    private void StartLevel(int level)
    {
        playerDataController.StartLevel(level);
        board.GetComponent<Board>().StartLevel(level);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
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
        cam.transform.position = new Vector3(0.0f, 0.0f, cam.transform.position.z);
        cam.orthographicSize = 6.0f;
    }

    public void OnLevelWasLoaded(int scene)
    {
        if (scene == 2)
        {
            gameState = _state.board;
            StartBoard();
            LevelPostStart();
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
        CancelInvoke();
        StopAllCoroutines();
        SceneManager.LoadScene(1);
        gameState = _state.world;
    }

    public void SetLevelScore(int level, int score)
    {
        playerDataController.SetLevelScore(level, score);
        bcc.SetScore(score);
    }

    public void SetProgress(float amt)
    {
        bcc.SetFillAmount(amt);
    }

    public void AddTileCount(int level, TilePiece._TileType tiletype, int value)
    {
        bcc.Deduct(tiletype, value);
        playerDataController.AddTileCount(level, tiletype, value);
    }

    public int GetTileCount(int level, TilePiece._TileType tileType, int value)
    {
        return playerDataController.GetTileCount(level, tileType, value);
    }
    public void AddLevelStart(int level)
    {
        playerDataController.AddLevelStar(level);
    }

    public void AddOverallScore(int score)
    {
        playerDataController.AddOverallScore(score);
    }

    public void ShowWin(int numScore, int stars)
    {
        PlayWinSound();
        bcc.ShowWin(numScore, stars);
    }

    public void ShowLose()
    {
        bcc.ShowLose();
    }

    public void WinButtonGoBackToWorld()
    {
        BackToWorld();
    }

    public void PlayWinSound()
    {
        soundController.PlayWinSound();
    }

    public void PlayTileDestroySound()
    {
        soundController.PlayTileDestroySound();
    }

    public void PlayGreatSound()
    {
        soundController.PlayGreat();
    }

    public void PlayWooHooSound()
    {
        soundController.PlayWooHoo();
    }
    
    public void PlayLoseSound()
    {
        soundController.PlayLose();
    }


    public void ToggleSoundButton()
    {
        soundController.ToggleSoundButton();
    }

    public void UpdateSoundButton()
    {
        soundController.SetSoundButtonOff(soundController.SoundFXOn);
    }

    public void ShowEndBoard(int score, int stars)
    {
        Debug.Log("Showendboard");
        ebp = GameObject.Instantiate(endBoardPanelPrefab);       
    }

    public void ShowSkyLanterns()
    {
        InvokeRepeating("SpawnLantern", 0.5f, 1f);
    }

    private void SpawnLantern()
    {
        float sizeX = lanternPrefab.transform.Find("sky_lantern").GetComponent<Renderer>().bounds.size.x;
        float sizeY = lanternPrefab.transform.Find("sky_lantern").GetComponent<Renderer>().bounds.size.y;
        Vector3 newPos = new Vector3(Random.Range(-boardSize.x / 2f + sizeX / 2f, boardSize.x / 2f - sizeX),
            -boardSize.y / 2f - sizeY,
            -2f);
        GameObject lantern = GameObject.Instantiate(lanternPrefab, newPos, Quaternion.identity);
        lantern.GetComponent<Rigidbody>().velocity = new Vector2(0f, Random.Range(.5f, 2f));
        StartCoroutine(LanternSway(lantern));
    }

    IEnumerator LanternSway(GameObject go)
    {
        Vector3 pos = new Vector3();
        float sizeX = lanternPrefab.transform.Find("sky_lantern").GetComponent<Renderer>().bounds.size.x;
        float sizeY = lanternPrefab.transform.Find("sky_lantern").GetComponent<Renderer>().bounds.size.y;
        bool inView = true;
        float topBounds = boardSize.y / 2f + sizeY / 2f;
        float amplitude = Random.Range(0.05f, 0.2f);
        while(inView)
        {
            pos = go.transform.position;
            if (pos.y > topBounds)
            {
                inView = false;
            }
            else
            {
                pos.x += amplitude * Mathf.Sin(Time.fixedTime) / boardSize.x;
                go.transform.position = pos;
            }
            yield return null;
        }
        Destroy(go);
    }

    public void ShowFireworks()
    {
        SpawnFirework();
    }

    private void SpawnFirework()
    {
        Vector3 newPos = new Vector3(Random.Range(-boardSize.x / 2f, boardSize.x / 2f),
            -boardSize.y * 1.5f,
            -2f);
        GameObject firework = GameObject.Instantiate(fireworkPrefab);
        firework.transform.position = newPos;
    }


}
