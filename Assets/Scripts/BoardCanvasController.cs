using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardCanvasController : MonoBehaviour {

    public GameObject missionGoalsCanvasPrefab;
    private TextMeshProUGUI scoreArea;
    private GameObject progressBar;
    public GameObject dropCountPanel;
    public GameObject tileCountPanel;

    private void Awake()
    {
        scoreArea = GameObject.FindGameObjectWithTag("BoardScore").GetComponent<TextMeshProUGUI>();
        progressBar = GameObject.FindGameObjectWithTag("ProgressBar").gameObject;
        GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GetComponent<Canvas>().planeDistance = 3f;
    }

    public void SpawnMissionGoal(int toReach, TilePiece._TileType tileType, int tileValue, int xOffset, Sprite theSprite)
    { 
        GameObject tileCountPanel = GameObject.FindGameObjectWithTag("TileCountPanel");
        if (tileCountPanel)
        {
            GameObject canvas;
            canvas = GameObject.Instantiate(missionGoalsCanvasPrefab, tileCountPanel.transform);
            canvas.GetComponent<RectTransform>().anchoredPosition = new Vector3(xOffset * missionGoalsCanvasPrefab.GetComponent<RectTransform>().rect.width + 60f, 0.0f, 0.0f);
            GameObject image = canvas.transform.Find("Image").gameObject;
            image.GetComponent<Image>().sprite = theSprite;
            canvas.GetComponent<GoalCanvas>().tileType = tileType;
            canvas.GetComponent<GoalCanvas>().value = tileValue;
            canvas.GetComponent<GoalCanvas>().toReach = toReach;
            canvas.transform.Find("reachtext").GetComponent<TextMeshProUGUI>().text = canvas.GetComponent<GoalCanvas>().toReach.ToString();
        }
    }

    public void Deduct(TilePiece._TileType tileType, int value)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GoalsCanvas"))
        {
            if (go.GetComponent<GoalCanvas>().tileType == tileType && go.GetComponent<GoalCanvas>().value == value && go.GetComponent<GoalCanvas>().toReach > 0)
            {
                go.GetComponent<GoalCanvas>().toReach--;
                go.transform.Find("reachtext").GetComponent<TextMeshProUGUI>().text = go.GetComponent<GoalCanvas>().toReach.ToString();
            }
        }
    }

    public void ShowWin(int score, int stars)
    {
        GameObject go = transform.Find("WinCanvas").gameObject;
        go.SetActive(true);
    }

    public void ShowLose()
    {
        //GameObject go = transform.Find("WinCanvas").gameObject;
        //go.SetActive(false);
        //go = transform.Find("LoseCanvas").gameObject;
        //go.SetActive(true);
        
    }

    public void SetScore(int score)
    {
        scoreArea.text = score.ToString();
    }

    public void SetFillAmount(int tier1, int tier2, int tier3, int maxFill, int numScore)
    {
        float amt = (float)numScore / (float)maxFill;
        ProgressBarController pbc = transform.Find("ProgressUI").GetComponent<ProgressBarController>();
        pbc.CheckStarFill(tier1, tier2, tier3, numScore);
        progressBar.GetComponent<Image>().fillAmount = amt;
    }

    public void PlaceStars(int tier1, int tier2, int tier3, int mfs)
    {
        ProgressBarController pbc = transform.Find("ProgressUI").GetComponent<ProgressBarController>();
        pbc.PlaceStars(tier1, tier2, tier3, mfs);
    }

    public void ShowDropCountPanel()
    {
        dropCountPanel.SetActive(true);
    }

    public void SetDropCountText(string text)
    {
        GameObject.FindGameObjectWithTag("DropCountText").GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ShowTileCountPanel()
    {
        tileCountPanel.SetActive(true);
    }
}
