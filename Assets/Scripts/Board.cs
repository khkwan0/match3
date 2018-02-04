using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour {

    public enum LeftRightNeither { Left, Right, Neither };

    public static int maxRows = 8;
    public static int maxCols = 8;

    public List<GameObject> tiles = new List<GameObject>();
    public GameObject tileMaskPrefab;
    public List<GameObject> powerTilesVertical = new List<GameObject>();
    public List<GameObject> powerTilesHorizontal = new List<GameObject>();
    public List<GameObject> powerTilesCross = new List<GameObject>();
    public List<GameObject> generatorTiles = new List<GameObject>();
    public GameObject rainbowTile;
    public GameObject unknownCrackable;
    public GameObject rabbitPrefab;
    private GameObject[,] board;
    private GameObject[,] backgroundBoard;
    public static Vector2 tileSize;
    public static Vector2 boardSize;

    public GameObject tileBackgroundLayerContainerPrefab;
    private GameObject tileBackgroundLayerContainer;
    public GameObject tileBackground;

    [SerializeField]
    private bool locked = false;
    private bool winLocked = false;

    [SerializeField]
    private int falling;
    [SerializeField]
    private int generating;
    public GameObject canvas;
    //private GameObject GUI;
    private TextMeshProUGUI moves;
    private int numMoves;
    private int maxMoves;
    private int numScore;

    public int scoreAmt;
    public int horizontalBlastScoreAmt;
    public int verticalBlastScoreAmt;
    public int crossBlastScoreAmt;
    public int rainbowScoreAmt;
    public float floatingScoreLifeTimeSeconds;

    public float hintTimeout = 3.0f;
    [SerializeField]
    private int hintI;
    [SerializeField]
    private int hintJ;

    private GameData gameData;
    private GameController gameController;
    private int level;
    [SerializeField]
    private int scoreMultiplier;

    [SerializeField]
    private int cascadeCount;

    private int maxFillScore;
    private int stars = 0;
    private List<BoardSpec> boardSpec;

    public GameObject TileExplosion;
    private int missionType;
    [SerializeField]
    private bool finishing;
    int reshufflingCount;
    int finalPointBonus;

    public GameObject virusPrefab;
    public GameObject woodEnclosurePrefab;

    private GameObject waves;
    private GameObject waves2;

    private List<GameObject> toDestroy = new List<GameObject>();

    public GameObject floatingScore;
    private int tier1Fill;
    private int tier2Fill;
    private int tier3Fill;

    [SerializeField]
    private int virusCount;
    [SerializeField]
    private bool virusCure;

    [SerializeField]
    private bool didFill;
    [SerializeField]
    private bool didCornerFill;

    [SerializeField]
    private int dropCount;
    [SerializeField]
    private int dropCountSpawned;
    [SerializeField]
    private int numFall;

    public float fallLerpTime;

    private Vector3 localScale;
        
    public void SetBoardSize(Vector2 _size)
    {
        boardSize = _size;
    }

    public void SetGameData(GameData gd)
    {
        gameData = gd;
    }

    public void SetGameController(GameController gc)
    {
        gameController = gc;
    }

    public bool Locked
    {
        get { return locked; }
        set { locked = value; }
    }

    public bool WinLocked
    {
        get { return winLocked; }
    }

    public int Falling
    {
        get { return falling; }
        set { falling = value; }
    }

    public int Level
    {
        get { return level; }
    }

    public int MissionType
    {
        get { return missionType; }
    }

    public int Score
    {
        get { return numScore; }
    }

    private string destroyEffect;

    public void Start()
    {
        waves2 = GameObject.Instantiate(Resources.Load("Sprites/wave2") as GameObject);
        waves = GameObject.Instantiate(Resources.Load("Sprites/wave1") as GameObject);

        waves2.transform.position = new Vector3(1f, -1f * boardSize.y/2.5f + 1f, 1f);

        waves.transform.position = new Vector3(0f, -1f * boardSize.y / 2.5f, 0.9f);
        waves2.transform.SetParent(waves.transform);
    }

    public void StartLevel(int level)
    {
        Debug.Log("startlevel");
        gameController.ShowStartBoard();
        virusCount = 0;
        dropCountSpawned = 0;
        virusCure = false;
        localScale = new Vector3(0.4f, 0.4f, 1f);
        scoreAmt = 20;
        finishing = false;
        stars = 0;
        reshufflingCount = 0;
        finalPointBonus = 100;
        dropCount = 0;
        tileSize = new Vector2(tiles[0].GetComponent<Renderer>().bounds.size.x, tiles[0].GetComponent<Renderer>().bounds.size.y);

        gameController.UpdateSoundButton();
        cascadeCount = 0;
        scoreMultiplier = cascadeCount + 1;

        numMoves = gameData.levelData[level].numMoves;
        maxMoves = numMoves;
        numScore = 0;
        missionType = gameData.levelData[level].mission.type;

        this.level = gameData.levelData[level].level;
        maxRows = gameData.levelData[level].rows;
        maxCols = gameData.levelData[level].cols;
   
        moves = GameObject.FindGameObjectWithTag("MovesTextArea").GetComponent<TextMeshProUGUI>();
        moves.text = numMoves.ToString();

        //missionText = GUI.transform.Find("Mission").GetComponent<TextMeshProUGUI>();
        maxFillScore = gameData.levelData[level].maxFillPoints;
        tier1Fill = gameData.levelData[level].tier1Fill;
        tier2Fill = gameData.levelData[level].tier2Fill;
        tier3Fill = gameData.levelData[level].tier3Fill;
        gameController.PlaceStars(tier1Fill, tier2Fill, tier3Fill, maxFillScore);
        if (gameData.levelData[level].mission.type == 2)
        {
            numFall = gameData.levelData[level].mission.missionGoals[0].numfall;
        }
        //if (gameData.levelData[level].mission.type == 0)
        //{
        //    missionText.text = "Get " + gameData.levelData[level].mission.missionGoals[0].score + " in " + numMoves + " moves or less.";
        //}

        if (gameData.levelData[level].mission.type == 1)
        {
            for (int idx = 0; idx < gameData.levelData[level].mission.missionGoals.Count; idx++)
            {
                Sprite theSprite = null;
                TilePiece._TileType tileType = TilePiece._TileType.Regular;
                int tileValue = gameData.levelData[level].mission.missionGoals[idx].tilevalue;
                switch (gameData.levelData[level].mission.missionGoals[idx].tiletype)
                {
                    case "regular": tileType = TilePiece._TileType.Regular; theSprite = tiles[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "horizontal": tileType = TilePiece._TileType.HorizontalBlast; theSprite = powerTilesHorizontal[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "vertical": tileType = TilePiece._TileType.VerticalBlast; theSprite = powerTilesVertical[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "cross": tileType = TilePiece._TileType.CrossBlast; theSprite = powerTilesCross[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "rainbow": tileType = TilePiece._TileType.Rainbow; theSprite = rainbowTile.GetComponent<SpriteRenderer>().sprite; break;
                    case "generator": tileType = TilePiece._TileType.Generator; theSprite = generatorTiles[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    default: break;
                }
                gameController.SpawnMissionGoal(gameData.levelData[level].mission.missionGoals[idx].toreach, tileType, tileValue, idx, theSprite);
            }
        }

        hintI = hintJ = -1;
        boardSpec = gameData.levelData[level].boardSpec;
        DrawBoard();
        while (!GetHint())
        {
            //Debug.Log("Reshuffle");
            for (int i = 0; i < maxRows; i++)
            {
                for (int j = 0; j < maxCols; j++)
                {
                    Destroy(board[i, j]);
                }
            }
            Destroy(tileBackgroundLayerContainer);
            DrawBoard();
        }
        ShowTiles();
        InvokeRepeating("ShowHint", hintTimeout, hintTimeout);
    }

    public void EnableHintStart()
    {
        StartCoroutine(EnableHint());
    }

    private void DrawBoard()
    {
        board = new GameObject[maxRows, maxCols];
        backgroundBoard = new GameObject[maxRows, maxCols];
        tileBackgroundLayerContainer = GameObject.Instantiate(tileBackgroundLayerContainerPrefab);
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {

                bool specFound = false;
                for (int k = 0; k < boardSpec.Count && !specFound; k++)
                {
                    if (boardSpec[k].row == i && boardSpec[k].col == j)
                    {
                        if (boardSpec[k].invisible != 1)
                        {
                            backgroundBoard[i, j] = GameObject.Instantiate(tileBackground, GetScreenCoordinatesWithZ(i, j, 0.5f), Quaternion.identity, tileBackgroundLayerContainer.transform);
                            backgroundBoard[i, j].name = "bkgd_r" + i + "_c" + j;
                            backgroundBoard[i, j].GetComponent<TileBackgroundController>().SetLocation(i, j);
                            if (boardSpec[k].istele == 1)
                            {
                                backgroundBoard[i, j].GetComponent<TileBackgroundController>().IsTele = true;
                                backgroundBoard[i, j].GetComponent<TileBackgroundController>().TeleDirection = TileBackgroundController._teleDirection.In;
                                backgroundBoard[i, j].GetComponent<TileBackgroundController>().SetTeleIJ(boardSpec[k].telefromrow, boardSpec[k].telefromcol);
                                GameObject.Instantiate(tileMaskPrefab, GetScreenCoordinates(i + 1, j), Quaternion.identity).name = "tilemask";
                                GameObject.Instantiate(tileMaskPrefab, GetScreenCoordinates(boardSpec[k].telefromrow - 1, boardSpec[k].telefromcol), Quaternion.identity);

                            }
                        }
                        specFound = true;
                        if (boardSpec[k].tiletype == "regular"
                            || boardSpec[k].tiletype == "vertical"
                            || boardSpec[k].tiletype == "horizontal"
                            || boardSpec[k].tiletype == "cross"
                            || boardSpec[k].tiletype == "rainbow"
                            || boardSpec[k].tiletype == "generator"
                            || boardSpec[k].tiletype == "rabbit"
                            || boardSpec[k].tiletype == "unknowncrackable")
                        {
                            GameObject prefab = null;
                            switch (boardSpec[k].tiletype)
                            {
                                case "cross": prefab = powerTilesCross[boardSpec[k].value]; break;
                                case "vertical": prefab = powerTilesVertical[boardSpec[k].value]; break;
                                case "horizontal": prefab = powerTilesHorizontal[boardSpec[k].value]; break;
                                case "rainbow": prefab = rainbowTile; break;
                                case "generator": prefab = generatorTiles[boardSpec[k].value]; break;
                                case "unknowncrackable": prefab = unknownCrackable; break;
                                case "rabbit": prefab = rabbitPrefab; break;
                                default: break;
                            }
                            int value = Random.Range(0, tiles.Count);
                            if (boardSpec[k].tiletype == "regular" && boardSpec[k].value == -1)
                            {
                                prefab = tiles[value];
                            }
                            else if (boardSpec[k].tiletype == "regular" && boardSpec[k].value >= 0)
                            {
                                prefab = tiles[boardSpec[k].value];
                            }
                            if (prefab)
                            {
                                board[i, j] = GameObject.Instantiate(
                                    prefab,
                                    GetScreenCoordinates(i, j),
                                    Quaternion.identity
                                );
                            }
                            if (boardSpec[k].tiletype == "unknowncrackable")
                            {
                                board[i, j].GetComponent<TilePiece>().HitPoints = boardSpec[k].hitpoints;
                            }
                            if (boardSpec[k].tiletype == "regular" && boardSpec[k].value == -1)
                            {
                                board[i, j].GetComponent<TilePiece>().Value = value;
                                board[i, j].GetComponent<TilePiece>().OriginalValue = value;
                            }
                            else
                            {
                                board[i, j].GetComponent<TilePiece>().Value = boardSpec[k].value;
                                board[i, j].GetComponent<TilePiece>().OriginalValue = boardSpec[k].value;
                            }

                            board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                            board[i, j].GetComponent<TilePiece>().Moveable = boardSpec[k].immoveable == 1 ? false : true;
                            board[i, j].GetComponent<TilePiece>().Indestructable = boardSpec[k].indestructable == 1 ? true : false;
                            board[i, j].GetComponent<TilePiece>().Invisible = boardSpec[k].invisible == 1 ? true : false;
                            board[i, j].GetComponent<TilePiece>().NonBlocking = boardSpec[k].nonblocking == 1 ? true : false;
                            TilePiece._TileType tt;
                            switch (boardSpec[k].tiletype)
                            {
                                case "regular": tt = TilePiece._TileType.Regular; break;
                                case "horizontal": tt = TilePiece._TileType.HorizontalBlast; break;
                                case "vertical": tt = TilePiece._TileType.VerticalBlast; break;
                                case "cross": tt = TilePiece._TileType.CrossBlast; break;
                                case "rainbow": tt = TilePiece._TileType.Rainbow; break;
                                case "generator": tt = TilePiece._TileType.Generator; break;
                                case "unknowncrackable": tt = TilePiece._TileType.UnknownCrackable; break;
                                case "rabbit": tt = TilePiece._TileType.DropCount; break;
                                default: tt = TilePiece._TileType.Regular; break;
                            }
                            board[i, j].GetComponent<TilePiece>().TileType = tt;
                            board[i, j].GetComponent<TilePiece>().OriginalTileType = tt;
                            board[i, j].name = prefab.name + "Xr" + i + "c" + j;
                            if (boardSpec[k].tiletype == "regular"
                                || boardSpec[k].tiletype == "vertical"
                                || boardSpec[k].tiletype == "horizontal"
                                || boardSpec[k].tiletype == "cross"
                                || boardSpec[k].tiletype == "rainbow"
                                || boardSpec[k].tiletype == "rabbit")
                            {
                                board[i, j].GetComponent<TilePiece>().OriginalMoveable = true;
                            } else
                            {
                                board[i, j].GetComponent<TilePiece>().OriginalMoveable = false;
                            }
                            if (tt == TilePiece._TileType.DropCount)
                            {
                                dropCountSpawned++;
                            }
                          
                        }
                        else
                        {
                            board[i, j] = new GameObject();
                            board[i, j].AddComponent<TilePiece>();
                            board[i, j].transform.SetPositionAndRotation(
                                    GetScreenCoordinates(i, j),
                                    Quaternion.identity
                                );
                            board[i, j].AddComponent<BoxCollider2D>();
                            board[i, j].GetComponent<TilePiece>().Value = -1;
                            board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                            board[i, j].transform.localScale = localScale;
                            if (boardSpec[k].immoveable == 1)
                            {
                                board[i, j].GetComponent<TilePiece>().Moveable = false;
                                board[i, j].GetComponent<TilePiece>().OriginalMoveable = false;
                            }
                            if (boardSpec[k].indestructable == 1)
                            {
                                board[i, j].GetComponent<TilePiece>().Indestructable = true;
                            }
                            if (boardSpec[k].invisible == 1)
                            {
                                board[i, j].GetComponent<TilePiece>().Invisible = true;
                            }
                            if (boardSpec[k].nonblocking == 0)
                            {
                                board[i, j].AddComponent<BoxCollider2D>();
                                board[i, j].GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
                                board[i, j].GetComponent<TilePiece>().NonBlocking = false;
                            }
                            else
                            {
                                board[i, j].GetComponent<TilePiece>().NonBlocking = true;
                            }
                            if (boardSpec[k].steel == 1)
                            {
                                board[i, j].name = "Steel Tile" + "r" + i + "c" + j;
                                board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Steel;
                                board[i, j].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.Steel;
                                board[i, j].transform.localScale = localScale;
                                board[i, j].AddComponent<SpriteRenderer>();
                                board[i, j].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Sprites/steelTile", typeof(Sprite));
                            }
                            if (board[i, j].GetComponent<TilePiece>().Indestructable
                                && board[i, j].GetComponent<TilePiece>().Invisible
                                && board[i, j].GetComponent<TilePiece>().NonBlocking
                                && !board[i, j].GetComponent<TilePiece>().Moveable)
                            {
                                board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Blank;
                                board[i, j].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.Blank;
                                board[i, j].name = "blank" + "r" + i + "c" + j;
                            }
                        }
                        if (boardSpec[k].overlay.Count > 0)
                        {
                            TilePiece._OverlayType overlayType;
                            foreach(string overlay in boardSpec[k].overlay)
                            {
                                switch(overlay)
                                {
                                    case "enc": overlayType = TilePiece._OverlayType.Enclosure; break;
                                    case "vir": overlayType = TilePiece._OverlayType.Virus; break;
                                    default: overlayType = TilePiece._OverlayType.None; break;
                                }
                                board[i, j].GetComponent<TilePiece>().AddOverlay(overlayType);
                                AddOverlay(i, j, overlayType);
                            }
                        }
                        board[i, j].SetActive(false);
                    }
                }
                if (!specFound)
                {
                    backgroundBoard[i, j] = GameObject.Instantiate(tileBackground, GetScreenCoordinatesWithZ(i, j, 0.5f), Quaternion.identity, tileBackgroundLayerContainer.transform);
                    backgroundBoard[i, j].name = "bkgd_r" + i + "_c" + j;
                    backgroundBoard[i, j].GetComponent<TileBackgroundController>().SetLocation(i, j);
                    int idx = Random.Range(0, tiles.Count);
                    while (IsContiguousHorizontal(i, j, idx) || IsContiguousVertical(i, j, idx))
                    {
                        idx = (int)Mathf.FloorToInt(Random.Range(0f, tiles.Count - 1));
                    }
                    board[i, j] = GameObject.Instantiate(
                        tiles[idx],
                        GetScreenCoordinates(i, j),
                        Quaternion.identity
                    );
                    board[i, j].SetActive(false);
                    board[i, j].GetComponent<TilePiece>().Value = idx;
                    board[i, j].GetComponent<TilePiece>().OriginalValue = idx;
                    board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                    board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                    board[i, j].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.Regular;
                    board[i, j].GetComponent<TilePiece>().NonBlocking = false;
                    board[i, j].name = tiles[idx].name + "r" + i + "c" + j;
                }
            }
        }
    }

    private void AddOverlay(int i, int j, TilePiece._OverlayType overlayType)
    {
        GameObject prefab = null;
        float z = 0;
        switch (overlayType)
        {
            case TilePiece._OverlayType.Enclosure: prefab = woodEnclosurePrefab; z = -0.1f; break;
            case TilePiece._OverlayType.Virus: prefab = virusPrefab; z = -0.2f; virusCount++;  break;
            default: break;
        }
        if (prefab)
        {
            board[i, j].GetComponent<TilePiece>().Moveable = false;
            GameObject.Instantiate(prefab, GetScreenCoordinatesWithZ(i, j, z), Quaternion.identity, board[i, j].transform);
        }
    }        

    public void ShowHint()
    {
        //Debug.Log("Show hint hintI:" + hintI + "hintJ: " + hintJ);
        if (hintI >= 0 && hintJ >= 0)
        {
            board[hintI, hintJ].GetComponent<TilePiece>().ShowHint();
        } else
        {
            GetHint();
        }
    }

    private void ShowTiles()
    {
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j< maxCols; j++)
            {
                board[i, j].SetActive(true);
                board[i, j].GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
            }
        }
    }

    private bool IsContiguousHorizontal(int i, int j, int value)
    {
        if (j > 1)
        {
            if (board[i, j - 1].GetComponent<TilePiece>().Value != value && board[i, j - 2].GetComponent<TilePiece>().Value != value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private bool IsContiguousVertical(int i, int j, int value)
    {
        if (i > 1)
        { 
            if (board[i - 1, j].GetComponent<TilePiece>().Value != value && board[i - 2, j].GetComponent<TilePiece>().Value != value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public GameObject[,] GetBoard()
    {
        return board;
    }

    public bool FoundSwitchMatch(int i, int j, int switchedI, int switchedJ)
    {
        bool matches = false;
        TilePiece._TileType tileType, switchedTileType;

        tileType = board[i, j].GetComponent<TilePiece>().TileType;
        switchedTileType = board[switchedI, switchedJ].GetComponent<TilePiece>().TileType;

        if (tileType == TilePiece._TileType.Rainbow || switchedTileType == TilePiece._TileType.Rainbow)
        {
            HandleRainbowMatch(i, j, switchedI, switchedJ);
            matches = true;
        } else
        {
            switch(Random.Range(0, 2))
            {
                case 0: destroyEffect = "explode"; break;
                case 1: destroyEffect = "fall"; break;
                default:break;
            }
            matches = CheckIJ(i, j);
            matches |= CheckIJ(switchedI, switchedJ);
        }

        if (matches)
        {
            //FinalizeBoard();
            //Fill();
            CancelInvoke("ShowHint");
            FinalizeUnmoveable2(true);
            numMoves--;
            UpdateMoves(numMoves);
            cascadeCount = 0;
            scoreMultiplier = cascadeCount + 1;
        }                   
        return matches;            
    }

    IEnumerator EnableHint()
    {
        while (falling > 0)
        {
            yield return null;
        }
        //Debug.Log("Enable hint");
        GetHint();
        if (hintI == -1 || hintJ == -1)
        {
            ReshuffleDrawn();
        }
        InvokeRepeating("ShowHint", hintTimeout, hintTimeout);
    }

    public bool HasWon()
    {
        bool win = false;
        
        if (gameData.levelData[level].mission.type == 0)
        {
            if (numScore >= gameData.levelData[level].mission.missionGoals[0].score)
            {
                win = true;
            }
        }
        if (gameData.levelData[level].mission.type == 1)
        {
            int goalCount = 0;
            for (int i = 0; i < gameData.levelData[level].mission.missionGoals.Count; i++)
            {
                if (gameData.levelData[level].mission.missionGoals[i].tiletype == "regular")                 
                {
                    if (gameController.GetTileCount(level, TilePiece._TileType.Regular, gameData.levelData[level].mission.missionGoals[i].tilevalue) >= gameData.levelData[level].mission.missionGoals[i].toreach)
                    {
                        goalCount++;
                    }
                }
                else if (gameData.levelData[level].mission.missionGoals[i].tiletype == "horizonal")
                { 
                    if (gameController.GetTileCount(level, TilePiece._TileType.HorizontalBlast, gameData.levelData[level].mission.missionGoals[i].tilevalue) >= gameData.levelData[level].mission.missionGoals[i].toreach)
                    {
                        goalCount++;
                    }
                }
                else if (gameData.levelData[level].mission.missionGoals[i].tiletype == "vertical")
                {
                    if (gameController.GetTileCount(level, TilePiece._TileType.VerticalBlast, gameData.levelData[level].mission.missionGoals[i].tilevalue) >= gameData.levelData[level].mission.missionGoals[i].toreach)
                    {
                        goalCount++;
                    }
                }
                else if (gameData.levelData[level].mission.missionGoals[i].tiletype == "cross")
                { 
                    if (gameController.GetTileCount(level, TilePiece._TileType.CrossBlast, gameData.levelData[level].mission.missionGoals[i].tilevalue) >= gameData.levelData[level].mission.missionGoals[i].toreach)
                    {
                        goalCount++;
                    }
                }
                else if (gameData.levelData[level].mission.missionGoals[i].tiletype == "rainbow")
                {
                    if (gameController.GetTileCount(level, TilePiece._TileType.Rainbow, gameData.levelData[level].mission.missionGoals[i].tilevalue) >= gameData.levelData[level].mission.missionGoals[i].toreach)
                    {
                        goalCount++;
                    }
                }
            }
            if (goalCount >= gameData.levelData[level].mission.missionGoals.Count)
            {
                win = true;
            }
        }
        if (gameData.levelData[level].mission.type == 2)
        {
            if (dropCount >= gameData.levelData[level].mission.missionGoals[0].numfall)
            {
                win = true;
            }

        }
        return win;
    }

    public bool CheckLoseCondition()
    {
        bool lose = false;
        if (gameData.levelData[level].mission.type == 0)
        {
            if (numScore < gameData.levelData[level].mission.missionGoals[0].score && numMoves <= 0)
            {
                lose = true;
            }
        }
        if (gameData.levelData[level].mission.type == 1)
        {
            if (numMoves <= 0)
            {
                lose = true;
            }
        }
        if (gameData.levelData[level].mission.type == 2)
        {
            if (numMoves <= 0)
            {
                lose = true;
            }
        }
        return lose;
    }

    private bool CheckIJ(int i, int j)
    {
        int lowestRow, lowestCol;
        int crossLowestRow, crossLowestCol;
        int horCount, vertCount;

        horCount = CheckHorizontal(i, j, out lowestCol);
        vertCount = CheckVertical(i, j, out lowestRow);

        if (horCount >= 5 || vertCount >= 5)
        {
            if (horCount >= 5)
            {
                HandleFiveMatch(i, j, true, lowestCol);
            }
            else
            {
                HandleFiveMatch(i, j, false, lowestRow);
            }
        }
        else if (CheckCross(i, j, out crossLowestRow, out crossLowestCol))
        {
            HandleCrossMatch(i, j, crossLowestRow, crossLowestCol);
        }
        else if (horCount == 4 || vertCount == 4)
        {
            if (horCount == 4)
            {
                HandleFourMatch(i, j, lowestCol, true);
            }
            else
            {
                HandleFourMatch(i, j, lowestRow, false);
            }
        }
        else if (horCount == 3 || vertCount == 3)
        {
            if (horCount == 3)
            {
                HandleThreeMatch(i, j, lowestCol, true);
            }
            else
            {
                HandleThreeMatch(i, j, lowestRow, false);
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    private bool CheckIJCascade(int i, int j) {

        if (HandledCascadeCross(i, j))
        {

        }
        else
        {
            int lowestRow, lowestCol;
            int horCount, vertCount;
            horCount = CheckHorizontal(i, j, out lowestCol);
            vertCount = CheckVertical(i, j, out lowestRow);

            if (horCount == 4 || vertCount == 4)
            {
                if (horCount == 4)
                {
                    HandleFourMatch(i, j, lowestCol, true);
                }
                else
                {
                    HandleFourMatch(i, j, lowestRow, false);
                }
            }
            else if (horCount == 3 || vertCount == 3)
            {
                if (horCount == 3)
                {
                    HandleThreeMatch(i, j, lowestCol, true);
                }
                else
                {
                    HandleThreeMatch(i, j, lowestRow, false);
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private void HandleRainbowMatch(int i, int j, int si, int sj)
    {
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow)
        {
            // two rainbows collided destroy all
            DestroyAllTiles();
        }
        else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            // one rainbow and one regular
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            int value = board[i, j].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.VerticalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
            int value = board[i, j].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.VerticalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.HorizontalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.HorizontalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.CrossBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.CrossBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
    }

    private void HandleAdjacent(bool horizontal, int i, int j, int numberOfTilesToCheck)
    {
        if (horizontal)
        {
            for (int col = j; col < j + numberOfTilesToCheck; col++)
            {
                if (col == j && col > 0)  // check left
                {
                    if (board[i, col - 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator) {
                        SpawnFromGenerator(i, col - 1, board[i, col - 1].GetComponent<TilePiece>().Value);
                    }
                    if (board[i, col - 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                    {
                        board[i, col - 1].GetComponent<TilePiece>().Crack();
                    }
                    if (board[i, col - 1].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                        && board[i, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                    {
                        virusCount--;
                        virusCure = true;
                        PopOverlay(i, col - 1);
                    }
                }
                else if (col < maxCols - 1 && col == j + numberOfTilesToCheck - 1)  // check right
                { 
                    if (board[i, col + 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                    {
                        SpawnFromGenerator(i, col + 1, board[i, col + 1].GetComponent<TilePiece>().Value);
                    }
                    if (board[i, col + 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                    {
                        board[i, col + 1].GetComponent<TilePiece>().Crack();
                    }
                    if (board[i, col + 1].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                        && board[i, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                    {
                        virusCount--;
                        virusCure = true;
                        PopOverlay(i, col + 1);
                    }
                }
                if (i < maxRows - 1)  // check up
                {
                    if (board[i + 1, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                    {
                        SpawnFromGenerator(i + 1, col, board[i + 1, col].GetComponent<TilePiece>().Value);
                    }
                    if (board[i + 1, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                    {
                        board[i + 1, col].GetComponent<TilePiece>().Crack();
                    }
                    if (board[i + 1, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                        && board[i, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                    {
                        virusCount--;
                        virusCure = true;
                        PopOverlay(i + 1, col);
                    }
                }
                if (i > 0) // check down
                {
                    if (board[i - 1, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                    {
                        SpawnFromGenerator(i - 1, col, board[i - 1, col].GetComponent<TilePiece>().Value);
                    }
                    if (board[i - 1, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                    {
                        board[i - 1, col].GetComponent<TilePiece>().Crack();
                    }
                    if (board[i - 1, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                        && board[i, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                    {
                        virusCount--;
                        virusCure = true;
                        PopOverlay(i - 1, col);
                    }
                }
            }
        } else
        {
            for (int row = i; row < i + numberOfTilesToCheck; row++)
            {
                if (row < maxRows)
                {
                    if (j > 0)  // check left
                    {
                        if (board[row, j - 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                        {
                            SpawnFromGenerator(row, j - 1, board[row, j - 1].GetComponent<TilePiece>().Value);
                        }
                        if (board[row, j - 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                        {
                            board[row, j - 1].GetComponent<TilePiece>().Crack();
                        }
                        if (board[row, j - 1].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                            && board[row, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                        {
                            virusCount--;
                            virusCure = true;
                            PopOverlay(row, j - 1);
                        }
                    }
                    if (j < maxCols - 1)  // check right
                    {
                        if (board[row, j + 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                        {
                            SpawnFromGenerator(row, j + 1, board[row, j + 1].GetComponent<TilePiece>().Value);
                        }
                        if (board[row, j + 1].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                        {
                            board[row, j + 1].GetComponent<TilePiece>().Crack();
                        }
                        if (board[row, j + 1].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                            && board[row, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                        {
                            virusCount--;
                            virusCure = true;
                            PopOverlay(row, j + 1);
                        }
                    }
                    if (i < maxRows - 1 && i == i + numberOfTilesToCheck - 1)  // check up
                    {
                        if (board[row + 1, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                        {
                            SpawnFromGenerator(row + 1, j, board[row + 1, j].GetComponent<TilePiece>().Value);
                        }
                        if (board[row + 1, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                        {
                            board[row + 1, j].GetComponent<TilePiece>().Crack();
                        }
                        if (board[row + 1, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                            && board[row, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                        {
                            virusCount--;
                            virusCure = true;
                            PopOverlay(row + 1, j);
                        }
                    }
                    if (i > 0 && row == i)  // check down
                    {
                        if (board[row - 1, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Generator)
                        {
                            SpawnFromGenerator(row - 1, j, board[row - 1, j].GetComponent<TilePiece>().Value);
                        }
                        if (board[row - 1, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.UnknownCrackable)
                        {
                            board[row - 1, j].GetComponent<TilePiece>().Crack();
                        }
                        if (board[row - 1, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus
                            && board[row, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Enclosure)
                        {
                            virusCount--;
                            virusCure = true;
                            PopOverlay(row - 1, j);
                        }
                    }
                }
            }
        }
    }

    private void SpawnFromGenerator(int i, int j, int value)
    {
        GameObject randomTile;
        GameObject newTile;
        randomTile = ChooseRandomTileExclusive(value);
        if (randomTile)
        {
            newTile = SpawnTileFromLocation(i, j, randomTile.GetComponent<TilePiece>().I, randomTile.GetComponent<TilePiece>().J, value);
            generating++;
            StartCoroutine(MoveToLocationPostGenerate(newTile));
        }
    }

    private void HandleFiveMatch(int i, int j, bool horizontal,int lowest)
    {
        // if a user initiated a switch, the only way the user
        // can achieve five rows is by switching in the middle of 5.
        // however, if it is due to a cascade, the i and j parameters will always be the
        // the lowest row or lowest col.  we need to adjust the i, j to be the middle
        if (horizontal)
        {
            if (j != lowest + 2)
            {
                j = lowest + 2;
                HandleAdjacent(true, i, lowest, 5);
            }
        } else
        {
            if (i != lowest + 2)
            {
                i = lowest + 2;
                HandleAdjacent(false, lowest, j, 5);
            }
        }
        //int value = board[i, j].GetComponent<TilePiece>().Value;
        gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);
        Destroy(board[i, j]);
        SpawnRainbowTile(i, j);
        if (horizontal)
        {
            PopAndMarkDestroy(i, j - 2);
            PopAndMarkDestroy(i, j - 1);
            PopAndMarkDestroy(i, j + 2);
            PopAndMarkDestroy(i, j + 1);
        }
        else  // it's vertical
        {
            PopAndMarkDestroy(i + 2, j);
            PopAndMarkDestroy(i + 1, j);
            PopAndMarkDestroy(i - 2, j);
            PopAndMarkDestroy(i - 1, j);
        }
    }

    private void PopAndMarkDestroy(int i, int j)
    {
        if (board[i, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
        {
            PopOverlay(i, j);
        } else
        {
            MarkDestroy(i, j);
        }
    }

    private void HandleCrossMatch(int i, int j, int lowestRow, int lowestCol)
    {
        int value = board[i, j].GetComponent<TilePiece>().Value;
        gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);
        Destroy(board[i, j]);
        SpawnCrossBlastTile(i, j, value);
        // destroy vertical
        HandleAdjacent(false, lowestRow, j, 3);
        for (int row = lowestRow; row < lowestRow + 3; row++)
        {
            if (row != i)
            {
                if (board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                {
                    PopOverlay(row, j);
                }
                else
                {
                    MarkDestroy(row, j);
                }
            }
        }
        HandleAdjacent(false, i, lowestCol, 3);
        for (int col = lowestCol; col < lowestCol + 3; col++)
        {
            if (col != j)
            {
                if (board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                {
                    PopOverlay(i, col);
                }
                else
                {
                    MarkDestroy(i, col);
                }
            }
        }
    }

    private void HandleFourMatch(int i, int j, int lowest, bool horizontal)
    {
        int value = board[i, j].GetComponent<TilePiece>().Value;

        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);
            Destroy(board[i, j]);
            if (horizontal)
            {
                HandleAdjacent(true, i, lowest, 4);
                SpawnVerticalBlastTile(i, j, value);
                for (int col = lowest; col < lowest + 4; col++)
                {
                    if (col != j)
                    {
                        if (board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                        {
                            PopOverlay(i, col);
                        }
                        else
                        {
                            MarkDestroy(i, col);
                        }
                    }
                }
            }
            else
            {
                HandleAdjacent(false, lowest, j, 4);
                SpawnHorizontalBlastTile(i, j, value);
                for (int row = lowest; row < lowest + 4; row++)
                {
                    if (row != i)
                    {
                        if (board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                        {
                            PopOverlay(row, j);
                        }
                        else
                        {
                            MarkDestroy(row, j);
                        }
                    }
                }
            }
        } else
        {
            if (horizontal)
            {
                HandleAdjacent(true, i, lowest, 4);
                for (int col = lowest; col < lowest + 4; col++)
                {
                    if (board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                    {
                        PopOverlay(i, col);
                    }
                    else
                    {
                        MarkDestroy(i, col);
                    }
                }
            }
            else
            {
                HandleAdjacent(false, lowest, j, 4);
                for (int row = lowest; row < lowest + 4; row++)
                {
                    if (board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                    {
                        PopOverlay(row, j);
                    }
                    else
                    {
                        MarkDestroy(row, j);
                    }
                }
            }
        }
    }

    private void HandleThreeMatch(int i, int j, int lowest, bool horizontal)
    {
        if (horizontal)
        {
            HandleAdjacent(true, i, lowest, 3);
            for (int col = lowest; col < lowest + 3; col++)
            {
                if (board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                {
                    PopOverlay(i, col);
                }
                else
                {
                    MarkDestroy(i, col);
                }
            }
        } else
        {
            HandleAdjacent(false, lowest, j, 3);
            for (int row = lowest; row < lowest + 3; row++)
            {
                if (board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
                {
                    PopOverlay(row, j);
                }
                else
                {
                    MarkDestroy(row, j);
                }
            }
        }
    }

    private void PopOverlay(int row, int col)
    {
        if (board[row, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Enclosure)
        {
            GameObject enc = GameObject.Instantiate(woodEnclosurePrefab, GetScreenCoordinatesWithZ(row, col, -0.1f), Quaternion.identity);
            enc.transform.localScale = localScale;
            for (int i = 0; i < enc.transform.childCount; i++)
            {
                enc.transform.GetChild(i).gameObject.AddComponent<Rigidbody2D>();
                enc.transform.GetChild(i).GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(0.5f, 2f), Random.Range(0.5f, 2f));
            }
            Destroy(enc, 5f);

        }
        board[row, col].GetComponent<TilePiece>().PopOverlay();
        PopOverlayPrefab(row, col);
    }

    private void PopOverlayPrefab(int i, int j)
    {
        if (board[i, j].transform.childCount > 0)
        {
            Destroy(board[i, j].transform.GetChild(board[i, j].transform.childCount - 1).gameObject);
            if (board[i, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
            {
                board[i, j].GetComponent<TilePiece>().Moveable = true;
            }
        }
    }

    private bool CheckCross(int i, int j, out int crossLowestRow, out int crossLowestCol)
    {
        bool found = false;

        crossLowestRow = crossLowestCol = -1;
        if (CheckHorizontal(i, j, out crossLowestCol) >= 3 && CheckVertical(i, j, out crossLowestRow) >= 3)
        {
            found = true;
        }
        return found;
    }

    private bool HandledCascadeCross(int i, int j)
    {
        bool found = false;
        if (board[i, j].GetComponent<TilePiece>().Moveable && !board[i, j].GetComponent<TilePiece>().Destroyed)
        {
            if (j < maxCols - 2)
            {
                if (i < maxRows - 2
                    && GetValueIJ(i, j) == GetValueIJ(i, j + 1)
                    && GetValueIJ(i, j) == GetValueIJ(i, j + 2)
                    && !board[i, j + 1].GetComponent<TilePiece>().Destroyed
                    && !board[i, j + 1].GetComponent<TilePiece>().Indestructable)
                {
                    int value = GetValueIJ(i, j);
                    // potential for cross match with horiztonal base
                    for (int col = j; col < j + 3 && !found; col++)
                    {
                        if (GetValueIJ(i, j) == GetValueIJ(i + 1, col)
                            && GetValueIJ(i, j) == GetValueIJ(i + 2, col)
                            && !board[i + 1, j].GetComponent<TilePiece>().Destroyed
                            && !board[i + 2, j].GetComponent<TilePiece>().Destroyed
                            && !board[i + 1, j].GetComponent<TilePiece>().Indestructable
                            && !board[i + 1, j].GetComponent<TilePiece>().Indestructable)
                        {
                            found = true;
                            MarkDestroy(i, j);
                            MarkDestroy(i, j + 1);
                            MarkDestroy(i, j + 2);
                            MarkDestroy(i + 1, col);
                            MarkDestroy(i + 2, col);
                            Destroy(board[i, col]);
                            SpawnCrossBlastTile(i, col, value);
                        }
                    }
                }
            }
            if (i < maxRows - 2)
            {
                if (GetValueIJ(i, j) == GetValueIJ(i + 1, j)
                    && GetValueIJ(i, j) == GetValueIJ(i + 2, j)
                    && !board[i + 1, j].GetComponent<TilePiece>().Destroyed
                    && !board[i + 2, j].GetComponent<TilePiece>().Destroyed
                    && !board[i + 1, j].GetComponent<TilePiece>().Indestructable
                    && !board[i + 2, j].GetComponent<TilePiece>().Indestructable)
                {
                    int value = GetValueIJ(i, j);
                    for (int row = i + 1; row < i + 3 && !found; row++)
                    {
                        if (j > 1 && GetValueIJ(i, j) == GetValueIJ(row, j - 1)
                            && GetValueIJ(i, j) == GetValueIJ(row, j - 2)
                            && !board[row, j - 1].GetComponent<TilePiece>().Destroyed
                            && !board[row, j - 2].GetComponent<TilePiece>().Destroyed
                            && !board[row, j - 1].GetComponent<TilePiece>().Indestructable
                            && !board[row, j - 2].GetComponent<TilePiece>().Indestructable)
                        {
                            found = true;
                            MarkDestroy(i, j);
                            MarkDestroy(i + 1, j);
                            MarkDestroy(i + 2, j);
                            MarkDestroy(row, j - 1);
                            MarkDestroy(row, j - 2);
                            Destroy(board[row, j]);
                            SpawnCrossBlastTile(row, j, value);
                        }
                        else if (j < maxCols - 2 
                            && GetValueIJ(i, j) == GetValueIJ(row, j + 1)
                            && GetValueIJ(i, j) == GetValueIJ(row, j + 2)
                            && !board[row, j + 1].GetComponent<TilePiece>().Destroyed
                            && !board[row, j + 2].GetComponent<TilePiece>().Destroyed
                            && !board[row, j + 1].GetComponent<TilePiece>().Indestructable
                            && !board[row, j + 2].GetComponent<TilePiece>().Indestructable)
                        {
                            found = true;
                            MarkDestroy(i, j);
                            MarkDestroy(i + 1, j);
                            MarkDestroy(i + 2, j);
                            MarkDestroy(row, j + 1);
                            MarkDestroy(row, j + 2);
                            Destroy(board[row, j]);
                            SpawnCrossBlastTile(row, j, value);
                        }
                        else if (j > 0
                          && j < maxCols - 1
                          && GetValueIJ(i, j) == GetValueIJ(row, j - 1)
                          && GetValueIJ(i, j) == GetValueIJ(row, j + 1)
                          && !board[row, j - 1].GetComponent<TilePiece>().Destroyed
                          && !board[row, j + 1].GetComponent<TilePiece>().Destroyed
                          && !board[row, j - 1].GetComponent<TilePiece>().Indestructable
                          && !board[row, j + 1].GetComponent<TilePiece>().Indestructable)
                        {
                            MarkDestroy(i, j);
                            MarkDestroy(i + 1, j);
                            MarkDestroy(i + 2, j);
                            MarkDestroy(row, j - 1);
                            MarkDestroy(row, j + 1);
                            Destroy(board[row, j]);
                            SpawnCrossBlastTile(row, j, value);
                            found = true;
                        }
                    }
                }
            }
        }
        return found;
    }   

    private int CheckHorizontal(int i, int j, out int lowestCol)
    {
        int count = 1;
        lowestCol = -1;

        if (!board[i, j].GetComponent<TilePiece>().Destroyed && board[i, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
        {
            count += CheckLeft(i, j);
            lowestCol = j - count + 1;
            count += CheckRight(i, j);
        }
        return count;
    }

    private int CheckLeft(int i, int j)
    {
        int count = 0;
        while (j > 0 
            && board[i, j - 1].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value 
            && !board[i, j - 1].GetComponent<TilePiece>().Destroyed
            && !board[i, j - 1].GetComponent<TilePiece>().Indestructable
            && board[i, j - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
        {
            count++;
            j--;
        }
        return count;
    }

    private int CheckRight(int i, int j)
    {
        int count = 0;
        while (j < maxCols - 1
            && board[i, j + 1].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value
            && !board[i, j].GetComponent<TilePiece>().Destroyed
            && !board[i, j + 1].GetComponent<TilePiece>().Indestructable
            && board[i, j + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
        {
            count++;
            j++;
        }
        return count;
    }

    private int CheckVertical(int i, int j, out int lowestRow)
    { 
        int count = 1;
        lowestRow = -1;

        if (!board[i, j].GetComponent<TilePiece>().Destroyed && board[i, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus && !board[i, j].GetComponent<TilePiece>().Indestructable)
        {
            count += CheckDown(i, j);
            lowestRow = i - count + 1;
            count += CheckUp(i, j);
        }
        return count;
    }

    private int CheckUp(int i, int j)
    {
        int count = 0;
        while (i < maxRows - 1
            && board[i + 1, j].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value 
            && !board[i + 1, j].GetComponent<TilePiece>().Destroyed
            && !board[i + 1, j].GetComponent<TilePiece>().Indestructable
            && board[i + 1, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
        {
            count++;
            i++;
        }
        return count;
    }

    private int CheckDown(int i, int j)
    {
        int count = 0;
        while (i > 0 
            && board[i - 1, j].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value 
            && !board[i - 1, j].GetComponent<TilePiece>().Destroyed
            && !board[i - 1, j].GetComponent<TilePiece>().Indestructable
            && board[i - 1, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
        {
            count++;
            i--;
        }
        return count;
    }

    // algorithm:  check if the tile moves left, right, up or down creates a match in either the horizontal or vertical direction
    public bool GetHint()
    {
        bool found = false;
        bool rainbowFound = false;
        int i, j;
        i = j = -1;
        int rainbowMatchI, rainbowMatchJ;
        rainbowMatchI = rainbowMatchJ = -1;
    
        for (int row = 0; row < maxRows && !found; row++)
        {
            for (int col = 0; col < maxCols && !found; col++)
            {
                // look for rainbow tiles.  if one exists, it is always eligible to move if adjacent to a destructable tile
                if (!rainbowFound && board[row, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow)
                {
                    // check left
                    if (col > 0 && board[row, col - 1].GetComponent<TilePiece>().Moveable && !board[row, col - 1].GetComponent<TilePiece>().Destroyed)
                    {
                        rainbowFound = true;
                    }
                    // check right
                    if (!rainbowFound && col < maxCols - 1 && board[row, col + 1].GetComponent<TilePiece>().Moveable && !board[row, col + 1].GetComponent<TilePiece>().Destroyed)
                    {
                        rainbowFound = true;
                    }
                    // check up
                    if (!rainbowFound && row < maxRows - 1 && board[row + 1, col].GetComponent<TilePiece>().Moveable && !board[row + 1, col].GetComponent<TilePiece>().Destroyed)
                    {
                        rainbowFound = true;
                    }
                    if (!rainbowFound && row > 0 && board[row - 1, col].GetComponent<TilePiece>().Moveable && !board[row - 1, col].GetComponent<TilePiece>().Destroyed)
                    {
                        rainbowFound = true;
                    }
                    if (rainbowFound)
                    {
                        rainbowMatchI = row;
                        rainbowMatchJ = col;
                    }
                }
                int value = board[row, col].GetComponent<TilePiece>().Value;
                if (board[row, col].GetComponent<TilePiece>().Moveable && !board[row, col].GetComponent<TilePiece>().Destroyed && !board[row, col].GetComponent<TilePiece>().Indestructable)
                {
                    // check left
                    if (col > 0  && board[row, col - 1].GetComponent<TilePiece>().Moveable && board[row, col - 1].GetComponent<TilePiece>().Destroyed)
                    {
                        if (col > 2)
                        {
                            // check if possible horizontal match
                            if (board[row, col - 3].GetComponent<TilePiece>().Value == board[row, col - 2].GetComponent<TilePiece>().Value
                                && board[row, col - 2].GetComponent<TilePiece>().Value == value
                                && !board[row, col - 3].GetComponent<TilePiece>().Destroyed
                                && !board[row, col - 2].GetComponent<TilePiece>().Destroyed
                                && board[row, col - 3].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                && board[row, col - 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            {
                                // check if possible vertical match
                                if (row > 0 && row < maxRows - 1)
                                {
                                    if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row - 1, col - 1].GetComponent<TilePiece>().Value 
                                        && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value
                                        && !board[row + 1, col - 1].GetComponent<TilePiece>().Destroyed
                                        && !board[row - 1, col - 1].GetComponent<TilePiece>().Destroyed
                                        && board[row - 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                        && board[row + 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                    {
                                        found = true;
                                    }
                                }
                                if (!found && row > 1)
                                {
                                    if (board[row - 1, col - 1].GetComponent<TilePiece>().Value == board[row - 2, col - 1].GetComponent<TilePiece>().Value 
                                        && board[row - 1, col - 1].GetComponent<TilePiece>().Value == value
                                        && !board[row - 1, col - 1].GetComponent<TilePiece>().Destroyed
                                        && !board[row - 2, col - 1].GetComponent<TilePiece>().Destroyed
                                        && board[row - 2, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                        && board[row - 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                    {
                                        found = true;
                                    }
                                }
                                if (!found && row < maxRows - 2)
                                {
                                    if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 2, col - 1].GetComponent<TilePiece>().Value 
                                        && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value
                                        && !board[row + 1, col - 1].GetComponent<TilePiece>().Destroyed
                                        && !board[row + 2, col - 1].GetComponent<TilePiece>().Destroyed
                                        && board[row + 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                        && board[row + 2, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                    {
                                        found = true;
                                    }
                                }
                            }
                        }
                    }
                    // check right
                    if (!found 
                        && col < maxCols - 1 
                        && board[row, col + 1].GetComponent<TilePiece>().Moveable 
                        && !board[row, col + 1].GetComponent<TilePiece>().Destroyed)
                    {
                        if (col < maxCols - 3)
                        {
                            // check horizontal match
                            if (board[row, col + 2].GetComponent<TilePiece>().Value == board[row, col + 3].GetComponent<TilePiece>().Value 
                                && board[row, col + 3].GetComponent<TilePiece>().Value == value
                                && !board[row, col + 2].GetComponent<TilePiece>().Destroyed
                                && !board[row, col + 3].GetComponent<TilePiece>().Destroyed
                                && board[row, col + 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                && board[row, col + 3].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check if possible vertical match
                            if (row > 0 && row < maxRows - 1)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row - 1, col + 1].GetComponent<TilePiece>().Value 
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value
                                    && !board[row + 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row - 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                            if (!found && row > 1)
                            {
                                if (board[row - 1, col + 1].GetComponent<TilePiece>().Value == board[row - 2, col + 1].GetComponent<TilePiece>().Value 
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().Value == value
                                    && !board[row - 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row - 2, col + 1].GetComponent<TilePiece>().Destroyed
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row - 2, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                            if (!found && row < maxRows - 2)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row + 2, col + 1].GetComponent<TilePiece>().Value 
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value
                                    && !board[row + 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row + 2, col + 1].GetComponent<TilePiece>().Destroyed
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row + 2, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    // check up
                    if (!found 
                        && row < maxRows - 1
                        && board[row + 1, col].GetComponent<TilePiece>().Moveable 
                        && !board[row + 1, col].GetComponent<TilePiece>().Destroyed)
                    {
                        if (row < maxRows - 3)
                        {
                            // check vertical match
                            if (board[row + 2, col].GetComponent<TilePiece>().Value == board[row + 3, col].GetComponent<TilePiece>().Value 
                                && board[row + 2, col].GetComponent<TilePiece>().Value == value
                                && !board[row + 2, col].GetComponent<TilePiece>().Destroyed
                                && !board[row + 3, col].GetComponent<TilePiece>().Destroyed
                                && board[row + 2, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                && board[row + 3, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check for horizontal match
                            if (col > 0 && col < maxCols - 1)
                            {
                                if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 1, col + 1].GetComponent<TilePiece>().Value 
                                    && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value
                                    && !board[row + 1, col - 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row + 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && board[row + 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col > 1)
                            {
                                if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 1, col - 2].GetComponent<TilePiece>().Value 
                                    && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value
                                    && !board[row + 1, col - 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row + 1, col - 2].GetComponent<TilePiece>().Destroyed
                                    && board[row + 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row + 1, col - 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col < maxCols - 2)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row + 1, col + 2].GetComponent<TilePiece>().Value
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value
                                    && !board[row + 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row + 1, col + 2].GetComponent<TilePiece>().Destroyed
                                    && board[row + 1, col + 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row + 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    // check down
                    if (!found 
                        && row > 0
                        && board[row - 1, col].GetComponent<TilePiece>().Moveable
                        && !board[row - 1, col].GetComponent<TilePiece>().Destroyed)
                    {
                        if (row > 2)
                        {
                            // checked for vertical match
                            if (board[row - 2, col].GetComponent<TilePiece>().Value == board[row - 3, col].GetComponent<TilePiece>().Value 
                                && board[row - 2, col].GetComponent<TilePiece>().Value == value
                                && !board[row - 2, col].GetComponent<TilePiece>().Destroyed
                                && !board[row - 3, col].GetComponent<TilePiece>().Destroyed
                                && board[row - 2, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                && board[row - 3, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check for horizontal match
                            if (col > 0 && col < maxCols - 1)
                            {
                                if (board[row - 1, col - 1].GetComponent<TilePiece>().Value == board[row - 1, col + 1].GetComponent<TilePiece>().Value
                                    && board[row - 1, col - 1].GetComponent<TilePiece>().Value == value
                                    && !board[row - 1, col - 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row - 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row - 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col > 2)
                            {
                                if (board[row - 1, col - 2].GetComponent<TilePiece>().Value == board[row - 1, col - 1].GetComponent<TilePiece>().Value
                                    && board[row - 1, col - 2].GetComponent<TilePiece>().Value == value
                                    && !board[row - 1, col - 2].GetComponent<TilePiece>().Destroyed
                                    && !board[row - 1, col - 1].GetComponent<TilePiece>().Destroyed
                                    && board[row - 1, col - 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row - 1, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {

                                    found = true;
                                }
                            }
                            if (!found && col < maxCols - 2)
                            {
                                if (board[row - 1, col + 1].GetComponent<TilePiece>().Value == board[row - 1, col + 2].GetComponent<TilePiece>().Value
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().Value == value
                                    && !board[row - 1, col + 1].GetComponent<TilePiece>().Destroyed
                                    && !board[row - 1, col + 2].GetComponent<TilePiece>().Destroyed
                                    && board[row - 1, col + 2].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
                                    && board[row - 1, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    if (found)
                    {
                        i = row;
                        j = col;
                    }
                }
            }
        }
        hintI = i;
        hintJ = j;
        if (hintI == -1 || hintJ == 1)
        {
            if (rainbowFound)
            {
                hintI = rainbowMatchI;
                hintJ = rainbowMatchJ;
                found = true;
            }
        }
        return found;
    }

    public void ReshuffleDrawn()
    {
        List<GameObject> go = new List<GameObject>();
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                if (board[i,j].GetComponent<TilePiece>().Moveable && !board[i, j].GetComponent<TilePiece>().Destroyed)
                {
                    go.Add(board[i, j]);
                }
            }
        }
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j< maxCols; j++)
            {
                if (board[i, j].GetComponent<TilePiece>().Moveable && !board[i, j].GetComponent<TilePiece>().Destroyed)
                {
                    int rand = Random.Range(0, go.Count);
                    board[i, j] = go[rand];
                    go.RemoveAt(rand);
                    board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                    reshufflingCount++;
                    StartCoroutine(MoveToLocation(board[i, j]));
                }
            }
        }
        StartCoroutine(PostReshuffleCascade());
    }

    IEnumerator PostReshuffleCascade()
    {
        while (reshufflingCount > 0)
        {
            yield return null;
        }
        Cascade(true);
    }

    IEnumerator MoveToLocationPostGenerate(GameObject go) 
    {
        float startTime = Time.time;
        float lerpTime = 1.0f;
        float currentLerpTime = 0.0f;
        int i, j;
        i = go.GetComponent<TilePiece>().I;
        j = go.GetComponent<TilePiece>().J;     
        while (Time.time - startTime < lerpTime)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }
            float perc = currentLerpTime / lerpTime;
            Vector3 newPosition = GetScreenCoordinates(i, j);
            go.transform.position = Vector3.Lerp(go.transform.position, newPosition, perc);
            //Debug.Log(go.transform.position);
            yield return null;
        }
        Destroy(board[i, j]);
        board[i, j] = go;
        generating--;
    }

    IEnumerator MoveToLocation(GameObject go)
    {
        float startTime = Time.time;
        int i, j;
        i = go.GetComponent<TilePiece>().I;
        j = go.GetComponent<TilePiece>().J;
        while (Time.time - startTime < 2.0f)
        {
            Vector3 newPosition = GetScreenCoordinates(i, j);
            board[i, j].transform.position = Vector3.Lerp(board[i, j].transform.position, newPosition, Time.time - startTime);
            yield return null;
        }
        reshufflingCount--;
    }

    public void Cascade(bool checkWin)
    {
        StartCoroutine(DoCascade(checkWin));
    }

    IEnumerator DoCascade(bool checkWin)
    {
        // wait for pieces to stop fallling
        while (falling > 0 || generating > 0)
        {
            yield return null;
        }
        //yield return new WaitForSeconds(0.5f);
        bool match = false;
        int lowest = -1;

        // first scan for 5 matches across entire board
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                if (CheckHorizontal(i, j, out lowest) >= 5)
                {
                    HandleFiveMatch(i, j, true, lowest);
                    match = true;
                } else if (CheckVertical(i, j, out lowest) >= 5)
                {
                    HandleFiveMatch(i, j, false, lowest);
                    match = true;
                }
            }
        }        
        // now check for the rest
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                match |= CheckIJCascade(i, j);
            }
        }
        if (match)
        {
            cascadeCount++;
            scoreMultiplier = cascadeCount + 1;
            FinalizeUnmoveable2(checkWin);
        }
        else
        {

            bool found = false;
            if (gameData.levelData[level].mission.type == 2)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (board[0, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.DropCount)
                    {
                        board[0, col].GetComponent<SpriteRenderer>().enabled = false;
                        board[0, col].GetComponent<TilePiece>().Destroyed = true;
                        found = true;
                        dropCount++;
                        gameController.UpdateDropCount(dropCount);
                    }
                }
                if (found)
                {
                    FinalizeUnmoveable2(checkWin);
                }
            }
            if (checkWin)
            {
                bool won = HasWon();
                bool lost = CheckLoseCondition();

                if (!won)
                {
                    if (!lost)
                    {
                        if (!found)
                        {
                            PlayCascadeSounds();
                        }
                        if (!virusCure && virusCount > 0)
                        {
                            SpreadVirus();
                        }
                        if (virusCure && virusCount > 0)
                        {
                            virusCure = false;
                        }
                        EnableHintStart();
                    }
                    else
                    {
                        winLocked = true;
                        LevelLose();
                    }
                }
                else
                {
                    winLocked = true;
                    LevelWin();
                }
            }
        }
    }

    private void SpreadVirus()
    {
        // gather all virii
        List<Virus> virusSpreadList = new List<Virus>();
        List<string> directions;
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (board[row, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.Virus)
                {
                    directions = CanSpreadVirus(row, col);
                    if (directions.Count > 0)
                    {
                        Virus virus = new Virus();
                        virus.row = row;
                        virus.col = col;
                        virus.directions = directions;
                        virusSpreadList.Add(virus);
                    }
                }
            }
        }

        // randomly pick a virus that can spread
        if (virusSpreadList.Count > 0)
        {
            Virus chosen = virusSpreadList[Random.Range(0, virusSpreadList.Count)];
            int row = chosen.row;
            int col = chosen.col;
            string dir = chosen.directions[Random.Range(0, chosen.directions.Count)];
            switch (dir)
            {
                case "up": row++; break;
                case "down": row--; break;
                case "left": col--; break;
                case "right": col++; break;
                default: break;
            }
            board[row, col].GetComponent<TilePiece>().AddOverlay(TilePiece._OverlayType.Virus);
            AddOverlay(row, col, TilePiece._OverlayType.Virus);
        }                
    }
    

    // return a bitwise value of available directions
    private List<string> CanSpreadVirus(int row, int col)
    {
        List<string> canSpread = new List<string>();

        // check left
        if (col > 0 && board[row, col - 1].GetComponent<TilePiece>().OriginalMoveable
            && board[row, col - 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
            && !board[row, col - 1].GetComponent<TilePiece>().Destroyed)
        {
            canSpread.Add("left");
        } 
        // check right
        if (col < maxCols - 1 && board[row, col + 1].GetComponent<TilePiece>().OriginalMoveable
            && board[row, col + 1].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
            && !board[row, col + 1].GetComponent<TilePiece>().Destroyed)
        {
            canSpread.Add("right");
        }
        // check down
        if (row > 0 && board[row - 1, col].GetComponent<TilePiece>().OriginalMoveable
            && board[row - 1, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
            && !board[row - 1, col].GetComponent<TilePiece>().Destroyed)
        {
            canSpread.Add("down");
        }
        // check up
        if (row < maxRows - 1 && board[row + 1, col].GetComponent<TilePiece>().OriginalMoveable
            && board[row + 1, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.Virus
            && !board[row + 1, col].GetComponent<TilePiece>().Destroyed)
        {
            canSpread.Add("up");
        }
        return canSpread;
    }

    public void SwitchPositions(int ai, int aj, int bi, int bj)
    {
        locked = true;
        Vector3 newPositionB = GetScreenCoordinates(ai, aj);
        Vector3 newPositionA = GetScreenCoordinates(bi, bj);
        //Vector3 newPositionB = new Vector3(aj * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f, ai * tileSize.y + tileSize.y / 2.0f - boardSize.y / 2.0f + 1.0f, 0.0f);
        //Vector3 newPositionA = new Vector3(bj * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f, bi * tileSize.y + tileSize.y / 2.0f - boardSize.y / 2.0f + 1.0f, 0.0f);

        StartCoroutine(SwitchPositionsLerp(ai, aj, bi, bj, newPositionA, newPositionB));
    }

    IEnumerator SwitchPositionsLerp(int ai, int aj, int bi, int bj, Vector3 newPositionA, Vector3 newPositionB)
    {
        float startTime = Time.time;
        GameObject temp;        
        while (Time.time - startTime <= 0.5f)
        {
            board[bi, bj].transform.position = Vector3.Lerp(board[bi, bj].transform.position, newPositionB, Time.time - startTime);
            board[ai, aj].transform.position = Vector3.Lerp(board[ai, aj].transform.position, newPositionA, Time.time - startTime);
            yield return null;
        }

        board[bi, bj].GetComponent<TilePiece>().SetLocation(ai, aj);
        board[ai, aj].GetComponent<TilePiece>().TargetI = ai;
        board[ai, aj].GetComponent<TilePiece>().TargetJ = aj;
        board[ai, aj].GetComponent<TilePiece>().SetLocation(bi, bj);
        temp = board[ai, aj];
        board[ai, aj] = board[bi, bj];
        board[bi, bj] = temp;
        locked = false;
    }

    private void DestroyAllTiles()
    {
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (!board[row, col].GetComponent<TilePiece>().Indestructable)
                {
                    //gameController.AddTileCount(level, board[row, col].GetComponent<TilePiece>().OriginalTileType, board[row, col].GetComponent<TilePiece>().OriginalValue);
                    //board[row, col].GetComponent<TilePiece>().Destroyed = true;
                    if (board[row, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.None)
                    {
                        PopOverlay(row, col);
                    }
                    else
                    {
                        MarkDestroy(row, col);
                    }
                }
            }
        }
    }

    private void DestroyAllTiles(int value)
    {
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (board[row, col].GetComponent<TilePiece>().Value == value)
                {
                    if (board[row, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.None)
                    {
                        PopOverlay(row, col);
                    }
                    else
                    {
                        MarkDestroy(row, col);
                    }
                }
            }
        }
    }

    private void ConvertTiles(int value, TilePiece._TileType tileType)
    {
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (board[row, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
                {
                    board[row, col].GetComponent<TilePiece>().TileType = tileType;
                }
            }
        }
    }

    private void MarkDestroy(int i, int j)
    {
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
        {
            //Debug.Log("horizontal");
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            for (int col = 0; col < maxCols; col++)
            {
                // turn all other horizontal blasts in same row into normal tile pieces to prevent infinite recursion
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast
                    && board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                    board[i, col].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.HorizontalBlast;
                }
                // turn all cross blasts in same row into vertical blast since this horizontal blast is already triggering a row destruction
                // remember cross blasts destroy column AND row, need to avoid destroying the same row twice thus 
                // preventing an infinte recursion
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast
                    && board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
                    board[i, col].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.CrossBlast;
                }
            }
            DestroyRow(i);
        } else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
            //Debug.Log("VERtical");

            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            // turn all other vertical blast in same column into normal tile pieces
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast
                    && board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast
                    && board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
                }
            }
            DestroyCol(j);
        } else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
        {
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast
                    && board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast
                    && board[row, j].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
                }
            }
            for (int col = 0; col < maxCols; col++)
            {
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast
                    && board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast
                    && board[i, col].GetComponent<TilePiece>().OverlayType == TilePiece._OverlayType.None)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
                }
            }
            DestroyRow(i);
            DestroyCol(j);
        }
        else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow)
        {
            DestroyAllTiles(Random.Range(0, tiles.Count + 1));
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            board[i, j].GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            if (!board[i, j].GetComponent<TilePiece>().Destroyed && !board[i, j].GetComponent<TilePiece>().Indestructable)
            {
                int scoreAmt = 0;
                GameObject floatingScoreGO;
                gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);

                board[i, j].GetComponent<TilePiece>().Destroyed = true;
                switch (destroyEffect)
                {
                    case "explode":
                        GameObject explosion = GameObject.Instantiate(TileExplosion, GetScreenCoordinates(i, j), Quaternion.identity, transform);
                        Destroy(explosion, 2.0f);
                        break;
                    case "fall":
                        GameObject temp = GameObject.Instantiate(tiles[board[i, j].GetComponent<TilePiece>().Value], GetScreenCoordinates(i, j), Quaternion.identity);
                        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                        temp.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                        Destroy(temp, 3f);
                        break;
                    default: break;
                }
                gameController.PlayTileDestroySound();
                switch (board[i, j].GetComponent<TilePiece>().OriginalTileType)
                {
                    case TilePiece._TileType.Regular: scoreAmt = this.scoreAmt; break;
                    case TilePiece._TileType.HorizontalBlast: scoreAmt = this.horizontalBlastScoreAmt; break;
                    case TilePiece._TileType.VerticalBlast: scoreAmt = this.verticalBlastScoreAmt;break;
                    case TilePiece._TileType.CrossBlast: scoreAmt = this.crossBlastScoreAmt;break;
                    case TilePiece._TileType.Rainbow: scoreAmt = this.rainbowScoreAmt; break;
                    default: scoreAmt = this.scoreAmt; break;
                }
                floatingScoreGO = GameObject.Instantiate(floatingScore, GetScreenCoordinates(i, j), Quaternion.identity);
                floatingScoreGO.GetComponent<TextMesh>().text = (scoreAmt * scoreMultiplier).ToString();
                IncrementScore(scoreAmt);
                Destroy(floatingScoreGO, floatingScoreLifeTimeSeconds);
            }
        }
    }

    private void DestroyRow(int row)
    {
        for (int j = 0; j < maxCols; j++)
        {
            if (!board[row, j].GetComponent<TilePiece>().Indestructable)
            {
                if (board[row, j].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.None)
                {
                    PopOverlay(row, j);
                }
                else
                {
                    MarkDestroy(row, j);
                }
            }
        }
    }

    private void DestroyCol(int col)
    {
        for (int i = 0; i< maxRows; i++)
        {
            if (!board[i, col].GetComponent<TilePiece>().Indestructable)
            {
                if (board[i, col].GetComponent<TilePiece>().OverlayType != TilePiece._OverlayType.None)
                {
                    PopOverlay(i, col);
                }
                else
                {
                    MarkDestroy(i, col);
                }
            }
        }
    }

    private void FinalizeUnmoveable2(bool checkWin)
    {
		didFill = false;  // global
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                FillFromAbove2(row, col, row, col);
            }
        }
        StartCoroutine(FillCorner());
        StartCoroutine(FillFromAboveC());
        Cascade(checkWin);

    }

    private void PlayCascadeSounds()
    {
        if (cascadeCount == 2)
        {
            gameController.PlayWooHooSound();
        }
        else if (cascadeCount > 2)
        {
            gameController.PlayGreatSound();
        }

    }

    private void FinalStop(bool checkWin)
    {
        if (CheckLoseCondition())
        {
            gameController.PlayLoseSound();
            ShowLoseScreen();
        }
    }

    IEnumerator FillFromAboveC()
    {
        while (falling > 0 || generating > 0)
        {
            yield return null;
        }
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                FillFromAbove2(row, col, row, col);
            }
        }
    }

    private GameObject FillFromAbove2(int row, int col, int fromRow, int fromCol)
    {
        if (board[row, col].GetComponent<TilePiece>().Destroyed)
        {
            TileBackgroundController tbc = backgroundBoard[row, col].GetComponent<TileBackgroundController>();
            board[row, col].GetComponent<SpriteRenderer>().enabled = false;
            
            if (row == maxRows - 1 
                && (!tbc.IsTele 
                    //&& tbc.TeleDirection != TileBackgroundController._teleDirection.In
                    )
                )
            {
                Destroy(board[fromRow, fromCol]);
                SpawnTile(fromRow, fromCol);
                falling++;
                StartCoroutine(Fall(fromRow, fromCol, fallLerpTime));
                didFill = true;
            }
            else if
                (
                    backgroundBoard[row, col] != null 
                    && backgroundBoard[row, col].GetComponent<TileBackgroundController>().IsTele 
                    && backgroundBoard[row, col].GetComponent<TileBackgroundController>().TeleDirection == TileBackgroundController._teleDirection.In
                )
            {
                board[fromRow, fromCol] = CloneAndSpawn(tbc.TeleI, tbc.TeleJ, fromRow, fromCol, row + 1, col);
                board[fromRow, fromCol].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                falling++;
                StartCoroutine(Fall(fromRow, fromCol, fallLerpTime));
                didFill = true;
                StartCoroutine(TeleFall(tbc.TeleI, tbc.TeleJ, fallLerpTime));
                board[tbc.TeleI, tbc.TeleJ].GetComponent<SpriteRenderer>().enabled = false;
                board[tbc.TeleI, tbc.TeleJ].GetComponent<TilePiece>().Destroyed = true;
                int i = tbc.TeleI;
                while (i < maxRows)
                {
                    FillFromAbove2(i, tbc.TeleJ, i, tbc.TeleJ);
                    i++;
                }

            }
            else
            {
                GameObject go = null;
                go = FillFromAbove2(row + 1, col, fromRow, fromCol);
                if (go)
                {
                    Destroy(board[fromRow, fromCol]);
                    board[fromRow, fromCol] = CloneAndSpawn(row + 1, col, fromRow, fromCol, row + 1, col);
                    go.GetComponent<SpriteRenderer>().enabled = false;
                    go.GetComponent<TilePiece>().Destroyed = true;
                    falling++;
                    StartCoroutine(Fall(fromRow, fromCol, fallLerpTime));
                    didFill = true;
                }
            }
        }
        else if (board[row, col].GetComponent<TilePiece>().Moveable && !board[row, col].GetComponent<TilePiece>().Destroyed)
        { 
            return board[row, col];
        }
        return null;
    }

    IEnumerator FillCorner()
    {
        while (falling > 0 || generating > 0)
        {
            yield return null;
        }

        do
        {
            didCornerFill = false;
            for (int j = 0; j < maxCols; j++)
            {
                for (int i = 1; i < maxRows; i++)
                {
                    //while(falling > 0)
                    //{
                    //    yield return null;
                    //}
                    DoFillCorner(i, j);
                    //while (toDestroy.Count > 0)
                    //{
                    //    Destroy(toDestroy[toDestroy.Count - 1]);
                    //    toDestroy.Remove(toDestroy[toDestroy.Count - 1]);
                    //}
                }
            }
        } while (didCornerFill);
    }

    private void DoFillCorner(int row, int col)
    {
        if (row == 0)
        {
            return;
        }
        if (board[row, col].GetComponent<TilePiece>().Moveable && !board[row, col].GetComponent<TilePiece>().Destroyed)
        {
            if (board[row - 1, col].GetComponent<TilePiece>().Destroyed)
            {
                Destroy(board[row - 1, col]);
                board[row - 1, col] = CloneAndSpawn(row, col, row - 1, col, row, col);
                board[row, col].GetComponent<TilePiece>().Destroyed = true;
                board[row, col].GetComponent<SpriteRenderer>().enabled = false;
                toDestroy.Add(board[row, col]);
                falling++;
                StartCoroutine(Fall(row - 1, col, fallLerpTime));
                didCornerFill = true;
                DoFillCorner(row - 1, col);
                
            }
            else if (col > 0 && board[row - 1, col - 1].GetComponent<TilePiece>().Destroyed)
            {
                Destroy(board[row - 1, col - 1]);
                board[row - 1, col - 1] = CloneAndSpawn(row, col, row - 1, col - 1, row, col);
                board[row, col].GetComponent<TilePiece>().Destroyed = true;
                board[row, col].GetComponent<SpriteRenderer>().enabled = false;
                toDestroy.Add(board[row, col]);
                falling++;
                StartCoroutine(Fall(row - 1, col - 1, fallLerpTime));
                didCornerFill = true;
                DoFillCorner(row - 1, col - 1);
            }
            else if (col < maxCols - 1 && board[row - 1, col + 1].GetComponent<TilePiece>().Destroyed)
            {
                Destroy(board[row - 1, col + 1]);
                board[row - 1, col + 1] = CloneAndSpawn(row, col, row - 1, col + 1, row, col);
                board[row, col].GetComponent<TilePiece>().Destroyed = true;
                board[row, col].GetComponent<SpriteRenderer>().enabled = false;
                toDestroy.Add(board[row, col]);
                falling++;
                StartCoroutine(Fall(row - 1, col + 1, fallLerpTime));
                didCornerFill = true;
                DoFillCorner(row - 1, col + 1);
            }
            else
            {
                return;
            }
        }
        return;
    }

    public void Fill()
    { 
        // destroy and spawn tiles
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, col].GetComponent<TilePiece>().Destroyed && !board[row, col].GetComponent<TilePiece>().DelayFill)
                {
                    IncrementScore(scoreAmt);
                    Destroy(board[row, col]);
                    SpawnTile(row, col);
                    falling++;
                    StartCoroutine(Fall(row, col, fallLerpTime));
                }
            }
        }
    }

    private LeftRightNeither ChooseLeftRightNeither(int i, int j) {
        LeftRightNeither leftRightNeither = LeftRightNeither.Neither;

        //Debug.Log("LeftrightNeith input " + i + "," + j);
        if (j == 0)
        {
            if (board[i, j + 1].GetComponent<TilePiece>().Moveable)
            {
                leftRightNeither = LeftRightNeither.Right;
            }
        }
        else if (j == maxCols - 1)
        {
            if (board[i, j - 1].GetComponent<TilePiece>().Moveable)
            {
                leftRightNeither = LeftRightNeither.Left;
            }
        }
        else
        {
            if (board[i, j - 1].GetComponent<TilePiece>().Moveable && !board[i, j + 1].GetComponent<TilePiece>().Moveable)
            {
                leftRightNeither = LeftRightNeither.Left;
            }
			else
            if (!board[i, j - 1].GetComponent<TilePiece>().Moveable && board[i, j + 1].GetComponent<TilePiece>().Moveable)
            {
                leftRightNeither = LeftRightNeither.Right;
            }
			else 
			if (board[i, j - 1].GetComponent<TilePiece>().Moveable
						&& board[i, j + 1].GetComponent<TilePiece>().Moveable)
			{
                int left = Mathf.FloorToInt(Random.Range(0, 1.99f));
                if (left == 0)
                {
                    leftRightNeither = LeftRightNeither.Left;
                }
                else
                {
                    leftRightNeither = LeftRightNeither.Right;
                }
            }
        }
        return leftRightNeither;
    }

    private GameObject CloneAndSpawn(int originalI, int originalJ, int spawnI, int spawnJ, int spawnLocI, int spawnLocJ)
    {
        GameObject tile;
        GameObject toSpawn;

        //Debug.Log(originalI + "," + originalJ);
        switch (board[originalI, originalJ].GetComponent<TilePiece>().TileType)
        {
            case TilePiece._TileType.HorizontalBlast: toSpawn = powerTilesHorizontal[board[originalI, originalJ].GetComponent<TilePiece>().Value]; break;
            case TilePiece._TileType.VerticalBlast: toSpawn = powerTilesVertical[board[originalI, originalJ].GetComponent<TilePiece>().Value]; break;
            case TilePiece._TileType.CrossBlast: toSpawn = powerTilesCross[board[originalI, originalJ].GetComponent<TilePiece>().Value]; break;
            case TilePiece._TileType.Rainbow: toSpawn = rainbowTile; break;
            case TilePiece._TileType.DropCount: toSpawn = rabbitPrefab; break;
            default: toSpawn = tiles[board[originalI, originalJ].GetComponent<TilePiece>().Value]; break;
        }
        tile = GameObject.Instantiate(toSpawn, GetScreenCoordinates(spawnLocI, spawnLocJ), Quaternion.identity);
        tile.GetComponent<TilePiece>().OriginalMoveable = true;
        tile.GetComponent<TilePiece>().Value = board[originalI, originalJ].GetComponent<TilePiece>().Value;
        tile.GetComponent<TilePiece>().OriginalValue = tile.GetComponent<TilePiece>().Value;
        tile.GetComponent<TilePiece>().SetLocation(spawnI, spawnJ);
        tile.GetComponent<TilePiece>().TileType = board[originalI, originalJ].GetComponent<TilePiece>().TileType;
        tile.GetComponent<TilePiece>().OriginalTileType = tile.GetComponent<TilePiece>().TileType;
        tile.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        return tile;
    }

    private Vector3 GetScreenCoordinates(int i, int j)
    {
        Vector3 position = new Vector3(
                j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                i * tileSize.y - boardSize.y / 2.5f + tileSize.y / 2.0f + 1.0f,
                0.0f
            );
        return position;
    }

    private Vector3 GetScreenCoordinatesWithZ(int i, int j, float z)
    {
        Vector3 position = new Vector3(
                j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                i * tileSize.y - boardSize.y / 2.5f + tileSize.y / 2.0f + 1.0f,
                z
            );
        return position;
    }

    private GameObject SpawnTileFromLocation(int row, int col, int targetRow, int targetCol, int value)
    {
        Vector3 newPos = GetScreenCoordinatesWithZ(row, col, -1.0f);
        GameObject newTile = GameObject.Instantiate(tiles[value], newPos, Quaternion.identity);
        newTile.name = "Generated";
        newTile.GetComponent<TilePiece>().Value = value;
        newTile.GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
        newTile.GetComponent<TilePiece>().Moveable = true;
        newTile.GetComponent<TilePiece>().OriginalMoveable = true;
        newTile.GetComponent<TilePiece>().SetLocation(targetRow, targetCol);
        newTile.GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        return newTile;
    }

    private GameObject SpawnTile(int row, int col)
    {
        if (dropCountSpawned < numFall)
        {
            float perc = ((float)(maxMoves - (float)numMoves)) / (float)maxMoves;
            float rand = Random.Range(0f, 1f);
            if (rand < perc)
            {
                dropCountSpawned++;
                return SpawnDropCountTile(row, col);
            }
        }
        int idx = Random.Range(0, tiles.Count);
        board[row, col] =
            GameObject.Instantiate(
                tiles[idx],
                new Vector3(
                    col * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    maxRows - 1 * tileSize.y,
                    0.0f),
                Quaternion.identity);
        board[row, col].GetComponent<TilePiece>().Value = idx;
        board[row, col].GetComponent<TilePiece>().SetLocation(row, col);
        board[row, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
        board[row, col].GetComponent<TilePiece>().Moveable = true;
        board[row, col].GetComponent<TilePiece>().OriginalMoveable = true;
        board[row, col].GetComponent<TilePiece>().NonBlocking = false;
        board[row, col].GetComponent<TilePiece>().OriginalTileType = board[row, col].GetComponent<TilePiece>().TileType;
        board[row, col].GetComponent<TilePiece>().OriginalValue = idx;
        board[row, col].GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        return board[row, col];

    }

    private GameObject SpawnDropCountTile(int row, int col) 
    {
        board[row, col] =
            GameObject.Instantiate(
                rabbitPrefab,
                new Vector3(
                    col * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    maxRows - 1 * tileSize.y,
                    0.0f),
                Quaternion.identity);
        board[row, col].GetComponent<TilePiece>().Value = -1;
        board[row, col].GetComponent<TilePiece>().SetLocation(row, col);
        board[row, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.DropCount;
        board[row, col].GetComponent<TilePiece>().Moveable = true;
        board[row, col].GetComponent<TilePiece>().OriginalMoveable = true;
        board[row, col].GetComponent<TilePiece>().NonBlocking = false;
        board[row, col].GetComponent<TilePiece>().OriginalTileType = board[row, col].GetComponent<TilePiece>().TileType;
        board[row, col].GetComponent<TilePiece>().OriginalValue = -1;
        board[row, col].GetComponent<TilePiece>().Indestructable = true;
        board[row, col].GetComponent<BoxCollider2D>().size = new Vector2(2f, 2f);
        return board[row, col];
    }

    private void SpawnHorizontalBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesHorizontal[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
        board[i, j].GetComponent<TilePiece>().OriginalTileType = board[i, j].GetComponent<TilePiece>().TileType;
        board[i, j].GetComponent<TilePiece>().OriginalValue = value;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnVerticalBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesVertical[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
        board[i, j].GetComponent<TilePiece>().OriginalTileType = board[i, j].GetComponent<TilePiece>().TileType;
        board[i, j].GetComponent<TilePiece>().OriginalValue = value;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnCrossBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesCross[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.CrossBlast;
        board[i, j].GetComponent<TilePiece>().OriginalTileType = board[i, j].GetComponent<TilePiece>().TileType;
        board[i, j].GetComponent<TilePiece>().OriginalValue = value;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnRainbowTile(int i, int j)
    {
        board[i, j] = GameObject.Instantiate(rainbowTile, GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = tiles.Count;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Rainbow;
        board[i, j].GetComponent<TilePiece>().OriginalTileType = board[i, j].GetComponent<TilePiece>().TileType;
        board[i, j].GetComponent<TilePiece>().OriginalValue = tiles.Count;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void IncrementScore(int amt)
    {
        numScore += amt * scoreMultiplier;

        gameController.SetLevelScore(level, numScore);
        gameController.AddOverallScore(numScore);

        gameController.SetProgress(tier1Fill, tier2Fill, tier3Fill, maxFillScore, numScore);
    }

    private void UpdateMoves(int _moves)
    {
        moves.text = _moves.ToString();
    }

    private int GetValueIJ(int i, int j)
    {
        return board[i, j].GetComponent<TilePiece>().Value;
    }

    IEnumerator TeleFall(int i, int j, float lerpTime)
    {
        float startTime = Time.time;
        locked = true;
        float currentLerp = 0.0f;
        Vector3 newPosition = GetScreenCoordinates(i - 1, j);
        GameObject go = CloneAndSpawn(i, j, i, j, i, j);
        go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        while (Time.time - startTime <= lerpTime)
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;

            go.transform.position = Vector3.Lerp(go.transform.position, newPosition, perc);
            yield return 1;
        }
        Destroy(go);
    }

    IEnumerator Fall(int i, int j, float lerpTime)
    {
        float startTime = Time.time; 
        locked = true;
        float currentLerp = 0.0f;
        Vector3 newPosition = GetScreenCoordinates(i, j);
        while (Time.time - startTime <= lerpTime) 
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;

            board[i, j].transform.position = Vector3.Lerp(board[i, j].transform.position, newPosition, perc);
            yield return 1;
        }
        falling--;
        if (falling == 0)
        {
            locked = false;
        }
    }

    public void LevelWin()
    {
        CancelInvoke("ShowHint");

        FinishWin();
    }

    private void FinishWin()
    {
        StartCoroutine(WinFinalize());
    }

    IEnumerator WinFinalize()
    {
        while (falling > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                if (board[i, j].GetComponent<TilePiece>().Moveable && board[i, j].GetComponent<TilePiece>().TileType != TilePiece._TileType.Regular)
                {
                    MarkDestroy(i, j);
                }
            }
        }
        FinalizeUnmoveable2(false);
        FinishMoves();
    }

    private void FinishMoves()
    {
        if (numMoves > 0)
        {
            finishing = true;
            InvokeRepeating("RandomExplode", 1.0f, 0.3f);
        } else
        {           
            ShowWinScreen();
        }
    }

    private void RandomExplode()
    {
        if (numMoves > 0)
        {
            GameObject targetTile = ChooseRandomTile();
            if (targetTile)
            {
                BombTile(targetTile);
            }
            numMoves--;
            UpdateMoves(numMoves);
        }
        else
        {
            CancelInvoke("RandomExplode");
            FinalizeUnmoveable2(false);
            ShowWinScreen();
        }
    }

    private GameObject ChooseRandomTileExclusive(int value)
    {
        GameObject randomTile;

        randomTile = board[Random.Range(0, maxRows), Random.Range(0, maxCols)];
        int max = maxCols * maxRows;
        int count = 0;
        while ((randomTile.GetComponent<TilePiece>().TileType != TilePiece._TileType.Regular 
            || randomTile.GetComponent<TilePiece>().Destroyed
            || randomTile.GetComponent<TilePiece>().Value == value
            || !randomTile.activeSelf) && (count < max))
        {
            randomTile = board[Random.Range(0, maxRows), Random.Range(0, maxCols)];
            count++;
        }
        if (count == max)
        {
            randomTile = null;
        }
        return randomTile;
    }

    private GameObject ChooseRandomTile()
    {
        GameObject randomTile;

        randomTile = board[Random.Range(0, maxRows), Random.Range(0, maxCols)];
        int max = maxCols * maxRows;
        int count = 0;
        while ((randomTile.GetComponent<TilePiece>().TileType != TilePiece._TileType.Regular 
            || randomTile.GetComponent<TilePiece>().Destroyed 
            || randomTile.GetComponent<TilePiece>().Indestructable
            || !randomTile.activeSelf) 
            && (count < max))
        {
            randomTile = board[Random.Range(0, maxRows), Random.Range(0, maxCols)];
            count++;
        }
        if (count == max)
        {
            randomTile = null;
        }
        return randomTile;
    }

    private void BombTile(GameObject targetTile)
    {
        int i = targetTile.GetComponent<TilePiece>().I;
        int j = targetTile.GetComponent<TilePiece>().J;

        //for (int row = i - 1; row < i + 2; row++)
        //{
        //    for (int col = j - 1; col < j + 2; col++)
        //    {
        //        if (row < maxRows && col < maxCols && row > 0 && col > 0)
        //        {
        //            if (board[row, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular && !board[row, col].GetComponent<TilePiece>().Destroyed && board[row, col].activeSelf)
        //            {                        
        //                MarkDestroy(row, col);
        //                board[row, col].SetActive(false);
        //            }
        //        }
        //    }
        //}
        MarkDestroy(i, j);
        //board[i, j].SetActive(false);
        board[i, j].GetComponent<SpriteRenderer>().enabled = false;
        IncrementScore(finalPointBonus);
    }

    private void ShowWinScreen()
    {
        StartCoroutine(DoShowWinScreen());
    }

    IEnumerator DoShowWinScreen()
    {
        while (falling > 0)
        {
            yield return null;
        }
        int stars = 0;
        if (numScore >= gameData.levelData[level].tier3Fill)
        {
            stars = 3;
        }
        else if (numScore >= gameData.levelData[level].tier2Fill)
        {
            stars = 2;
        }
        else if (numScore >= gameData.levelData[level].tier1Fill)
        {
            stars = 1;
        }
        gameController.LevelWin(level, numScore, GetEpochTime(), stars);
        gameController.ShowEndBoard(numMoves, stars);
        switch (Random.Range(0, 2))
        {
            case 0: gameController.ShowFireworks(); break;
            case 1: gameController.ShowSkyLanterns(); break;
            default: break;
        }
    }

    private void LevelLose()
    {
        gameController.PlayLoseSound();
        ShowLoseScreen();
    }

    private void ShowLoseScreen()
    {
        gameController.ShowLose();
    }

    private int GetEpochTime()
    {        
        System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;

        return secondsSinceEpoch;
    }
}