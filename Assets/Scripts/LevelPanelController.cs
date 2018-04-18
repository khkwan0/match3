using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelPanelController : MonoBehaviour {

    public Button startLevelButton;
    public Button cancelButton;
    private WorldCanvasController wcc;
    private GameController gc;
    public GameObject textArea;
    public List<GameObject> tilePanel;
    public List<GameObject> tilePanelImage;
    public List<GameObject> tilePanelText;
    public List<Sprite> tiles;
    public List<Sprite> bringToBottomImage;

    private GameObject levelMarker;

	// Use this for initialization

    public GameObject LevelMarker
    {
        get { return levelMarker; }
        set { levelMarker = value; }
    }

	void Start () {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        wcc = GameObject.Find("WorldControllerCanvas").GetComponent<WorldCanvasController>();
        cancelButton.onClick.AddListener(DoCancel);
        startLevelButton.onClick.AddListener(DoStart);
	}
	
    private void DoCancel()
    {
        wcc.HideLevel();
    }

    private void DoStart()
    {
        levelMarker.GetComponent<LevelMarkerController>().StartLevel();
    }

    public void GetLevelData(int level)
    {
        string missionText = "";
        LevelData ld = gc.GetLevelData(level);
        for (int i = 0; i < 3; i++)
        {
            tilePanel[i].SetActive(false);
        }
        switch(ld.mission.type)
        {
            case 0: missionText += "Get " + ld.mission.missionGoals[0].score + " points in " + ld.numMoves + " moves."; break;
            case 1:
                {
                    missionText += "Match the following: ";
                    for(int i = 0; i < ld.mission.missionGoals.Count; i++)
                    {
                        //ld.mission.missionGoals[0].tilevalue;
                        tilePanelImage[i].GetComponent<Image>().sprite = tiles[ld.mission.missionGoals[i].tilevalue];
                        tilePanelText[i].GetComponent<TextMeshProUGUI>().text = ld.mission.missionGoals[i].toreach.ToString();
                        tilePanel[i].SetActive(true);
                    }
                    break;
                }
            case 2:
                {
                    missionText += "Bring to the bottom: ";
                    tilePanelImage[1].GetComponent<Image>().sprite = bringToBottomImage[0];
                    tilePanelText[1].GetComponent<TextMeshProUGUI>().text = ld.mission.missionGoals[0].numfall.ToString();
                    tilePanel[1].SetActive(true);
                    break;
                }
            default: break;
        }
        textArea.GetComponent<TextMeshProUGUI>().text = missionText;


    }

	// Update is called once per frame
	void Update () {
		
	}
}
