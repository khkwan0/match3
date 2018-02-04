using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileCountPanelController : MonoBehaviour {
    public List<Sprite> tiles = new List<Sprite>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddTileCounter(List<MissionGoals> missionGoals)
    {
        float anchorPosX = -1f;
        float anchorPosY = 1f;
        float pivotPosX = anchorPosX;
        float pivotPosY = anchorPosY;
        for (int idx = 0; idx < missionGoals.Count; idx++) {
            int toReach = missionGoals[idx].toreach;

            GameObject panel = new GameObject(tiles[idx].name + "_panel", typeof(RectTransform));
            panel.AddComponent<CanvasRenderer>();
            panel.GetComponent<RectTransform>().SetParent(transform, false);
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 110f);


            GameObject go = new GameObject(tiles[idx].name, typeof(RectTransform));
            go.AddComponent<CanvasRenderer>();
            go.AddComponent<Image>();
            go.GetComponent<Image>().sprite = tiles[idx];
            go.GetComponent<RectTransform>().SetParent(panel.transform, false);
            go.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1f);


            GameObject goText = new GameObject(tiles[idx].name + "_text", typeof(RectTransform));
            goText.GetComponent<RectTransform>().SetParent(go.transform, false);
            goText.AddComponent<CanvasRenderer>();
            goText.AddComponent<TextMeshProUGUI>();
            goText.GetComponent<TextMeshProUGUI>().text = toReach.ToString();        
            goText.GetComponent<RectTransform>().localPosition = new Vector3(go.GetComponent<RectTransform>().rect.width * 1.5f, 0f, 0f);

            panel.GetComponent<RectTransform>().anchorMax = new Vector2(anchorPosX, anchorPosY);
            panel.GetComponent<RectTransform>().anchorMin = new Vector2(anchorPosX, anchorPosY);
            panel.GetComponent<RectTransform>().pivot = new Vector2(pivotPosX, pivotPosY);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(idx * panel.GetComponent<RectTransform>().rect.width, 0f, 0f);
        }
    }
}
