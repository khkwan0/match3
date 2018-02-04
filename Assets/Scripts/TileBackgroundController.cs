using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBackgroundController : MonoBehaviour {

    public GameObject teleInPrefab;
    public GameObject teleOutPrefab;

    [SerializeField]
    private bool isTele;

    [SerializeField]
    private int i, j;

    [SerializeField]
    private int teleI, teleJ;

    public enum _teleDirection { In, Out };

    [SerializeField]
    private _teleDirection teleDirection;

    public bool IsTele
    {
        get { return isTele; }
        set { isTele = value; }
    }

    public _teleDirection TeleDirection {
        get { return teleDirection; }
        set { teleDirection = value; }
    }

    public int I
    {
        get { return i; }
        set { i = value; }
    }

    public int J
    {
        get { return j; }
        set { j = value; }
    }

    public void SetLocation(int i, int j)
    {
        this.i = i;
        this.j = j;
    }

    public int TeleI
    {
        get { return teleI; }
        set { teleI = value; }
    }

    public int TeleJ
    {
        get { return teleJ; }
        set { teleJ = value; }
    }

    public void SetTeleIJ(int i, int j)
    {
        teleI = i;
        teleJ = j;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
