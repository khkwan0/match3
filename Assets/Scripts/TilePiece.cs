﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePiece : MonoBehaviour {

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
    private bool moveable;

    public enum _TileType { Regular, VerticalBlast, HorizontalBlast, CrossBlast, Rainbow };
    private _TileType tileType;

    public float swipeThreshhold = 0.15f;

    private bool destroyed;

    private void Start()
    {
        boardObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<Board>();
        board = boardObj.GetBoard();
        targetI = targetJ = -1;
        destroyed = false;
    }

    public bool Destroyed
    {
        get { return destroyed; }
        set { destroyed = value; }
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
        if (!boardObj.GetMatches(i,j, targetI, targetJ, false))  // collapse
        {
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
        if (!boardObj.Locked)
        {
            bool moveHor = false;
            bool moveVert = false;
            //Debug.Log("deltax" + (Input.mousePosition.x - previousMousePosition.x) + "," + (Input.mousePosition.y - previousMousePosition.y));
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
                    targetI = i;
                    targetJ = j - 1;
                    boardObj.SwitchPositions(i, j, targetI, targetJ);
                }
                else
                {
                    if (!lockMoveAxisY && !lockLeft && j != Board.maxCols - 1)
                    {
                        lockMoveAxisX = true;
                        lockRight = true;
                        targetI = i;
                        targetJ = j + 1;
                        boardObj.SwitchPositions(i, j, targetI, targetJ);
                    }
                }
            }
            if (moveVert)
            {
                if (!lockMoveAxisX && !lockUp && deltaY < 0.0f && i != 0)
                {
                    lockMoveAxisY = true;
                    lockDown = true;
                    targetI = i - 1;
                    targetJ = j;
                    boardObj.SwitchPositions(i, j, targetI, targetJ);
                }
                if (!lockMoveAxisX && !lockDown && deltaY > 0.0f && i != Board.maxRows - 1)
                {
                    lockMoveAxisY = true;
                    lockUp = true;
                    targetI = i + 1;
                    targetJ = j;
                    boardObj.SwitchPositions(i, j, targetI, targetJ);
                }
            }
        }
    }
}
