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
    public List<GameObject> powerTilesVertical = new List<GameObject>();
    public List<GameObject> powerTilesHorizontal = new List<GameObject>();
    public List<GameObject> powerTilesCross = new List<GameObject>();
    public List<GameObject> generatorTiles = new List<GameObject>();
    public GameObject rainbowTile;
    public GameObject unknownCrackable;
    private GameObject[,] board;
    public static Vector2 tileSize;
    public static Vector2 boardSize;

    [SerializeField]
    private bool locked = false;

    [SerializeField]
    private int falling;
    [SerializeField]
    private int generating;
    public GameObject canvas;
    private GameObject GUI;
    private ScoreBarFillController filler;

    private TextMeshProUGUI score;
    private TextMeshProUGUI moves;
    private TextMeshProUGUI missionText;
    private int numMoves;
    private int numScore;

    public int scoreAmt;

    public float hintTimeout = 3.0f;
    [SerializeField]
    private int hintI;
    [SerializeField]
    private int hintJ;

    private GameData gameData;
    private GameController gameController;
    private int level;
    private int scoreMultiplier;

    [SerializeField]
    private int cascadeCount;

    private float fillAmount;
    private int maxFillScore;
    private int stars = 0;
    private List<BoardSpec> boardSpec;

    private bool winLocked;

    public GameObject TileExplosion;
    private int missionType;
    [SerializeField]
    private bool finishing;
    int reshufflingCount;
    int finalPointBonus;

    public GameObject woodEnclosurePrefab;
    private GameObject woodEnclosure;

    private GameObject waves;
    private GameObject waves2;
    private Vector3 wavesPosition;
    private Vector3 waves2Position;

    public float fallLerpTime;
        
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

    public bool WinLocked
    {
        get { return winLocked; }
        set { winLocked = value; }
    }

    public void Start()
    {
        waves2 = GameObject.Instantiate(Resources.Load("Sprites/wave2") as GameObject);
        waves = GameObject.Instantiate(Resources.Load("Sprites/wave1") as GameObject);

        waves2.transform.position = new Vector3(1f, -1f * boardSize.y/2.5f + 1f, 1f);

        waves.transform.position = new Vector3(0f, -1f * boardSize.y / 2.5f, 0.9f);
        wavesPosition = waves.transform.position;
        waves2Position = waves2.transform.position;
        waves2.transform.SetParent(waves.transform);
    }

    public void Update()
    {
        wavesPosition.x += Mathf.Sin(Time.fixedTime) * 0.005f;
        waves.transform.position = wavesPosition;
        waves2Position.x -= Mathf.Sin(Time.fixedTime) * 0.0025f;
        waves2.transform.position = waves2Position;
    }

    public void StartLevel(int level)
    {
        scoreAmt = 20;
        fallLerpTime = 1.0f;
        finishing = false;
        winLocked = false;
        stars = 0;
        reshufflingCount = 0;
        finalPointBonus = 100;
        tileSize = new Vector2(tiles[0].GetComponent<Renderer>().bounds.size.x, tiles[0].GetComponent<Renderer>().bounds.size.y);
        GUI = GameObject.Instantiate(canvas);
        cascadeCount = 0;
        scoreMultiplier = cascadeCount + 1;

        numMoves = gameData.levelData[level].numMoves;
        numScore = 0;
        missionType = gameData.levelData[level].mission.type;

        this.level = gameData.levelData[level].level;
        maxRows = gameData.levelData[level].rows;
        maxCols = gameData.levelData[level].cols;

        board = new GameObject[maxRows, maxCols];       

        moves = GUI.transform.Find("Moves").GetComponent<TextMeshProUGUI>();
        moves.text = numMoves.ToString();

        missionText = GUI.transform.Find("Mission").GetComponent<TextMeshProUGUI>();
        maxFillScore = gameData.levelData[level].maxFillPoints;
        if (gameData.levelData[level].mission.type == 0)
        {
            missionText.text = "Get " + gameData.levelData[level].mission.missionGoals[0].score + " in " + numMoves + " moves or less.";
        }

        if (gameData.levelData[level].mission.type == 1)
        {
            missionText.text = "";
            for (int idx = 0; idx < gameData.levelData[level].mission.missionGoals.Count; idx++)         
            {
                Sprite theSprite = null;
                TilePiece._TileType tileType = TilePiece._TileType.Regular;
                int tileValue = gameData.levelData[level].mission.missionGoals[idx].tilevalue;
                switch(gameData.levelData[level].mission.missionGoals[idx].tiletype)
                {
                    case "regular": tileType = TilePiece._TileType.Regular; theSprite = tiles[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "horizontal": tileType = TilePiece._TileType.HorizontalBlast; theSprite = powerTilesHorizontal[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "vertical": tileType = TilePiece._TileType.VerticalBlast; theSprite = powerTilesVertical[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "cross": tileType = TilePiece._TileType.CrossBlast; theSprite = powerTilesCross[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    case "rainbow": tileType = TilePiece._TileType.Rainbow; theSprite = rainbowTile.GetComponent<SpriteRenderer>().sprite; break;
                    case "generator": tileType = TilePiece._TileType.Generator; theSprite = generatorTiles[gameData.levelData[level].mission.missionGoals[idx].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
                    default: break;
                }
                GUI.GetComponent<BoardCanvasController>().SpawnMissionGoal(gameData.levelData[level].mission.missionGoals[idx].toreach, tileType, tileValue, idx, theSprite);
            }
        }

        fillAmount = 0.0f;

        hintI = hintJ = -1;
        boardSpec = gameData.levelData[level].boardSpec;
        DrawBoard();
        while (!GetHint())
        {
            Debug.Log("Reshuffle");
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
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                bool specFound = false;
                for (int k = 0; k < boardSpec.Count && !specFound; k++)
                {
                    if (boardSpec[k].row == i && boardSpec[k].col == j)
                    {
                        specFound = true;
                        if (boardSpec[k].tiletype == "regular" 
                            || boardSpec[k].tiletype == "vertical" 
                            || boardSpec[k].tiletype == "horizontal" 
                            || boardSpec[k].tiletype == "cross" 
                            || boardSpec[k].tiletype == "rainbow"
                            || boardSpec[k].tiletype == "generator"
                            || boardSpec[k].tiletype == "unknowncrackable")
                        {
                            GameObject prefab;
                            switch(boardSpec[k].tiletype) 
                            {
                                case "cross": prefab = powerTilesCross[boardSpec[k].value];break;
                                case "vertical": prefab = powerTilesVertical[boardSpec[k].value];break;
                                case "horizontal": prefab = powerTilesHorizontal[boardSpec[k].value];break;
                                case "rainbow": prefab = rainbowTile;break;
                                case "generator": prefab = generatorTiles[boardSpec[k].value];break;
                                case "unknowncrackable": prefab = unknownCrackable;break;
                                default: prefab = tiles[boardSpec[k].value];break;
                            }
                            board[i, j] = GameObject.Instantiate(
                                prefab,
                                GetScreenCoordinates(i, j),
                                Quaternion.identity
                            );
                            if (boardSpec[k].tiletype == "unknowncrackable")
                            {
                                board[i, j].GetComponent<TilePiece>().HitPoints = boardSpec[k].hitpoints;
                            }
                            board[i, j].GetComponent<TilePiece>().Value = boardSpec[k].value;
                            board[i, j].GetComponent<TilePiece>().OriginalValue = boardSpec[k].value;
                            board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                            board[i, j].GetComponent<TilePiece>().Moveable = boardSpec[k].immoveable == 1 ? false : true;
                            board[i, j].GetComponent<TilePiece>().Indestructable = boardSpec[k].indestructable == 1 ? true : false;
                            board[i, j].GetComponent<TilePiece>().Invisible = boardSpec[k].invisible == 1 ? true : false;
                            board[i, j].GetComponent<TilePiece>().NonBlocking = boardSpec[k].nonblocking == 1 ? true : false;
                            TilePiece._TileType tt;
                            switch (boardSpec[k].tiletype)
                            {
                                case "regular": tt = TilePiece._TileType.Regular;break;
                                case "horizontal": tt = TilePiece._TileType.HorizontalBlast;break;
                                case "vertical": tt = TilePiece._TileType.VerticalBlast;break;
                                case "cross": tt = TilePiece._TileType.CrossBlast;break;
                                case "rainbow": tt = TilePiece._TileType.Rainbow;break;
                                case "generator": tt = TilePiece._TileType.Generator;break;
                                case "unknowncrackable": tt = TilePiece._TileType.UnknownCrackable;break;
                                default: tt = TilePiece._TileType.Regular; break;
                            }
                            board[i, j].GetComponent<TilePiece>().TileType = tt;
                            board[i, j].GetComponent<TilePiece>().OriginalTileType = tt;
                            board[i, j].name = prefab.name + "Xr" + i + "c" + j;
                        }
                        else
                        {
                            board[i, j] = new GameObject();
                            board[i, j].AddComponent<TilePiece>();
                            board[i, j].transform.SetPositionAndRotation(
                                    //new Vector3(
                                    //    j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                                    //    i * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                                    //    0.0f
                                    //),
                                    GetScreenCoordinates(i, j),
                                    Quaternion.identity
                                );
                            board[i, j].GetComponent<TilePiece>().Value = -1;
                            board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                            if (boardSpec[k].immoveable == 1)
                            {
                                board[i, j].GetComponent<TilePiece>().Moveable = false;
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
                                board[i, j].transform.localScale = new Vector3(1.38f, 1.38f, 1.0f);
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
                                board[i, j].name = "blabk" + "r" + i + "c" + j;
                            }
                        }
                        board[i, j].SetActive(false);
                    }
                }
                if (!specFound)
                {
                    int idx = Random.Range(0, tiles.Count);
                    while (IsContiguousHorizontal(i, j, idx) || IsContiguousVertical(i, j, idx))
                    {
                        idx = (int)Mathf.FloorToInt(Random.Range(0f, tiles.Count - 1));
                    }
                    board[i, j] = GameObject.Instantiate(
                        tiles[idx],
                        //new Vector3(
                        //    j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                        //    i * tileSize.y - boardSize.y / 2.5f + tileSize.y / 2.0f + 1.0f,
                        //    0.0f
                        //),
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

    public void ShowHint()
    {
        board[hintI, hintJ].GetComponent<TilePiece>().ShowHint();
    }

    private void ShowTiles()
    {
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j< maxCols; j++)
            {
                board[i, j].SetActive(true);
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
            FinalizeBoard();
            //Fill();
            numMoves--;
            UpdateMoves(numMoves);
            cascadeCount = 0;
            scoreMultiplier = cascadeCount + 1;
            CancelInvoke("ShowHint");
        }                   
        return matches;            
    }

    IEnumerator EnableHint()
    {
        while (falling > 0)
        {
            yield return null;
        }
        GetHint();
        if (hintI == -1 || hintJ == -1)
        {
            ReshuffleDrawn();
        }
        InvokeRepeating("ShowHint", hintTimeout, hintTimeout);
    }

    public bool CheckWinCondition()
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
            if (horCount >= 3 || vertCount >= 3)
            {
                Debug.Log("Cascade (i,j): (" + i + "," + j + ")");
            }

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
        else 
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            // one rainbow and one regular
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            int value = board[i, j].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.VerticalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else 
        if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
            int value = board[i, j].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.VerticalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.HorizontalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.HorizontalBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
        {
            int value = board[si, sj].GetComponent<TilePiece>().Value;
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
            ConvertTiles(value, TilePiece._TileType.CrossBlast);
            gameController.AddTileCount(level, TilePiece._TileType.Rainbow, tiles.Count);
            DestroyAllTiles(value);
        }
        else
        if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
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

    private void HandleGenerator(bool horizontal, int i, int j, int numberOfTilesToCheck)
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
                }
            }
        } else
        {
            for (int row = i; row < i + numberOfTilesToCheck; row++)
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
                HandleGenerator(true, i, lowest, 5);
            }
        } else
        {
            if (i != lowest + 2)
            {
                i = lowest + 2;
                HandleGenerator(false, lowest, j, 5);
            }
        }
        //int value = board[i, j].GetComponent<TilePiece>().Value;
        gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);
        Destroy(board[i, j]);
        SpawnRainbowTile(i, j);
        if (horizontal)
        {
            MarkDestroy(i, j - 2);
            MarkDestroy(i, j - 1);
            MarkDestroy(i, j + 2);
            MarkDestroy(i, j + 1);
        }
        else  // it's vertical
        {
            MarkDestroy(i + 2, j);
            MarkDestroy(i + 1, j);
            MarkDestroy(i - 2, j);
            MarkDestroy(i - 1, j);
        }
    }

    private void HandleCrossMatch(int i, int j, int lowestRow, int lowestCol)
    {
        int value = board[i, j].GetComponent<TilePiece>().Value;
        gameController.AddTileCount(level, board[i, j].GetComponent<TilePiece>().OriginalTileType, board[i, j].GetComponent<TilePiece>().OriginalValue);
        Destroy(board[i, j]);
        SpawnCrossBlastTile(i, j, value);
        // destroy vertical
        for (int row = lowestRow; row < lowestRow + 3; row++)
        {
            if (row != i)
            {
                MarkDestroy(row, j);
            }
        }
        for (int col = lowestCol; col < lowestCol + 3; col++)
        {
            if (col != j)
            {
                MarkDestroy(i, col);
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
                HandleGenerator(true, i, lowest, 4);
                SpawnVerticalBlastTile(i, j, value);
                for (int col = lowest; col < lowest + 4; col++)
                {
                    if (col != j)
                    {
                        MarkDestroy(i, col);
                    }
                }
            }
            else
            {
                HandleGenerator(false, lowest, j, 4);
                SpawnHorizontalBlastTile(i, j, value);
                for (int row = lowest; row < lowest + 4; row++)
                {
                    if (row != i)
                    {
                        MarkDestroy(row, j);
                    }
                }
            }
        } else
        {
            if (horizontal)
            {
                HandleGenerator(true, i, lowest, 4);
                for (int col = lowest; col < lowest + 4; col++)
                {
                    MarkDestroy(i, col);
                }
            }
            else
            {
                HandleGenerator(false, lowest, j, 4);
                for (int row = lowest; row < lowest + 4; row++)
                {
                    MarkDestroy(row, j);
                }
            }
        }
    }

    private void HandleThreeMatch(int i, int j, int lowest, bool horizontal)
    {
        if (horizontal)
        {
            HandleGenerator(true, i, lowest, 3);
            for (int col = lowest; col < lowest + 3; col++)
            {
                MarkDestroy(i, col);
            }
        } else
        {
            HandleGenerator(false, lowest, j, 3);
            for (int row = lowest; row < lowest + 3; row++)
            {
                MarkDestroy(row, j);
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
                    && !board[i, j + 1].GetComponent<TilePiece>().Destroyed)
                {
                    int value = GetValueIJ(i, j);
                    // potential for cross match with horiztonal base
                    for (int col = j; col < j + 3 && !found; col++)
                    {
                        if (GetValueIJ(i, j) == GetValueIJ(i + 1, col)
                            && GetValueIJ(i, j) == GetValueIJ(i + 2, col)
                            && !board[i + 1, j].GetComponent<TilePiece>().Destroyed
                            && !board[i + 2, j].GetComponent<TilePiece>().Destroyed)
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
                    && !board[i + 2, j].GetComponent<TilePiece>().Destroyed)
                {
                    int value = GetValueIJ(i, j);
                    for (int row = i + 1; row < i + 3 && !found; row++)
                    {
                        if (j > 1 && GetValueIJ(i, j) == GetValueIJ(row, j - 1)
                            && GetValueIJ(i, j) == GetValueIJ(row, j - 2)
                            && !board[row, j - 1].GetComponent<TilePiece>().Destroyed
                            && !board[row, j - 2].GetComponent<TilePiece>().Destroyed)
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
                            && !board[row, j + 2].GetComponent<TilePiece>().Destroyed)
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
                          && !board[row, j + 1].GetComponent<TilePiece>().Destroyed)
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

        if (!board[i, j].GetComponent<TilePiece>().Destroyed)
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
            && !board[i, j - 1].GetComponent<TilePiece>().Indestructable)
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
            && !board[i, j + 1].GetComponent<TilePiece>().Indestructable)
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

        if (!board[i, j].GetComponent<TilePiece>().Destroyed)
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
            && !board[i + 1, j].GetComponent<TilePiece>().Indestructable)
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
            && !board[i - 1, j].GetComponent<TilePiece>().Indestructable)
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
                    if (col > 0 && board[row, col - 1].GetComponent<TilePiece>().Moveable)
                    {
                        rainbowFound = true;
                    }
                    // check right
                    if (!rainbowFound && col < maxCols - 1 && board[row, col + 1].GetComponent<TilePiece>().Moveable)
                    {
                        rainbowFound = true;
                    }
                    // check up
                    if (!rainbowFound && row < maxRows - 1 && board[row + 1, col].GetComponent<TilePiece>().Moveable)
                    {
                        rainbowFound = true;
                    }
                    if (!rainbowFound && row > 0 && board[row - 1, col].GetComponent<TilePiece>().Moveable)
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
                if (board[row, col].GetComponent<TilePiece>().Moveable)
                {
                    // check left
                    if (col > 0  && board[row, col - 1].GetComponent<TilePiece>().Moveable)
                    {
                        if (col > 2)
                        {
                            // check if possible horizontal match
                            if (board[row, col - 3].GetComponent<TilePiece>().Value == board[row, col - 2].GetComponent<TilePiece>().Value && board[row, col - 2].GetComponent<TilePiece>().Value == value)
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
                                    if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row - 1, col - 1].GetComponent<TilePiece>().Value && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value)
                                    {
                                        found = true;
                                    }
                                }
                                if (!found && row > 1)
                                {
                                    if (board[row - 1, col - 1].GetComponent<TilePiece>().Value == board[row - 2, col - 1].GetComponent<TilePiece>().Value && board[row - 1, col - 1].GetComponent<TilePiece>().Value == value)
                                    {
                                        found = true;
                                    }
                                }
                                if (!found && row < maxRows - 2)
                                {
                                    if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 2, col - 1].GetComponent<TilePiece>().Value && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value)
                                    {
                                        found = true;
                                    }
                                }
                            }
                        }
                    }
                    // check right
                    if (!found && col < maxCols - 1 && board[row, col + 1].GetComponent<TilePiece>().Moveable)
                    {
                        if (col < maxCols - 3)
                        {
                            // check horizontal match
                            if (board[row, col + 2].GetComponent<TilePiece>().Value == board[row, col + 3].GetComponent<TilePiece>().Value && board[row, col + 3].GetComponent<TilePiece>().Value == value)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check if possible vertical match
                            if (row > 0 && row < maxRows - 1)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row - 1, col + 1].GetComponent<TilePiece>().Value && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && row > 1)
                            {
                                if (board[row - 1, col + 1].GetComponent<TilePiece>().Value == board[row - 2, col + 1].GetComponent<TilePiece>().Value && board[row - 1, col + 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && row < maxRows - 2)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row + 2, col + 1].GetComponent<TilePiece>().Value && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    // check up
                    if (!found && row < maxRows - 1 && board[row + 1, col].GetComponent<TilePiece>().Moveable)
                    {
                        if (row < maxRows - 3)
                        {
                            // check vertical match
                            if (board[row + 2, col].GetComponent<TilePiece>().Value == board[row + 3, col].GetComponent<TilePiece>().Value && board[row + 2, col].GetComponent<TilePiece>().Value == value)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check for horizontal match
                            if (col > 0 && col < maxCols - 1)
                            {
                                if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 1, col + 1].GetComponent<TilePiece>().Value && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col > 1)
                            {
                                if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == board[row + 1, col - 2].GetComponent<TilePiece>().Value && board[row + 1, col - 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col < maxCols - 2)
                            {
                                if (board[row + 1, col + 1].GetComponent<TilePiece>().Value == board[row + 1, col + 2].GetComponent<TilePiece>().Value && board[row + 1, col + 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    // check down
                    if (!found && row > 0 && board[row - 1, col].GetComponent<TilePiece>().Moveable)
                    {
                        if (row > 2)
                        {
                            // checked for vertical match
                            if (board[row - 2, col].GetComponent<TilePiece>().Value == board[row - 3, col].GetComponent<TilePiece>().Value && board[row - 2, col].GetComponent<TilePiece>().Value == value)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            // check for horizontal match
                            if (col > 0 && col < maxCols - 1)
                            {
                                if (board[row - 1, col - 1].GetComponent<TilePiece>().Value == board[row - 1, col + 1].GetComponent<TilePiece>().Value && board[row - 1, col - 1].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col > 2)
                            {
                                if (board[row - 1, col - 2].GetComponent<TilePiece>().Value == board[row - 1, col - 1].GetComponent<TilePiece>().Value && board[row - 1, col - 2].GetComponent<TilePiece>().Value == value)
                                {
                                    found = true;
                                }
                            }
                            if (!found && col < maxCols - 2)
                            {
                                if (board[row - 1, col + 1].GetComponent<TilePiece>().Value == board[row - 1, col + 2].GetComponent<TilePiece>().Value && board[row - 1, col + 1].GetComponent<TilePiece>().Value == value)
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
                if (board[i,j].GetComponent<TilePiece>().Moveable)
                {
                    go.Add(board[i, j]);
                }
            }
        }
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j< maxCols; j++)
            {
                if (board[i, j].GetComponent<TilePiece>().Moveable)
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
        FinalizeBoard();
        //Fill();
        StartCoroutine(FinalizeUnmoveable(true));

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
            FinalizeBoard();
            //Fill();
            Cascade(checkWin);
        }
        else
        {
            //StartCoroutine(FinalizeUnmoveable(checkWin));
            FinalizeUnmoveable2(checkWin);
        }
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
                    gameController.AddTileCount(level, board[row, col].GetComponent<TilePiece>().OriginalTileType, board[row, col].GetComponent<TilePiece>().OriginalValue);
                    board[row, col].GetComponent<TilePiece>().Destroyed = true;
                }
            }
        }
        FinalizeBoard();
        //Fill();
    }

    private void DestroyAllTiles(int value)
    {
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                if (board[row, col].GetComponent<TilePiece>().Value == value)
                {
                    MarkDestroy(row, col);
                }
            }
        }
        FinalizeBoard();
        //Fill();
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
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                    board[i, col].GetComponent<TilePiece>().OriginalTileType = TilePiece._TileType.HorizontalBlast;
                }
                // turn all cross blasts in same row into vertical blast since this horizontal blast is already triggering a row destruction
                // remember cross blasts destroy column AND row, need to avoid destroying the same row twice thus 
                // preventing an infinte recursion
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
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
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
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
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[row, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
                {
                    board[row, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
                }
            }
            for (int col = 0; col < maxCols; col++)
            {
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
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
            }
        }
    }

    private void DestroyRow(int row)
    {
        for (int j = 0; j < maxCols; j++)
        {
            if (!board[row, j].GetComponent<TilePiece>().Indestructable)
            {
                MarkDestroy(row, j);
            }
        }
    }

    private void DestroyCol(int col)
    {
        for (int i = 0; i< maxRows; i++)
        {
            if (!board[i, col].GetComponent<TilePiece>().Indestructable)
            {
                MarkDestroy(i, col);
            }
        }
    }

    private void FinalizeBoard()
    {
        StartCoroutine(DoFinalizeBoard());
    }

    IEnumerator DoFinalizeBoard()
    {
        while (falling > 0 || generating > 0)
        {
            yield return null;
        }
        // finalize - reorganize row or col, basically shift tile positions down so they fall to a new resting spot
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, col].GetComponent<TilePiece>().Destroyed)
                {
                    board[row, col].SetActive(false);
                    if (row < maxRows - 1)
                    {
                        int i = row + 1;
                        bool found = false;
                        while (i < maxRows && !found)
                        {
                            if (board[i, col].GetComponent<TilePiece>().Destroyed)
                            {
                                i++;
                            } else
                            {
                                found = true;                                    
                            }
                        }
                        if (found)
                        {
                            if (board[i, col].GetComponent<TilePiece>().Moveable)
                            {
                                Destroy(board[row, col]);
                                board[row, col] = CloneAndSpawn(i, col, row, col);
                                board[i, col].GetComponent<TilePiece>().Destroyed = true;
                                falling++;
                                StartCoroutine(Fall(row, col, fallLerpTime));


                            }
                            else if (board[i, col].GetComponent<TilePiece>().NonBlocking == false)
                            {
                                board[row, col].GetComponent<TilePiece>().Destroyed = true;
                                board[row, col].GetComponent<TilePiece>().DelayFill = true;
                            }
                        }
                    }
                }
            }
        }
        Fill();
    }

    private void FinalizeUnmoveable2(bool checkWin)
    {
        bool foundEmpty;
        bool match = false;
        do
        {
            foundEmpty = false;
            for (int row = 0; row < maxRows - 1; row++)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    if (board[row, col].GetComponent<TilePiece>().Destroyed)
                    {
                        match = true;
                        foundEmpty = true;
                        FillFromAbove(row + 1, col, row, col);
                    }
                }
            }
            Fill();
        } while (foundEmpty);
        if (match)
        {
            Cascade(checkWin);
        }
        else
        {
            FinalStop(checkWin);
        }
    }

    private void FinalStop(bool checkWin)
    {
        CancelInvoke("ShowHint");
        if (checkWin && CheckWinCondition())
        {
            winLocked = true;
            LevelWin();
        }
        else
        {
            if (checkWin)
            {
                EnableHintStart();
            }
        }

        if (!winLocked)
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
        if (CheckLoseCondition())
        {
            gameController.PlayLoseSound();
            ShowLoseScreen();
        }
    }

    private void FillFromAbove(int row, int col, int fromRow, int fromCol)
    {
        if (row >= maxRows)
        {
            return;
        }
        if (board[row, col].GetComponent<TilePiece>().Destroyed)
        {
            FillFromAbove(row + 1, col, row, col);
        }
        if (!board[row, col].GetComponent<TilePiece>().Moveable)
        {
            LeftRightNeither lrn = ChooseLeftRightNeither(row, col);
            if (lrn == LeftRightNeither.Left)
            {
                FillFromAbove(row, col - 1, row - 1, col);
            }
            if (lrn == LeftRightNeither.Right)
            {
                FillFromAbove(row, col + 1, row - 1, col);
            }
            if (lrn == LeftRightNeither.Neither)
            {
                return;
            }
        }
        if (board[row, col].GetComponent<TilePiece>().Moveable && !board[row, col].GetComponent<TilePiece>().Destroyed)
        {
            board[row, col].GetComponent<TilePiece>().Destroyed = true;
            board[row, col].GetComponent<SpriteRenderer>().enabled = false;
            Destroy(board[fromRow, fromCol]);
            board[fromRow, fromCol] = CloneAndSpawn(row, col, fromRow, fromCol);
            falling++;
            StartCoroutine(Fall(fromRow, fromCol, 1.0f));
        }        
    }

    IEnumerator FinalizeUnmoveable(bool checkWin)
    {
        bool match = false;
        while (falling > 0 || generating > 0)
        {
            yield return null;
        }
        for (int col = 0; col < maxCols; col++)
        {
            int row = 0;
            int destroyedRow = -1;
            int unmoveableRow = -1;
            while (row < maxRows && (destroyedRow < 0 || unmoveableRow < 0)) {
                //Debug.Log(row+","+col+" " +board[row, col].GetComponent<TilePiece>().Destroyed);
                if (board[row, col].GetComponent<TilePiece>().Destroyed && destroyedRow < 0)
                {
                    destroyedRow = row;
                }
                if (board[row, col].GetComponent<TilePiece>().Moveable == false && board[row, col].GetComponent<TilePiece>().NonBlocking == false)
                {
                    unmoveableRow = row;
                }
                row++;
            }
            if (destroyedRow >= 0 && unmoveableRow >= 0) {
                int i = destroyedRow;
                match = true;
                while (i < unmoveableRow)
                {
                    LeftRightNeither leftRightNeither = ChooseLeftRightNeither(unmoveableRow, col);
                    //Debug.Log(leftRightNeither);
                    if (leftRightNeither != LeftRightNeither.Neither)
                    { 
                        if (leftRightNeither == LeftRightNeither.Left)
                        {
                            while (falling > 0)
                            {
                                yield return null;
                            }
                            //Debug.Log("val left " + board[unmoveableRow, col - 1].GetComponent<TilePiece>().Value);
                            board[unmoveableRow, col - 1].GetComponent<TilePiece>().Destroyed = true;
                            board[unmoveableRow, col - 1].SetActive(false);
                            board[i, col] = CloneAndSpawn(unmoveableRow, col - 1, i, col);
                            falling++;
                            StartCoroutine(Fall(i, col, 0.5f));
                            for (int k = unmoveableRow; k < maxRows - 1; k++)
                            {
                                board[k + 1, col - 1].GetComponent<TilePiece>().Destroyed = true;
                                board[k + 1, col - 1].SetActive(false);
                                board[k, col - 1] = CloneAndSpawn(k + 1, col - 1, k, col - 1);
                                falling++;
                                StartCoroutine(Fall(k, col - 1, 0.5f));
                            };
                            board[maxRows - 1, col - 1].GetComponent<TilePiece>().Destroyed = true;
                            Fill();
                        }
                        else
                        {
                            while (falling > 0)
                            {
                                yield return null;
                            }
                            //Debug.Log("val righ " + board[unmoveableRow, col + 1].GetComponent<TilePiece>().Value);
                            board[unmoveableRow, col + 1].GetComponent<TilePiece>().Destroyed = true;
                            board[unmoveableRow, col + 1].SetActive(false);
                            board[i, col] = CloneAndSpawn(unmoveableRow, col + 1, i, col);
                            falling++;
                            StartCoroutine(Fall(i, col, 0.5f));
                            for (int k = unmoveableRow; k < maxRows - 1; k++)
                            {
                                board[k + 1, col + 1].GetComponent<TilePiece>().Destroyed = true;
                                board[k + 1, col + 1].SetActive(false);
                                board[k, col + 1] = CloneAndSpawn(k + 1, col + 1, k, col + 1);
                                falling++;
                                StartCoroutine(Fall(k, col + 1, 0.5f));
                            }
                            board[maxRows - 1, col + 1].GetComponent<TilePiece>().Destroyed = true;
                            Fill();
                        }
                    }
                    i++;
                }
            }
        }
        if (match)
        {
            Cascade(checkWin);
        }
        else  // final stop
        {
            CancelInvoke("ShowHint");
            if (checkWin && CheckWinCondition())
            {
                winLocked = true;
                LevelWin();
            }
            else
            {
                if (checkWin)
                {
                    EnableHintStart();
                }
            }

            if (!winLocked)
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
            if (CheckLoseCondition())
            {
                gameController.PlayLoseSound();
                ShowLoseScreen();
            }

        }
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
            if (!board[i, j - 1].GetComponent<TilePiece>().Moveable && board[i, j + 1].GetComponent<TilePiece>().Moveable)
            {
                leftRightNeither = LeftRightNeither.Right;
            }
            if (leftRightNeither == LeftRightNeither.Neither)
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

    private GameObject CloneAndSpawn(int originalI, int originalJ, int spawnI, int spawnJ)
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
            default: toSpawn = tiles[board[originalI, originalJ].GetComponent<TilePiece>().Value]; break;
        }
        tile = GameObject.Instantiate(toSpawn, GetScreenCoordinates(originalI, originalJ), Quaternion.identity);
        tile.GetComponent<TilePiece>().Value = board[originalI, originalJ].GetComponent<TilePiece>().Value;
        tile.GetComponent<TilePiece>().OriginalValue = tile.GetComponent<TilePiece>().Value;
        tile.GetComponent<TilePiece>().SetLocation(spawnI, spawnJ);
        tile.GetComponent<TilePiece>().TileType = board[originalI, originalJ].GetComponent<TilePiece>().TileType;
        tile.GetComponent<TilePiece>().OriginalTileType = tile.GetComponent<TilePiece>().TileType;
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
        newTile.GetComponent<TilePiece>().SetLocation(targetRow, targetCol);
        return newTile;
    }

    private void SpawnTile(int row, int col)
    {
        int idx = Random.Range(0, tiles.Count - 1);
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
        board[row, col].GetComponent<TilePiece>().OriginalTileType = board[row, col].GetComponent<TilePiece>().TileType;
        board[row, col].GetComponent<TilePiece>().OriginalValue = idx;
        falling++;
        StartCoroutine(Fall(row, col, fallLerpTime));
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

        fillAmount = 1.0f * numScore / maxFillScore;
        gameController.SetProgress(fillAmount);
    }

    private void UpdateMoves(int _moves)
    {
        moves.text = _moves.ToString();
    }

    private int GetValueIJ(int i, int j)
    {
        return board[i, j].GetComponent<TilePiece>().Value;
    }

    IEnumerator Fall(int i, int j, float lerpTime)
    {
        float startTime = Time.time; 
        locked = true;
        float currentLerp = 0.0f;
        while (Time.time - startTime <= lerpTime) 
        {
            currentLerp += Time.deltaTime;
            if (currentLerp > lerpTime)
            {
                currentLerp = lerpTime;
            }
            float perc = currentLerp / lerpTime;
            Vector3 newPosition = GetScreenCoordinates(i, j);
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
        CancelInvoke("ShowHint");
        gameController.LevelWin(level, numScore, GetEpochTime(), stars);
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
        FinalizeBoard();
        //Fill();
        Cascade(false);
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

    private void WaitForWinFinalize()
    {
        FinalizeBoard();
        //Fill();
        ShowWinScreen();
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
            FinalizeBoard();
            //Fill();
            Debug.Log("Final Cascade");
            Cascade(false);
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
        board[i, j].SetActive(false);
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
        gameController.ShowEndBoard(numMoves, stars);
        switch (Random.Range(0, 2))
        {
            case 0: gameController.ShowFireworks(); break;
            case 1: gameController.ShowSkyLanterns(); break;
            default: break;
        }
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