﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public static int maxRows = 8;
    public static int maxCols = 8;

    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> powerTilesVertical = new List<GameObject>();
    public List<GameObject> powerTilesHorizontal = new List<GameObject>();
    public GameObject rainbowTile;
    private GameObject[,] board;
    public static Vector2 tileSize;
    public static Vector2 boardSize;

    public Camera cam;

    [SerializeField]
    private bool locked = false;

    private int falling;

	void Awake () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        board = new GameObject[maxRows, maxCols];
        boardSize = new Vector2(cam.aspect * 2f * cam.orthographicSize, 2f * cam.orthographicSize);
        tileSize = new Vector2(tiles[0].GetComponent<Renderer>().bounds.size.x, tiles[0].GetComponent<Renderer>().bounds.size.y);
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
        if (j>1)
        {
            if (board[i,j-1].GetComponent<TilePiece>().Value != value && board[i,j-2].GetComponent<TilePiece>().Value != value)
            {
                return false;
            } else
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
        if (i>1)
        {
            if (board[i-1, j].GetComponent<TilePiece>().Value != value && board[i-2, j].GetComponent<TilePiece>().Value != value)
            {
                return false;
            } else
            {
                return true;
            }
        } else
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
        bool collapseFoundH, collapseFoundV;
        int count;
        int row, col;
        int lowestRow, lowestCol;

        collapseFoundH = collapseFoundV = false;
        for (int i = 0; i < maxRows && !collapseFoundH && !collapseFoundV; i++)
        {
            for (int j = 0; j < maxCols && !collapseFoundH && !collapseFoundV; j++)
            {
                count = GetContiguousCountHorizontal(i, j, out row, out lowestCol);
                if (count >= 3)
                {
                    CollapseHorizontal(row, lowestCol, count, i, j);
                    falling += count;
                    collapseFoundH = true;
                }
                count = GetContiguousCountVertical(i, j, out col, out lowestRow);
                if (count >= 3)
                {
                    CollapseVertical(lowestRow, col, count, i, j);
                    collapseFoundV = true;
                    falling += count;
                }
                if (collapseFoundH || collapseFoundV)
                {
                    Cascade();
                }
            }
        }
    }

    public bool Collapse(int i, int j, int targetI, int targetJ)
    {
        bool collapseFound = false;
        int count = 0;
        int lowestCol, lowestRow;
        int row, col;

        count = GetContiguousCountHorizontal(i, j, out row, out lowestCol);
        if (count >= 3)
        {
            CollapseHorizontal(row, lowestCol, count, row, j);
            falling += count;
            collapseFound = true;
        }
        count = GetContiguousCountHorizontal(targetI, targetJ, out row, out lowestCol);
        if (count >= 3)
        {
            CollapseHorizontal(row, lowestCol, count, targetI, j);
            falling += count;
            collapseFound = true;
        }
        count = GetContiguousCountVertical(i, j, out col, out lowestRow);
        if (count >= 3)
        {
            CollapseVertical(lowestRow, col, count, i, col);
            collapseFound = true;
            falling += count;
        }
        count = GetContiguousCountVertical(targetI, targetJ, out col, out lowestRow);
        if (count >= 3)
        {
            CollapseVertical(lowestRow, col, count, i, targetJ);
            collapseFound = true;
            falling += count;
        }
        return collapseFound;
    }

    private int GetContiguousCountHorizontal(int i, int j, out int row, out int lowestCol)
    {
        int col;
        int count;
        int value = board[i, j].GetComponent<TilePiece>().Value;

        row = i;
        col = j;
        lowestCol = col;

        count = 1;

        // look left
        while (col - 1 >= 0 && board[row, col - 1].GetComponent<TilePiece>().Value == value)
        {
            col--;
            lowestCol--;
            count++;
        }

        row = i;
        col = j;

        // look right
        while (col + 1 < maxCols && board[row, col + 1].GetComponent<TilePiece>().Value == value)
        {
            col++;
            count++;            
        }
        return count;
    }
        
    private int GetContiguousCountVertical(int i, int j, out int col, out int lowestRow)
    {
        int count;
        int row;
        int value = board[i, j].GetComponent<TilePiece>().Value;

        count = 1;

        row = i;
        col = j;
        lowestRow = row;

        //Debug.Log(i + "," + j);
        // look down
        while (row - 1 >= 0 && board[row - 1, col].GetComponent<TilePiece>().Value == value)
        {
            lowestRow--;
            count++;
            row--;
        }
        row = i;
        col = j;

        // look up
        while (row + 1 < maxRows && board[row+1, col].GetComponent<TilePiece>().Value == value)
        {
            count++;
            row++;
        }

        //Debug.Log(lowestRow + ":" + count);
        return count;
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
        //Debug.Log("a: " + ai + "," + aj + "b: " + bi + "," + bj);
    }

    private void CollapseVertical(int lowestRow, int col, int count, int originalI, int originalJ)
    {
        int originalValue = -100; // unknown
        for (int row = lowestRow; row < lowestRow + count; row++)
        {
            if (originalValue < 0)
            {
                originalValue = board[row, col].GetComponent<TilePiece>().Value;
            }
            Destroy(board[row, col]);
        }
        if (count == 4)
        {
            // spawn a horizontal blast tile
            Vector3 position = new Vector3(
                    col * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    lowestRow * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                    0.0f
                );
            board[lowestRow, col] = GameObject.Instantiate(powerTilesHorizontal[originalValue], position, Quaternion.identity);
            board[lowestRow, col].GetComponent<TilePiece>().Value = originalValue;
            board[lowestRow, col].GetComponent<TilePiece>().SetLocation(lowestRow, col);
            board[lowestRow, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.HorizontalBlast;
        }
        if (count == 5)
        {
            // spawn a rainbow blast tile
            Vector3 position = new Vector3(
                    col * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    lowestRow * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                    0.0f
                );
            board[lowestRow, col] = GameObject.Instantiate(rainbowTile, position, Quaternion.identity);
            board[lowestRow, col].GetComponent<TilePiece>().Value = originalValue;
            board[lowestRow, col].GetComponent<TilePiece>().SetLocation(lowestRow, col);
            board[lowestRow, col].GetComponent<TilePiece>().TileType = TilePiece._TileType.Rainbow;
        }
        int modifier = count > 3 ? 1 : 0;
        //Debug.Log("modifier: " + modifier + ", maxrows - (lowestRow + count) - modifier = " + (maxRows - (lowestRow + count)));
        for (int i = 0; i < maxRows - (lowestRow + count); i++)
        {
            //Debug.Log(lowestRow + i);
            board[lowestRow + i + modifier, col] = board[lowestRow + count + i, col];
            board[lowestRow + i + modifier, col].GetComponent<TilePiece>().I = lowestRow + i + modifier;
            StartCoroutine(Fall(lowestRow + i + modifier, col));
        }
        for (int i = maxRows - count + modifier; i < maxRows; i++)
        {
            SpawnTile(i, col);
        }
    }

    private void CollapseHorizontal(int row, int lowestCol, int count, int originalI, int originalJ)
    {
        int originalValue = -100;  // unknown
        TilePiece._TileType tileType = board[originalI, originalJ].GetComponent<TilePiece>().TileType;

        for (int col = lowestCol; col < lowestCol + count; col++)
        {
            if (originalValue < 0)
            {
                originalValue = board[row, col].GetComponent<TilePiece>().Value;
            }
            Destroy(board[row, col]);
        }
        if (count == 4)
        {
            // spawn a vertical blast tile
            Vector3 position = new Vector3(
                    originalJ * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    originalI * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                    0.0f
                );
            board[row, originalJ] = GameObject.Instantiate(powerTilesVertical[originalValue], position, Quaternion.identity);
            board[row, originalJ].GetComponent<TilePiece>().Value = originalValue;
            board[row, originalJ].GetComponent<TilePiece>().SetLocation(row, originalJ);
            board[row, originalJ].GetComponent<TilePiece>().TileType = TilePiece._TileType.VerticalBlast;
        }
        if (count == 5)
        {
            // spawn a rainbow tile
            Vector3 position = new Vector3(
                    (lowestCol + 2) * tileSize.x - boardSize.x / 2.0f + tileSize.x / 2.0f + (boardSize.x - tileSize.x * maxCols) / 2.0f,
                    originalI * tileSize.y - boardSize.y / 2.0f + tileSize.y / 2.0f + 1.0f,
                    0.0f
                );
            board[row, lowestCol + 2] = GameObject.Instantiate(rainbowTile, position, Quaternion.identity);
            board[row, lowestCol + 2].GetComponent<TilePiece>().Value = tiles.Count;
            board[row, lowestCol + 2].GetComponent<TilePiece>().SetLocation(row, lowestCol + 2);
            board[row, lowestCol + 2].GetComponent<TilePiece>().TileType = TilePiece._TileType.Rainbow;
            originalJ = lowestCol + 2;
        }
        for (int col = lowestCol; col < lowestCol + count; col++)
        {
            for (int i = row; i < maxRows - 1; i++)
            {
                if (count < 4)
                {
                    board[i, col] = board[i + 1, col];
                    board[i, col].GetComponent<TilePiece>().I = i;
                    StartCoroutine(Fall(i, col));
                } else
                {
                    if (col != originalJ)
                    {
                        board[i, col] = board[i + 1, col];
                        board[i, col].GetComponent<TilePiece>().I = i;
                        StartCoroutine(Fall(i, col));
                    }
                }
            }
        }
        for (int col = lowestCol; col< lowestCol + count; col++)
        {
            if (count < 4)
            {
                SpawnTile(maxRows - 1, col);
            } else
            {
                if (col != originalJ)
                {
                    SpawnTile(maxRows - 1, col);
                }
            }
        }
        //for (int x = j; x < j + count; x++)
        //{
        //    Destroy(board[i, x]);
        //    for (int y = i; y < maxRows - 1; y++)
        //    {
        //        board[y, x] = board[y + 1, x];
        //        board[y, x].GetComponent<TilePiece>().I = y;
        //        Fall(y, x);
        //    }
        //    SpawnTile(maxRows - 1, x);
        //}
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
        board[row, col].GetComponent<TilePiece>().Value = idx; ;
        board[row, col].GetComponent<TilePiece>().SetLocation(row, col);
        StartCoroutine(Fall(row, col));
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

