using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardCanvasController : MonoBehaviour {

    public GameObject missionGoalsCanvasPrefab;
     
    public void SpawnMissionGoal(int toReach, TilePiece._TileType tileType, int tileValue, int xOffset, Sprite theSprite)
    {
        GameObject canvas;
        Transform missionGoalsCanvas;

        missionGoalsCanvas = transform.Find("MissionGoalsCanvas");
        canvas = GameObject.Instantiate(missionGoalsCanvasPrefab, missionGoalsCanvas);

        canvas.GetComponent<RectTransform>().anchoredPosition = new Vector3(xOffset * missionGoalsCanvasPrefab.GetComponent<RectTransform>().rect.width, 0.0f, 0.0f);
        GameObject image = canvas.transform.Find("Image").gameObject;
        image.GetComponent<Image>().sprite = theSprite;
        canvas.GetComponent<GoalCanvas>().tileType = tileType;
        canvas.GetComponent<GoalCanvas>().value = tileValue;
        canvas.GetComponent<GoalCanvas>().toReach = toReach;
        canvas.transform.Find("reachtext").GetComponent<TextMeshProUGUI>().text = canvas.GetComponent<GoalCanvas>().toReach.ToString();
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
}
