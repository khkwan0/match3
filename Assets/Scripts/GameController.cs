using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IPHONE
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class GameController : MonoBehaviour {

    // Use this for initialization
    public DataController dataController;
    public GameObject boardManager;
    public GameObject worldControllerPrefab;
    private GameObject worldController;
    public Camera mainCamera;
    private SoundController soundController;
    private MusicController musicController;
    //public GameObject startBoardPanelPrefab;
    //public GameObject pauseButtonPrefab;
    //private GameObject pauseButton;
    //public GameObject pauseMenuPrefab;
    //private GameObject pauseMenu;
    public GameObject boardCanvasPrefab;
    private GameObject boardCanvas;
    //public GameObject loseCanvasPrefab;
    private GameObject loseCanvas;

    [SerializeField]
    private Helper currentHelper;
    private BoardCanvasController bcc;

    //public List<GameObject> backgroundPrefabs = new List<GameObject>();
    private GameObject background;
    public List<string> backgrounds;

    private Camera cam;
    private GameObject board;

    private int startLevel;
    private int currentLevel;

    private PlayerDataController playerDataController;

    public enum _state { intro, world, board };
    private _state gameState;

    public GameObject endBoardPanelPrefab;
    //public GameObject startBoardPanelPrefab;
    private GameObject ebp;
    private Vector2 boardSize;
    private GameObject lanternPrefab;

    public GameObject fireworkPrefab;

    public float showLoseLerpTime;

    [SerializeField]
    private bool pauseMenuEnabled;
    [SerializeField]
    private bool paused;

    private int levelClicked = -1;

    [SerializeField]
    private bool helperEnabled;
    public enum _helperType { None, Hammer, Vertical, Horizontal, Rainbow, Bomb };

    public delegate void DelegateHandleSceneEvent();
    DelegateHandleSceneEvent sceneHandler;

    public static GameController GetGameController()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public bool SoundFXStatus
    {
        get { return soundController.SoundFXOn; }
    }

    public bool MusicStatus
    {
        get { return musicController.MusicOn; }
    }

    public int LevelClicked
    {
        get { return levelClicked; }
        set { levelClicked = value; }
    }

    public int PlayerLastLevel
    {
        get { return playerDataController.playerData.lastLevel; }
    }

    public bool Paused
    {
        get { return paused; }
    }

    public bool HelperEnabled
    {
        get { return helperEnabled; }
        set { helperEnabled = false; }
    }
    public int PlayerSeenIntro()
    {
        return playerDataController.playerData.seenIntro;
    }

    public void SetPlayerSeenIntro(int seen)
    {
        playerDataController.playerData.seenIntro = 1;
        playerDataController.SavePlayerData();
    }

    public void SetHelper(GameObject helper)
    {
        if (helper)
        {
            board.GetComponent<Board>().HelperType = helper.GetComponent<Helper>().HelperType;
            currentHelper = helper.GetComponent<Helper>();
        }
        else
        {
            currentHelper = null;
            board.GetComponent<Board>().HelperType = _helperType.None;
            bcc.GetComponent<BoardCanvasController>().HandlePostHelper();
        }
    }

    public void DeductHelper()
    {
        currentHelper.Amount--;
        playerDataController.DeductHelper(currentHelper.HelperType);

    }

    public void UpdateTimer(int timeLeft)
    {
        bcc.UpdateTimer(timeLeft);
    }

	void Awake () {
        //Application.targetFrameRate = 30;
        Screen.SetResolution(1080, 1920, true);
        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 0;
        cam = Camera.Instantiate(mainCamera);

        playerDataController = transform.GetComponent<PlayerDataController>();
        playerDataController.LoadPlayerData();
        Debug.Log(playerDataController.playerData.lastLevel);
        dataController = transform.GetComponent<DataController>();
        dataController.LoadGameData();
        gameState = _state.intro;
        Object.DontDestroyOnLoad(transform);
        Object.DontDestroyOnLoad(cam);
        soundController = GetComponent<SoundController>();
        musicController = GetComponent<MusicController>();
        pauseMenuEnabled = true;
        paused = false;
        helperEnabled = true;
        if (playerDataController.playerData.sfxOnOff == 1)
        {
            soundController.SoundFXOn = true;
        } 
        else
        {
            soundController.SoundFXOn = false;
        }
        if (playerDataController.playerData.musicOnOff == 1)
        {
            musicController.MusicOn = true;
        }
        else
        {
            musicController.MusicOn = false;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishLoading;
    }

    public void Start()
    {
        cam.GetComponent<Camera>().orthographic = false;
        cam.GetComponent<Camera>().fieldOfView = 100;
        cam.transform.position = new Vector3(0f, 0f, -19f);
        //ShowStartButton();
        GetComponent<Fading>().FadeIn();


    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Name = " + hit.collider.name);
                Debug.Log("Tag = " + hit.collider.tag);
                Debug.Log("Hit Point = " + hit.point);
                Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
                Debug.Log("--------------");
            }
        }
        */
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

    //private void ShowStartButton()
    //{
    //    StartCoroutine(PauseForSplash());
    //}

    //IEnumerator PauseForSplash()
    //{
    //    yield return new WaitForSeconds(5f);
    //    GameObject.FindGameObjectWithTag("SplashCanvas").GetComponent<Canvas>().enabled = true;
    //}

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
        if (dataController.gameData.levelData[currentLevel].rewards != null && dataController.gameData.levelData[currentLevel].rewards.Count > 0)
        {
            playerDataController.AddRewards(dataController.gameData.levelData[currentLevel].rewards);
        }
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
        //mc.PlayTrack(0);
        mc.PlayRandomTrack();
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    //public void ShowPauseButton()
    //{
    //    if (!pauseButton)
    //    {
    //        pauseButton = GameObject.Instantiate(pauseButtonPrefab);
    //    }
    //    pauseButton.SetActive(true);
    //}

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

        SceneManager.LoadScene(3);
        if (!cam)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        cam.orthographic = true;
        cam.transform.position = new Vector3(0.0f, 0.0f, -10f);
        cam.transform.rotation = Quaternion.Euler(Vector3.zero);
        cam.orthographicSize = 6.0f;
    }

    public void PlayMusic(int track)
    {
        if (track < 0)
        {
            musicController.PlayRandomTrack();
        }
        else
        {
            musicController.PlayTrack(track);
        }
    }

    private void OnLevelFinishLoading(Scene scene, LoadSceneMode mode) 
    {
        if (scene.name == "Board")
        {
            gameState = _state.board;
            StartBoard();
            LevelPostStart();
            sceneHandler = null;
            //background = GameObject.Instantiate(backgroundPrefabs[currentLevel % backgroundPrefabs.Count]);
            background = GameObject.Instantiate((GameObject)Resources.Load("Backgrounds/" + backgrounds[currentLevel % backgrounds.Count]));
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

    public void RegisterHandler(DelegateHandleSceneEvent toRegister)
    {
        sceneHandler = toRegister;
    }

    public void IntermediateScreen()
    {
        SocialLogin();
    }

    public void SocialLogin()
    {        
        Debug.Log(Social.Active);
#if UNITY_IPHONE
        Social.localUser.Authenticate(ProcessAuthentication);
#else
        BackToWorld();
#endif
    }

    void ProcessAuthentication(bool success)
    {
        if (success)
        {
            Debug.Log("authenticated");
            Debug.Log(Social.localUser.id);
                
        }
        else
        {
            Debug.Log("failed");
        }
        BackToWorld();
    }

    public void BackToWorld()
    {       
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (gc)
        {
            pauseMenuEnabled = true;
            gc.CancelInvoke();
            gc.StopAllCoroutines();
            gc.GetComponent<AudioSource>().Stop();
            Resources.UnloadUnusedAssets();

            gc.gameState = _state.world;
            if (!cam)
            {
                cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }
            //cam.GetComponent<Camera>().orthographic = true;
            //cam.GetComponent<Camera>().orthographicSize = 6;         
            cam.transform.position = new Vector3(5.7f, 38.6f, -22.2f);
            cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            cam.GetComponent<Camera>().fieldOfView = 60f;
            cam.GetComponent<Camera>().depth = -1f;
            cam.GetComponent<Camera>().farClipPlane = 150;
            cam.GetComponent<Camera>().nearClipPlane = 0.3f;
            cam.GetComponent<Camera>().orthographic = false;


            SceneManager.LoadScene(1);
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
        loseCanvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/LoseCanvas"));
        loseCanvas.GetComponent<RectTransform>().position = new Vector3(0f, 20f, 0f);
        board.GetComponent<Board>().HideTiles();
        StartCoroutine(DropFromAbove(loseCanvas, showLoseLerpTime, new Vector3(0f, 0f, 0f)));
    }

    IEnumerator DropFromAbove(GameObject go, float lerpTime, Vector3 newPosition)
    {
        float startTime = Time.time;
        float perc = 0;
        Vector3 originalPosition;
        if (go.GetComponent<RectTransform>())
        {
            originalPosition = go.transform.GetComponent<RectTransform>().position;
        }
        else
        {
            originalPosition = go.transform.position;
        }
        while((Time.time - startTime) < lerpTime)
        {
            perc = (Time.time - startTime) / lerpTime;
            if (go.GetComponent<RectTransform>())
            {
                go.GetComponent<RectTransform>().position = Vector3.Lerp(originalPosition, newPosition, perc);
            }
            else
            {
                go.transform.position = Vector3.Lerp(originalPosition, newPosition, perc);
            }
            yield return null;
        }

    }

    public void WinButtonGoBackToWorld()
    {
        BackToWorld();
    }

    public void PlayBoing()
    {
        soundController.PlayBoing();
    }

    public void PlayWinSound()
    {
        soundController.PlayWinSound();
    }

    public void PlayWindSound()
    {
        soundController.PlayWinSound();
    }

    public void PlayThunderSound()
    {
        soundController.PlayThunderRandom();
    }

    public void PlayRainSound()
    {
        soundController.PlayRainSound();
    }

    public void PlayTileDestroySound(int cascadeCount)
    {
        soundController.PlayTileDestroySound(cascadeCount);
    }

    public void PlayKickSound() 
    {
        soundController.PlayKickSound();
    }

    public void PlayBTSSound()
    {
        soundController.PlayBTSSound();
    }

    public void PlayGreatSound()
    {
        soundController.PlayGreat();
    }

    public void PlayWooHooSound()
    {
        soundController.PlayWooHoo();
    }
    
    public void PlaySpringInMyStep()
    {
        musicController.PlaySpring();
    }

    public void PlayCreep()
    {
        musicController.PlayCreep();
    }
    public void PlayLoseSound()
    {
        soundController.PlayLose();
    }

    public void PlaySwishUp()
    {
        soundController.PlaySwishUp();
    }

    public void PlayStarSound()
    {
        soundController.PlayStarSound();
    }

    public void PlaySwishDown()
    {
        soundController.PlaySwishDown();
    }

    public void PlayChooseLevel()
    {
        soundController.PlayChooseLevel();
    }

    public void ToggleSoundButton()
    {
        soundController.ToggleSoundButton();
        playerDataController.EnableSFX(soundController.SoundFXOn);
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
        playerDataController.EnableMusic(musicController.MusicOn);
    }

    public void HandleSwitchMatchEvent()
    {
        if (sceneHandler != null)
        {
            sceneHandler();
        }
    }

    public void ShowStartBoard()
    {
        boardCanvas = GameObject.Instantiate(boardCanvasPrefab);
        if (boardCanvas)
        {
            bcc = boardCanvas.GetComponent<BoardCanvasController>();
        }
        soundController.PlaySwishUp();
        GameObject sbp = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/StartBoardPanel"));

        Debug.Log(sbp);
        StartBoardController sbc = sbp.GetComponent<StartBoardController>();
        sbc.SetStartPanelText("");
        if (dataController.gameData.levelData[currentLevel].mission.type == 0)
        {
            sbc.AppendStartPanelText("\nGet " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].score + " in " + dataController.gameData.levelData[currentLevel].numMoves + " moves.\nGood Luck!");
        }
        if (dataController.gameData.levelData[currentLevel].mission.type == 1)
        {
            sbc.AppendStartPanelText("\nMatch the following pieces: \n");
            sbc.ShowMissionGoals(dataController.gameData.levelData[currentLevel].mission.missionGoals, board);
        }
        if (dataController.gameData.levelData[currentLevel].mission.type == 2)
        {
            sbc.AppendStartPanelText("\nBring " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].numfall + " to the bottom");
            if (!bcc)
            {
                bcc = GameObject.FindGameObjectWithTag("BoardCanvasController").GetComponent<BoardCanvasController>();
            }
            bcc.ShowDropCountPanel();
            bcc.SetDropCountText("0 / " + dataController.gameData.levelData[currentLevel].mission.missionGoals[0].numfall);

        }
        if (dataController.gameData.levelData[currentLevel].rewards.Count > 0)
        {
            sbc.ShowRewards(dataController.gameData.levelData[currentLevel].rewards);
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
            //gc.ShowPauseButton();
            bcc = GameObject.FindGameObjectWithTag("BoardCanvas").GetComponent<BoardCanvasController>();
            //helperPanel = GameObject.Instantiate(helperPanelPrefab);
            //helperPanel.GetComponent<HelperCanvasController>().CreateHelpers(gc.dataController.gameData.levelData[gc.CurrentLevel], gc.GetPlayerData(), helperPanel.transform);
            bcc.ShowHelpers(gc.dataController.gameData.levelData[gc.currentLevel], gc.GetPlayerData());
            gc.board.GetComponent<Board>().SetTimer(gc.dataController.gameData.levelData[currentLevel].timer);
        }
    }

    public void ShowEndBoard(int score, int stars)
    {
        ebp = GameObject.Instantiate(endBoardPanelPrefab);
        ebp.GetComponent<EndBoardPanelController>().SetText(score + " points!");
        ebp.GetComponent<EndBoardPanelController>().SetStars(stars);
        board.GetComponent<Board>().HideTiles();
    }

    public void HideMainBoard()
    {
        board.GetComponent<Board>().HideBoard();
    }

    public void ShowMainBoard()
    {
        board.GetComponent<Board>().ShowBoard();
    }

    public void ShowSkyLanterns()
    {
        if (!lanternPrefab)
        {
            lanternPrefab = Resources.Load("Sprites/sky_lantern") as GameObject;
        }
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

    //public void DestroyPauseMenu()
    //{
    //    if (pauseMenu)
    //    {
    //        Destroy(pauseMenu);
    //    }
    //}
    
    public void PauseGame()
    {
        board.GetComponent<Board>().TimerPaused = true;
        paused = true;
        board.GetComponent<Board>().Locked = true;
        HideMainBoard();
        helperEnabled = false;
    }

    public void ResumeGame()
    {
        paused = false;
        board.GetComponent<Board>().TimerPaused = false;
        board.GetComponent<Board>().Locked = false;
        ShowMainBoard();
        helperEnabled = true;
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

    public LevelData GetLevelData(int level)
    {
        return dataController.gameData.levelData[level];
    }

    public void Darken()
    {
        board.GetComponent<Board>().Darken();
    }

    public void Undarken()
    {
        board.GetComponent<Board>().Undarken();
    }

    public void HideEndBoardPanel()
    {
        GameObject go = GameObject.FindGameObjectWithTag("EndBoardPanel");
        if (go) {
            go.SetActive(false);
        }
    }


}
