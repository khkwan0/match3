using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePiece : MonoBehaviour {

    [SerializeField]
    private int value;
    [SerializeField]
    private int i, j;
    private Vector3 previousMousePosition;

    private Board boardObj;
    private GameObject[,] board;
    [SerializeField]
    private bool lockMoveAxisX, lockMoveAxisY, lockLeft, lockRight, lockUp, lockDown = false;

    private float originalX, originalY;
    [SerializeField]
    private int targetI, targetJ;

    private int layers;

    [SerializeField]
    private bool moveable = true;

    public enum _TileType { Regular, VerticalBlast, HorizontalBlast, CrossBlast, Rainbow, Indestructable, Steel, Invisible };
    private _TileType tileType;
    private _TileType originalTileType;
    private int originalValue;

    public float swipeThreshhold = 0.15f;

    private bool destroyed = false;

    private bool movedOne = false;

    private bool delayFill = false;

    [SerializeField]
    private bool nonBlocking = true;

    private void Start()
    {
        boardObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<Board>();
        board = boardObj.GetBoard();
        targetI = targetJ = -1;
    }

    public bool Destroyed
    {
        get { return destroyed; }
        set { destroyed = value; }
    }
    
    public bool DelayFill
    {
        get { return delayFill; }
        set { delayFill = value; }
    }
           
    public _TileType TileType
    {
        get { return tileType; }
        set { tileType = value; }
    }
    
    public int Layers
    {
        get { return layers; }
        set { layers = value; }
    }

    public bool Moveable
    {
        get { return moveable; }
        set { moveable = value; }
    }

    public bool NonBlocking
    {
        get { return nonBlocking; }
        set { nonBlocking = value; }
    }

    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public int OriginalValue
    {
        get { return originalValue; }
        set { this.originalValue = value; }
    }
    
    public _TileType OriginalTileType
    {
        get { return originalTileType; }
        set { originalTileType = value; }
    }

    public void SetLocation(int row, int col)
    {
        i = row;
        j = col;
    }

    public int I {
        get { return i; }
        set { i = value; }
    }

    public int J {     
        get { return j; }
        set { j = value; }
    }

    public int TargetI
    {
        get { return targetI; }
        set { targetI = value; }
    }

    public int TargetJ
    {
        get { return targetJ; }
        set { targetJ = value; }
    }

    public void OnMouseDown()
    {
        previousMousePosition = Input.mousePosition;
        originalX = transform.position.x;
        originalY = transform.position.y;
        //Debug.Log(i + "," + j + gameObject);
    }

    public void OnMouseUp()
    {
        if (boardObj.Locked)
        {
            StartCoroutine(SpinWait());
        } else
        {
            if (targetI >= 0 && targetJ >= 0)
            {
                CollapseOrRevert();
                targetI = targetJ = -1;
            }
        }        
    }

    IEnumerator SpinWait()
    {
        while (boardObj.Locked)
        {
            yield return null;
        }
        CollapseOrRevert();
    }

    private void CollapseOrRevert()
    {
        lockLeft = lockRight = false;
        lockUp = lockDown = false;
        if (movedOne)
        {
            if (boardObj.FoundSwitchMatch(i, j, targetI, targetJ))  // collapse
            {
                boardObj.Cascade(true);
                if (!boardObj.WinLocked)
                {
                    boardObj.EnableHintStart();
                }
             
                if (boardObj.CheckLoseCondition())
                {
                    Debug.Log("LOSE");
                }
            }
            else
            {
                boardObj.SwitchPositions(targetI, targetJ, i, j);  // revert
            }
        }
        movedOne = false;
        lockMoveAxisX = false;
        lockMoveAxisY = false;
    }

    public void OnMouseDrag()
    {
        if (!boardObj.Locked && !boardObj.WinLocked && moveable)
        {
            bool moveHor = false;
            bool moveVert = false;
            float deltaX = Input.mousePosition.x - previousMousePosition.x;
            float deltaY = Input.mousePosition.y - previousMousePosition.y;
            if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            {
                moveHor = true;
            }
            if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX))
            {             
                moveVert = true;
            }
            if (moveHor)
            {
                if (!lockMoveAxisY && !lockRight && deltaX < 0.0f && j != 0)
                {
                    lockMoveAxisX = true;
                    lockLeft = true;

                    if (!movedOne)
                    {
                        targetI = i;
                        targetJ = j - 1;
                        if (board[targetI, targetJ].GetComponent<TilePiece>().Moveable)
                        {
                            boardObj.SwitchPositions(i, j, targetI, targetJ);
                            movedOne = true;
                        }
                    }
                }
                else
                {
                    if (!lockMoveAxisY && !lockLeft && j != Board.maxCols - 1)
                    {
                        lockMoveAxisX = true;
                        lockRight = true;
                        if (!movedOne)
                        {
                            targetI = i;
                            targetJ = j + 1;
                            if (board[targetI, targetJ].GetComponent<TilePiece>().Moveable)
                            {
                                boardObj.SwitchPositions(i, j, targetI, targetJ);
                                movedOne = true;
                            }
                        }
                    }
                }
            }
            if (moveVert)
            {
                if (!lockMoveAxisX && !lockUp && deltaY < 0.0f && i != 0)
                {
                    lockMoveAxisY = true;
                    lockDown = true;
                    if (!movedOne)
                    {
                        targetI = i - 1;
                        targetJ = j;
                        if (board[targetI, targetJ].GetComponent<TilePiece>().Moveable)
                        {
                            boardObj.SwitchPositions(i, j, targetI, targetJ);
                            movedOne = true;
                        }
                    }
                }
                if (!lockMoveAxisX && !lockDown && deltaY > 0.0f && i != Board.maxRows - 1)
                {
                    lockMoveAxisY = true;
                    lockUp = true;
                    if (!movedOne)
                    {
                        targetI = i + 1;
                        targetJ = j;
                        if (board[targetI, targetJ].GetComponent<TilePiece>().Moveable)
                        {
                            boardObj.SwitchPositions(i, j, targetI, targetJ);
                            movedOne = true;
                        }
                    }
                }
            }
        }
    }

    public void ShowHint()
    {
        StartCoroutine(StandOut());
    }

    IEnumerator StandOut()
    {
        if (!boardObj.WinLocked)
        {
            float startTime = Time.time;
            float rotateAmount;
            for (float radians = 0f; radians < (4f * 3.14f); radians += 0.5f)
            {
                rotateAmount = Mathf.Sin(radians);
                if (radians < (2f * 3.14f))
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, rotateAmount * 4.0f));
                }
                else
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, rotateAmount * -4.0f));
                }
                yield return null;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }
}
