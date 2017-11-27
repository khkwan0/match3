using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    public static int maxRows = 8;
    public static int maxCols = 8;

    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> powerTilesVertical = new List<GameObject>();
    public List<GameObject> powerTilesHorizontal = new List<GameObject>();
    public List<GameObject> powerTilesCross = new List<GameObject>();
    public GameObject rainbowTile;
    private GameObject[,] board;
    public static Vector2 tileSize;
    public static Vector2 boardSize;

    public Camera cam;

    [SerializeField]
    private bool locked = false;

    private int falling;
    public GameObject canvas;
    private GameObject GUI;

    private Text score;
    private Text moves;
    private int numMoves;
    private int numScore;

    public int scoreAmt = 10;

    void Awake () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        board = new GameObject[maxRows, maxCols];
        boardSize = new Vector2(cam.aspect * 2f * cam.orthographicSize, 2f * cam.orthographicSize);
        tileSize = new Vector2(tiles[0].GetComponent<Renderer>().bounds.size.x, tiles[0].GetComponent<Renderer>().bounds.size.y);
        GUI = GameObject.Instantiate(canvas);
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

    void Start()
    {
        int hintI;
        int hintJ;


        numMoves = 50;
        numScore = 0;

        score = GUI.transform.GetChild(1).GetComponent<Text>();
        score.text = numScore.ToString();

        moves = GUI.transform.GetChild(3).GetComponent<Text>();
        moves.text = numMoves.ToString();             
        
        hintI = hintJ = -1;
        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < maxCols; j++)
            {
                int idx = (int)Mathf.FloorToInt(Random.Range(0f, tiles.Count - 1));
                while (IsContiguousHorizontal(i, j, idx) || IsContiguousVertical(i, j, idx)) {
                    idx = (int)Mathf.FloorToInt(Random.Range(0f, tiles.Count - 1));
                }
                board[i, j] = GameObject.Instantiate(
                    tiles[idx],
                    new Vector3(
                        j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                        i * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                        0.0f
                    ),
                    Quaternion.identity
                );
                board[i, j].SetActive(false);
                board[i, j].GetComponent<TilePiece>().Value = idx;
                board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
                board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            }
        }
        if (GetHint(out hintI, out hintJ)) {
            Debug.Log("Hint: Row " + hintI + ", Col " + hintJ);
            ShowTiles();
        } else
        {
            Reshuffle();
        }
    }

    private void Reshuffle()
    {
        Debug.Log("Reshuffle");
        Start();
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

    void Update() {
	}

    public GameObject[,] GetBoard()
    {
        return board;
    }

    public bool GetMatches(int i, int j, int switchedI, int switchedJ, bool isCascade)
    {
        bool matches = false;
        bool switchedMatches = false;

        if (!isCascade && 
            (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow ||
            board[switchedI, switchedJ].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow))
        {
            HandleRainbowMatch(i, j, switchedI, switchedJ);
            matches = true;
        }
        else
        {
            matches = CheckIJ(i, j, isCascade);
            if (!isCascade)
            {
                switchedMatches = CheckIJ(switchedI, switchedJ, isCascade);
            }
            if (matches || switchedMatches)
            {
                FinalizeAndFill();
                if (!isCascade)
                {
                    numMoves--;
                    moves.text = numMoves.ToString();
                }
            }
        }
        return matches || switchedMatches;
    }

    private bool CheckIJ(int i, int j, bool isCascade)
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
                HandleFiveMatch(i, j, true);
            }
            else
            {
                HandleFiveMatch(i, j, false);
            }
        }
        else if (!isCascade && CheckCross(i, j, out crossLowestRow, out crossLowestCol))
        {
            HandleCrossMatch(i, j, crossLowestRow, crossLowestCol);
        }
        else if (isCascade && CheckCascadeCross2())
        {

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
        } else
        {
            return false;
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
            DestroyAllTiles(value);
        }
        else
        if (board[si, sj].GetComponent<TilePiece>().TileType == TilePiece._TileType.Rainbow &&
            board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.Regular)
        {
            int value = board[i, j].GetComponent<TilePiece>().Value;
            board[si, sj].GetComponent<TilePiece>().Destroyed = true;
            board[si, sj].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
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
            DestroyAllTiles(value);
        }
    }

    private void HandleFiveMatch(int i, int j, bool horizontal)
    {
        int value = board[i, j].GetComponent<TilePiece>().Value;
        Destroy(board[i, j]);
        SpawnRainbowTile(i, j);
        if (horizontal)
        {
            MarkDestroy(i, j - 2);
            MarkDestroy(i, j - 1);
            MarkDestroy(i, j + 2);
            MarkDestroy(i, j + 1);
        } else  // it's vertical
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
        Destroy(board[i, j]);
        if (horizontal)
        {
            SpawnVerticalBlastTile(i, j, value);
            for (int col = lowest; col < lowest + 4; col++)
            {
                if (col != j)
                {
                    MarkDestroy(i, col);
                }
            }
        } else
        {
            SpawnVerticalBlastTile(i, j, value);
            for (int row = lowest; row < lowest + 4; row++)
            {
                if (row != i)
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
            for (int col = lowest; col < lowest + 3; col++)
            {
                MarkDestroy(i, col);
            }
        } else
        {
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
        if (CheckHorizontal(i, j, out crossLowestCol) == 3 && CheckVertical(i, j, out crossLowestRow) == 3)
        {
            found = true;
        }
        return found;
    }

    private bool CheckCascadeCross2()
    {
        bool found = false;

        return found;
    }
    private bool CheckCascadeCross(int i, int j, out int finalI, out int finalJ, out int horCount, out int vertCount, out int lowestRow, out int lowestCol)
    {
        bool foundCross = false;
        horCount = 0;
        vertCount = 0;
        lowestRow = lowestCol = -1;
        finalI = finalJ = -1;

        // check right
        if (j < maxCols - 3) {
            if (board[i,j].GetComponent<TilePiece>().Value == board[i, j + 1].GetComponent<TilePiece>().Value &&
                board[i,j].GetComponent<TilePiece>().Value == board[i, j + 2].GetComponent<TilePiece>().Value)
            {
                lowestCol = j;
                finalI = i;
                for (int col = j; col < j + 3 && ! foundCross; col++)
                {
                    if (i < maxRows - 2)
                    {  // check 2 up
                        if (board[i, col].GetComponent<TilePiece>().Value == board[i + 1, col].GetComponent<TilePiece>().Value &&
                            board[i, col].GetComponent<TilePiece>().Value == board[i + 2, col].GetComponent<TilePiece>().Value)
                        {
                            foundCross = true;
                            lowestRow = i;
                            finalJ = col;
                        }
                    }
                    if (!foundCross && i > 1)
                    { // check 2 down
                        if (board[i, col].GetComponent<TilePiece>().Value == board[i - 1, col].GetComponent<TilePiece>().Value &&
                            board[i, col].GetComponent<TilePiece>().Value == board[i - 2, col].GetComponent<TilePiece>().Value
                            )
                        {
                            foundCross = true;
                            lowestRow = i - 2;
                            finalJ = col;
                        }
                    }
                    if (!foundCross && i < maxRows - 1 && i > 0)
                    {
                        if (board[col, i].GetComponent<TilePiece>().Value == board[col, i + 1].GetComponent<TilePiece>().Value &&
                            board[col, i].GetComponent<TilePiece>().Value == board[col, i - 1].GetComponent<TilePiece>().Value
                            )
                        {
                            foundCross = true;
                            lowestRow = i - 1;
                            finalJ = col;
                        }
                    }
                }
            }
        }
        if (foundCross)
        {
            horCount = vertCount = 3;
        }
        return foundCross;
    }

    private int CheckHorizontal(int i, int j, out int lowestCol)
    {
        int count = 1;
        count += CheckLeft(i, j);
        lowestCol = j - count + 1;
        count += CheckRight(i, j);
        return count;
    }

    private int CheckLeft(int i, int j)
    {
        int count = 0;
        while (j > 0 && board[i, j - 1].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value)
        {
            count++;
            j--;
        }
        return count;
    }

    private int CheckRight(int i, int j)
    {
        int count = 0;
        while (j < maxCols - 1 && board[i, j + 1].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value)
        {
            count++;
            j++;
        }
        return count;
    }

    private int CheckVertical(int i, int j, out int lowestRow)
    {
        int count = 1;

        count += CheckDown(i, j);
        lowestRow = i - count + 1;
        count += CheckUp(i, j);      
        return count;
    }

    private int CheckUp(int i, int j)
    {
        int count = 0;
        while (i < maxRows - 1 && board[i + 1, j].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value)
        {
            count++;
            i++;
        }
        return count;
    }

    private int CheckDown(int i, int j)
    {
        int count = 0;
        while (i > 0 && board[i - 1, j].GetComponent<TilePiece>().Value == board[i, j].GetComponent<TilePiece>().Value)
        {
            count++;
            i--;
        }
        return count;
    }

    // algorithm:  look for a _potential_ match3 by looking for 2 and only 2 contiguous matching tiles
    // if found 2 contiguous matching tiles, then check one end to see if there are any surrounding tiles
    // that would match with the 2 already contiguous tiles.  If not, check the other end.  If still not then
    // move on to the next contiguous matching tiles (2 and only 2).
    public bool GetHint(out int i, out int j)
    {
        bool found = false;
        i = j = -1;
        for (int row = 0; row < maxRows - 2 && !found; row++)
        {
            for (int col = 0; col < maxCols - 2 && !found; col++)
            {
                int value = board[row, col].GetComponent<TilePiece>().Value;
                // check horizontal
                if (board[row, col + 1].GetComponent<TilePiece>().Value == value)  // we found 2 matching horizontal contiguous
                {
                    // check the left end
                    if (col > 0)
                    {
                        if (col > 1)
                        {
                            if (board[row, col - 2].GetComponent<TilePiece>().Value == value)
                            {
                                // we found a tile one away from the left end
                                i = col - 2;
                                j = row;
                                found = true;
                            }
                        }
                        if (row < maxRows - 1)
                        {
                            if (board[row + 1, col - 1].GetComponent<TilePiece>().Value == value)
                            {
                                // we found a matching tile above
                                i = row + 1;
                                j = col - 1;
                                found = true;
                            }
                        }
                        if (row > 0)
                        {
                            if (board[row -1, col -1].GetComponent<TilePiece>().Value == value)
                            {
                                i = row - 1;
                                j = col - 1;
                                found = true;
                            }
                        }
                    }
                    // check the right end
                    if (col < maxCols - 2)
                    {
                        if (col < maxCols - 3)
                        {
                            if (board[row, col + 3].GetComponent<TilePiece>().Value == value)
                            {
                                // found a matching tile one to the right of the right end
                                i = row;
                                j = col + 3;
                                found = true;                                
                            }
                        }
                        if (row < maxRows - 1)
                        {
                            if (board[row + 1, col + 2].GetComponent<TilePiece>().Value == value)
                            {
                                // found a matching tile on the right end, up one
                                i = row + 1;
                                j = col + 2;
                                found = true;
                            }                         
                        }
                        if (row > 0)
                        {
                            if (board[row - 1, col + 2].GetComponent<TilePiece>().Value == value)
                            {
                                i = row - 1;
                                j = col + 2;
                                found = true;
                            }
                        }
                    }

                }
                // check vertical
                if (board[row + 1, col].GetComponent<TilePiece>().Value == value)
                {
                    // check bottom
                    if (row > 0)
                    {
                        if (row > 1)
                        {
                            if (board[row - 2, col].GetComponent<TilePiece>().Value == value)
                            {
                                i = row - 2;
                                j = col;
                                found = true;
                            }
                        }
                        if (col > 0)
                        {
                            if (board[row - 1, col - 1].GetComponent<TilePiece>().Value == value)
                            {
                                i = row - 1;
                                j = col - 1;
                                found = true;
                            }                            
                        }
                        if (col < maxCols)
                        {
                            if (board[row - 1, col + 1].GetComponent<TilePiece>().Value == value)
                            {
                                i = row - 1;
                                j = col + 1;
                                found = true;
                            }
                        }
                    }
                    // check top
                    if (row < maxRows - 2)
                    {
                        if (row < maxRows - 3)
                        {
                            if (board[row + 3, col].GetComponent<TilePiece>().Value == value)
                            {
                                i = row + 3;
                                j = col;
                                found = true;
                            }
                        }
                        if (col > 0)
                        {
                            if (board[row + 2, col - 1].GetComponent<TilePiece>().Value == value)
                            {
                                i = row + 2;
                                j = col - 1;
                                found = true;
                            }
                        }
                        if (col < maxCols)
                        {
                            if (board[row + 2, col + 1].GetComponent<TilePiece>().Value == value)
                            {
                                i = row + 2;
                                j = col + 1;
                                found = true;
                            }
                        }
                    }
                }
            }
        }
        return found;
    }

    public void Cascade()
    {
        StartCoroutine(SpinLockOnFalling());
    }

    IEnumerator SpinLockOnFalling()
    {
        while (falling > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        bool match;

        match = false;

        for (int i = 0; i < maxRows && !match; i++)
        {
            for (int j = 0; j < maxCols && !match; j++)
            {
                match = GetMatches(i, j, -1, -1, true);
                if (match)
                {
                    Cascade();
                }
            }
        }
    }

    public void SwitchPositions(int ai, int aj, int bi, int bj)
    {
        locked = true;
        Vector3 newPositionB = new Vector3(aj * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f, ai * tileSize.y + tileSize.y / 2.0f - boardSize.y / 2.0f + 1.0f, 0.0f);
        Vector3 newPositionA = new Vector3(bj * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f, bi * tileSize.y + tileSize.y / 2.0f - boardSize.y / 2.0f + 1.0f, 0.0f);

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
                board[row, col].GetComponent<TilePiece>().Destroyed = true;
            }
        }
        FinalizeAndFill();
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
        FinalizeAndFill();
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
            board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
            for (int col = 0; col < maxCols; col++)
            {
                // turn all other horizontal blasts in same row into normal tile pieces to prevent infinite recursion
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.HorizontalBlast)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Regular;
                }
                // turn all cross blasts in same row into vertical blast since this horizontal blast is already triggering a row destruction
                // remember cross blasts destroy column AND row, need to avoid destroying the same row twice thus 
                // preventing an infinte recursion
                if (board[i, col].GetComponent<TilePiece>().TileType == TilePiece._TileType.CrossBlast)
                {
                    board[i, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
                }
            }
            DestroyRow(i);
        } else if (board[i, j].GetComponent<TilePiece>().TileType == TilePiece._TileType.VerticalBlast)
        {
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
        else
        {
            board[i, j].GetComponent<TilePiece>().Destroyed = true;
        }
    }

    private void DestroyRow(int row)
    {
        for (int j = 0; j < maxCols; j++)
        {
            MarkDestroy(row, j);
        }
    }

    private void DestroyCol(int col)
    {
        for (int i = 0; i< maxRows; i++)
        {
            MarkDestroy(i, col);
        }
    }

    private void FinalizeAndFill()
    {
        // finalize
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, col].GetComponent<TilePiece>().Destroyed)
                {
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
                            Destroy(board[row, col]);
                            GameObject toSpawn;
                            switch (board[i, col].GetComponent<TilePiece>().TileType)
                            {
                                case TilePiece._TileType.HorizontalBlast: toSpawn = powerTilesHorizontal[board[i, col].GetComponent<TilePiece>().Value]; break;
                                case TilePiece._TileType.VerticalBlast: toSpawn = powerTilesVertical[board[i, col].GetComponent<TilePiece>().Value]; break;
                                case TilePiece._TileType.CrossBlast: toSpawn = powerTilesCross[board[i, col].GetComponent<TilePiece>().Value]; break;
                                case TilePiece._TileType.Rainbow: toSpawn = rainbowTile; break;
                                default: toSpawn = tiles[board[i, col].GetComponent<TilePiece>().Value]; break;
                            }               
                            board[row, col] = GameObject.Instantiate(toSpawn, GetScreenCoordinates(i, col), Quaternion.identity);                            
                            board[row, col].GetComponent<TilePiece>().Value = board[i, col].GetComponent<TilePiece>().Value;
                            board[row, col].GetComponent<TilePiece>().SetLocation(row, col);
                            board[row, col].GetComponent<TilePiece>().TileType = board[i, col].GetComponent<TilePiece>().TileType;
                            board[i, col].GetComponent<TilePiece>().Destroyed = true;
                            falling++;
                            StartCoroutine(Fall(row, col));
                        }
                    }
                }
            }
        }
        for (int col = 0; col < maxCols; col++)
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (board[row, col].GetComponent<TilePiece>().Destroyed)
                {
                    IncrementScore(scoreAmt);
                    Destroy(board[row, col]);
                    SpawnTile(row, col);
                }
            }
        }
    }

    private Vector3 GetScreenCoordinates(int i, int j)
    {
        Vector3 position = new Vector3(
                j * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                i * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                0.0f
            );
        return position;
    }

    private void SpawnTile(int row, int col)
    {
        int idx = (int)Mathf.FloorToInt(Random.Range(0f, tiles.Count - 1));
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
        StartCoroutine(Fall(row, col));
    }

    private void SpawnHorizontalBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesHorizontal[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnVerticalBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesVertical[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnCrossBlastTile(int i, int j, int value)
    {
        board[i, j] = GameObject.Instantiate(powerTilesCross[value], GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = value;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.CrossBlast;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void SpawnRainbowTile(int i, int j)
    {
        board[i, j] = GameObject.Instantiate(rainbowTile, GetScreenCoordinates(i, j), Quaternion.identity);
        board[i, j].GetComponent<TilePiece>().Value = tiles.Count;
        board[i, j].GetComponent<TilePiece>().TileType = TilePiece._TileType.Rainbow;
        board[i, j].GetComponent<TilePiece>().SetLocation(i, j);
    }

    private void IncrementScore(int amt)
    {
        numScore += amt;
        score.text = numScore.ToString();
    }

    IEnumerator Fall(int i, int j)
    {
        float startTime = Time.time;
        while (Time.time - startTime <= 1.0f) 
        {
            Vector3 newPosition = new Vector3(board[i, j].transform.position.x, i * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f, 0.0f);
            board[i, j].transform.position = Vector3.Lerp(board[i, j].transform.position, newPosition, Time.time - startTime);
            yield return 1;
        }
        falling--;
    }
}

