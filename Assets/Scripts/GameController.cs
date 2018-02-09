using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    // Use this for initialization
    public DataController dataController;
    public GameObject boardManager;
    public GameObject worldControllerPrefab;
    private GameObject worldController;
    public Camera mainCamera;
    private SoundController soundController;
    private MusicController musicController;
    public GameObject pauseButtonPrefab;
    private GameObject pauseButton;
    public GameObject pauseMenuPrefab;
    private GameObject pauseMenu;
    public GameObject boardCanvasPrefab;
    private GameObject boardCanvas;
    public GameObject loseCanvasPrefab;
    private GameObject loseCanvas;
    public GameObject ripple;
    public GameObject helperPanelPrefab;
    private GameObject helperPanel;
    [SerializeField]
    private GameObject currentHelper;
    private BoardCanvasController bcc;
       

    private Camera cam;
    private GameObject board;

    private int startLevel;
    private int currentLevel;

    private PlayerDataController playerDataController;

    public enum _state { intro, world, board };
    private _state gameState;

    public GameObject endBoardPanelPrefab;
    public GameObject startBoardPanelPrefab;
    private GameObject ebp;
    private Vector2 boardSize;
    private GameObject lanternPrefab;

    public GameObject fireworkPrefab;

    [SerializeField]
    private bool pauseMenuEnabled;
    [SerializeField]
    private bool paused;

    public enum _helperType { None, Hammer, Vertical, Horizontal, Rainbow, Bomb };
    public static GameController GetGameController()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public GameObject CurrentHelper
    {
        get { return currentHelper; }
    }

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
        musicController = GetComponent<MusicController>();
        pauseMenuEnabled = true;
        paused = false;       
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishLoading;
    }

    public void Start()
    {
        ShowStartButton();
        lanternPrefab = Resources.Load("Sprites/sky_lantern") as GameObject;
    }

    public bool PauseMenuEnabled
    {
        get { return pauseMenuEnabled; }
        set { pauseMenuEnabled = value; }
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
        ripple.SetActive(true);
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
        MusicController mc = GetComponent<MusicController>();
        mc.PlayTrack(0);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    public void ShowPauseButton()
    {
        if (!pauseButton)
        {
            pauseButton = GameObject.Instantiate(pauseButtonPrefab);
        }
        pauseButton.SetActive(true);
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
        if (!cam)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        cam.transform.position = new Vector3(0.0f, 0.0f, cam.transform.position.z);
        cam.orthographicSize = 6.0f;
    }

    private void OnLevelFinishLoading(Scene scene, LoadSceneMode mode) 
    {
        if (scene.name == "Board")
        {
            gameState = _state.board;
            StartBoard();
            LevelPostStart();
        }
        if (scene.name == "World")
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
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (gc)
        {
            pauseMenuEnabled = true;
            gc.CancelInvoke();
            gc.StopAllCoroutines();
            SceneManager.LoadScene(1);
            gc.gameState = _state.world;
        }
    }

    public void SetLevelScore(int level, int score)
    {
        playerDataController.SetLevelScore(level, score);
        bcc.SetScore(score);
    }

    public void SetProgress(int tier1, int tier2, int tier3, int maxFill, int numScore)
    {
        bcc.SetFillAmount(tier1, tier2, tier3, maxFill, numScore);
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
        loseCanvas = GameObject.Instantiate(loseCanvasPrefab);
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

    public void PlaySwishUp()
    {
        soundController.PlaySwishUp();
    }

    public void PlaySwishDown()
    {
        soundController.PlaySwishDown();
    }
    public void ToggleSoundButton()
    {
        soundController.ToggleSoundButton();
    }

    public void UpdateSoundButton()
    {
        soundController.SetSoundButtonOff(soundController.SoundFXOn);
    }

    public void UpdateMusicButton()
    {
        musicController.SetMusicButtonState(musicController.MusicOn);
    }

    public void ToggleMusicButton()
    {
        musicController.ToggleMusicButton();
    }

    public void ShowStartBoard()
    {
        boardCanvas = GameObject.Instantiate(boardCanvasPrefab);
        if (boardCanvas)
        {
            bcc = boardCanvas.GetComponent<BoardCanvasController>();
        }
        soundController.PlaySwishUp();
        GameObject sbp = GameObject.Instantiate(startBoardPanelPrefab);
        StartBoardPanelController sbpc = GameObject.FindGameObjectWithTag("StartPanel").GetComponent<StartBoardPanelController>();
        sbpc.SetText("");
        if (dataController.gameData.levelData[currentLevel].mission.type == 0)
        {
            sbpc.AppendText("\nGet " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].score + " in " + dataController.gameData.levelData[currentLevel].numMoves + " moves.\nGood Luck!");
        }
        if (dataController.gameData.levelData[currentLevel].mission.type == 1)
        {
            sbpc.AppendText("\nMatch the following pieces: \n");
            sbpc.ShowMissionGoals(dataController.gameData.levelData[currentLevel].mission.missionGoals, board);
        }
        if (dataController.gameData.levelData[currentLevel].mission.type == 2)
        {
            sbpc.AppendText("\nBring " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].numfall + " to the bottom");
            if (!bcc)
            {
                bcc = GameObject.FindGameObjectWithTag("BoardCanvasController").GetComponent<BoardCanvasController>();
            }
            bcc.ShowDropCountPanel();
            bcc.SetDropCountText("0 / " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].numfall);

        }
        board.GetComponent<Board>().Locked = true;
    }

    public void DisappearStartBoard()
    {
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (gc)
        {
            gc.PlaySwishDown();
            GameObject.FindGameObjectWithTag("StartBoardPanel").GetComponent<StartBoardController>().Disappear();
            gc.board.GetComponent<Board>().Locked = false;
            gc.ShowPauseButton();
            bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
            helperPanel = GameObject.Instantiate(helperPanelPrefab);
            helperPanel.GetComponent<HelperCanvasController>().CreateHelpers(gc.dataController.gameData.levelData[currentLevel], helperPanel.transform);
        }
    }

    public void ShowEndBoard(int score, int stars)
    {
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
        float topBounds = boardSize.y / 2f + sizeY;
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

    public void ShowPauseMenu()
    {
        if (pauseMenuEnabled)
        {
            if (pauseMenu)
            {
                ClosePauseMenu();
            }
            else
            {
                pauseMenu = GameObject.Instantiate(pauseMenuPrefab, new Vector3(-10.0f, 0f, -2f), Quaternion.identity);
                soundController.PlaySwishUp();               
            }
        }
    }

    public void ClosePauseMenu()
    {
        if (pauseMenu)
        {
            soundController.PlaySwishDown();
            pauseMenu.GetComponent<PauseMenuController>().Close(true);
            paused = false;
        }
    }

    public void DestroyPauseMenu()
    {
        if (pauseMenu)
        {
            Destroy(pauseMenu);
        }
    }
    
    public void PauseGame()
    {
        paused = true;
    }

    public void ResumeGame()
    {
        paused = false;
    }

    public void LockBoard()
    {
        board.GetComponent<Board>().Locked = true;
    }

    public void UnLockBoard()
    {
        board.GetComponent<Board>().Locked = false;
    }

    public void PlaceStars(int tier1Fill, int tier2Fill, int tier3Fill, int maxFillScore)
    {
        bcc.PlaceStars(tier1Fill, tier2Fill, tier3Fill, maxFillScore);
    }

    public void UpdateDropCount(int amt)
    {
        bcc.SetDropCountText(amt + " / " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].numfall);
    }

    public void SpawnMissionGoal(int toreach, TilePiece._TileType tileType, int tileValue, int idx, Sprite theSprite)
    {
        if (!bcc)
        {
            bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
        }
        bcc.ShowTileCountPanel();
        bcc.SpawnMissionGoal(toreach, tileType, tileValue, idx, theSprite);
    }

    public LevelData GetLevelData()
    {
        return dataController.gameData.levelData[currentLevel];
    }

    public void SetHelper(GameObject helper)
    {
        if (helper)
        {
            currentHelper = helper;
            _helperType helperType = helper.GetComponent<HelperController>().HelperType;
            board.GetComponent<Board>().HelperType = helperType;
            Darken();
            helperPanel = GameObject.FindGameObjectWithTag("HelperPanel");
            if (helperPanel)
            {
                helperPanel.GetComponent<HelperCanvasController>().MaskOtherHelpers(helper);
            }
        }
        else
        {
            currentHelper = null;
            board.GetComponent<Board>().HelperType = _helperType.None;
            Undarken();
            helperPanel = GameObject.FindGameObjectWithTag("HelperPanel");
            if (helperPanel)
            {
                helperPanel.GetComponent<HelperCanvasController>().ShowAllHelpers();
            }
        }
    }

    public void DeductHelper()
    {
        if (currentHelper)
        {
            currentHelper.GetComponent<HelperController>().Decrement();
        }
        currentHelper = null;
    }

    public void Darken()
    {
        board.GetComponent<Board>().Darken();
    }

    public void Undarken()
    {
        board.GetComponent<Board>().Undarken();
    }
}
