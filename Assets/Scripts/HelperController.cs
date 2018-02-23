using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelperController : MonoBehaviour {

    public GameObject hammerPrefab;
    public GameObject VerticalBlastPrefab;
    public GameObject HorizontalBlastPrefab;
    public GameObject RainbowPrefab;
    public GameObject BombPrefab;

    private GameObject helper;

    private int amount;

    private TextMeshProUGUI tmp;

    private GameController._helperType helperType;

    public GameController._helperType HelperType
    {
        get { return helperType; }
        set { helperType = value; }
    }

    private void Awake()
    {
        tmp = transform.Find("AmountCanvas").gameObject.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
    }

    public void CreateHelper(string helperType_s, int amount, PlayerData playerData, Transform _parent)
    {
        GameObject toSpawn = null;
        switch (helperType_s)
        {
            case "hammer": toSpawn = hammerPrefab; helperType = GameController._helperType.Hammer; break;
            case "horizontal": toSpawn = HorizontalBlastPrefab; helperType = GameController._helperType.Horizontal; break;
            case "vertical": toSpawn = VerticalBlastPrefab; helperType = GameController._helperType.Vertical; break;
            case "rainbow": toSpawn = RainbowPrefab; helperType = GameController._helperType.Rainbow; break;
            case "bomb": toSpawn = BombPrefab; helperType = GameController._helperType.Bomb; break;
            default: helperType = GameController._helperType.None; break;
        }
        if (playerData != null)
        {
            switch (helperType_s)
            {
                case "hammer": amount += playerData.hammerHelper; break;
                case "horizontal": amount += playerData.horizontalHelper; break;
                case "vertical": amount += playerData.verticalHelper; break;
                case "rainbow": amount += playerData.rainbowHelper; break;
                case "bomb": amount += playerData.bombHelper; break;
                default: break;
            }
        }
        if (toSpawn)
        {
            helper = GameObject.Instantiate(toSpawn, _parent);
            this.amount = amount;
            UpdateShowAmount(amount);
            //helper.transform.Find("AmountCanvas").GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    public int Amount
    {
        get { return amount; }
        set { UpdateShowAmount(value); amount = value; }
    }

    public void Increment()
    {
        amount++;
        UpdateShowAmount(amount);
    }

    public void Decrement()
    {
        amount--;
        UpdateShowAmount(amount);
    }

    private void UpdateShowAmount(int amount)
    {
        tmp.text = amount.ToString();
    }

    //public void OnMouseDown()
    //{
    //    GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    //    if (gc.CurrentHelper == gameObject)
    //    {
    //        gc.SetHelper(null);
    //    }
    //    else if (amount > 0)
    //    {
    //        gc.SetHelper(gameObject);
    //    }
    //}
}
