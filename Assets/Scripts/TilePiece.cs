using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePiece : MonoBehaviour {

    private int value;
    [SerializeField]
    private int i, j;
    private Vector3 previousMousePosition = new Vector3();

    private Board boardObj;
    private GameObject[,] board;
    [SerializeField]
    private bool lockMoveAxisX, lockMoveAxisY, lockLeft, lockRight, lockUp, lockDown = false;

    private float originalX, originalY;
    [SerializeField]
    private int targetI, targetJ;

    private void Start()
    {
        boardObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<Board>();
        board = boardObj.GetBoard();
    }
    
    public int Value
    {
        get { return value; }
        set { this.value = value; }
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
        if (!boardObj.Collapse(i,j, targetI, targetJ))  // collapse
        {
            //StartCoroutine(WaitAndMoveOriginal());
            boardObj.SwitchPositions(targetI, targetJ, i, j);  // revert
        } else
        {
            boardObj.Cascade();
        }
        lockMoveAxisX = false;
        lockMoveAxisY = false;
    }

    public void OnMouseDrag()
    {
        //if (previousMousePosition != Input.mousePosition)
        //{
        //    if (Mathf.Abs(Input.mousePosition.x - previousMousePosition.x) >= Mathf.Abs(Input.mousePosition.y - previousMousePosition.y))
        //    {
        if (!boardObj.Locked)
        {
            if (!lockMoveAxisY && !lockRight && Input.mousePosition.x < previousMousePosition.x && j != 0)
            {
                lockMoveAxisX = true;
                lockLeft = true;
                targetI = i;
                targetJ = j - 1;
                boardObj.SwitchPositions(i, j, targetI, targetJ);
            }
            if (!lockMoveAxisY && !lockLeft && Input.mousePosition.x > previousMousePosition.x && j != Board.maxCols - 1)
            {
                lockMoveAxisX = true;
                lockRight = true;
                targetI = i;
                targetJ = j + 1;
                boardObj.SwitchPositions(i, j, targetI, targetJ);
            }
            //}
            //else
            //{
            if (!lockMoveAxisX && !lockUp && Input.mousePosition.y < previousMousePosition.y && i != 0)
            {
                lockMoveAxisY = true;
                lockDown = true;
                targetI = i - 1;
                targetJ = j;
                boardObj.SwitchPositions(i, j, targetI, targetJ);
            }
            if (!lockMoveAxisX && !lockDown && Input.mousePosition.y > previousMousePosition.y && i != Board.maxRows - 1)
            {
                lockMoveAxisY = true;
                lockUp = true;
                targetI = i + 1;
                targetJ = j;
                boardObj.SwitchPositions(i, j, targetI, targetJ);
            }
        }

            //}
            //transform.position = transform.position = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0.0f);
        //}
        previousMousePosition = Input.mousePosition;
    }

    //IEnumerator WaitAndMoveOriginal()
    //{
    //    //yield return new WaitForSeconds(delayTime);
    //    float startTime = Time.time;
    //    while (Time.time - startTime <= 1)
    //    {           
    //        Vector3 newPosition = new Vector3(originalX, originalY, 0.0f);
    //        Vector3 originalPosition = new Vector3(targetJ * Board.tileSize.x - Board.boardSize.x / 2.0f + Board.tileSize.x / 2.0f + (Board.boardSize.x - Board.tileSize.x * Board.maxCols) / 2.0f, targetI * Board.tileSize.y - Board.boardSize.y / 2.0f + Board.tileSize.y / 2.0f + 1.0f, 0.0f);

    //        transform.position = Vector3.Lerp(transform.position, newPosition, Time.time - startTime);
    //        board[targetI, targetJ].transform.position = Vector3.Lerp(board[targetI, targetJ].transform.position, originalPosition, Time.time - startTime);
    //        yield return null;
    //    }
    //}
}
