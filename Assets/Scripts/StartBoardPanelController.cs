using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StartBoardPanelController : MonoBehaviour {

    public TextMeshProUGUI tmpGUI;
    public GameObject rewardsPanel;

    public void ShowRewards(List<Rewards> rewards)
    {
        rewardsPanel.GetComponent<RewardsPanelController>().SpawnReward(rewards);
    }

    public void SetText(string text)
    {
        tmpGUI.text = text;
    }
    
    public void AppendText(string text)
    {
        TextMeshProUGUI tmpgui = GameObject.FindGameObjectWithTag("StartPanelText").GetComponent<TextMeshProUGUI>();

        tmpGUI.text = tmpGUI.text + text;
    }

    public void ShowMissionGoals(List<MissionGoals> mg, GameObject board)
    {
        for (int i = 0; i < mg.Count; i++)
        {
            GameObject go = SpawnMissionGoal(i, board, mg);
            if (go)
            {
                //go.GetComponent<RectTransform>().SetParent(GameObject.FindGameObjectWithTag("StartPanelSpeechCanvas").transform.Find("PanelText"), false);
                go.transform.position = new Vector3(i * go.GetComponent<SpriteRenderer>().bounds.size.x - GetComponent<SpriteRenderer>().bounds.size.x/3, go.GetComponent<SpriteRenderer>().bounds.size.y/2, -2f);
                go.GetComponent<Transform>().SetParent(transform, false);
            }
        }
    }

    private GameObject SpawnMissionGoal(int i, GameObject board, List<MissionGoals> mg)
    {
        TilePiece._TileType tileType = TilePiece._TileType.Regular;
        Sprite theSprite = null;
        switch (mg[i].tiletype)
        {
            case "regular": tileType = TilePiece._TileType.Regular; theSprite = board.GetComponent<Board>().tiles[mg[i].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
            case "horizontal": tileType = TilePiece._TileType.HorizontalBlast; theSprite = board.GetComponent<Board>().powerTilesHorizontal[mg[i].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
            case "vertical": tileType = TilePiece._TileType.VerticalBlast; theSprite = board.GetComponent<Board>().powerTilesVertical[mg[i].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
            case "cross": tileType = TilePiece._TileType.CrossBlast; theSprite = board.GetComponent<Board>().powerTilesCross[mg[i].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
            case "rainbow": tileType = TilePiece._TileType.Rainbow; theSprite = board.GetComponent<Board>().rainbowTile.GetComponent<SpriteRenderer>().sprite; break;
            case "generator": tileType = TilePiece._TileType.Generator; theSprite = board.GetComponent<Board>().generatorTiles[mg[i].tilevalue].GetComponent<SpriteRenderer>().sprite; break;
            default: break;
        }
        GameObject go = null;
        if (theSprite)
        {
            GameObject missionImage = new GameObject(theSprite.name);
            missionImage.AddComponent<SpriteRenderer>();
            missionImage.GetComponent<SpriteRenderer>().sprite = theSprite;

            //GameObject missionText = new GameObject(theSprite.name + "_text", typeof(RectTransform));
            //missionText.AddComponent<CanvasScaler>();
            //missionText.GetComponent<RectTransform>().SetParent(missionImage.transform, false);
            //missionText.AddComponent<TextMeshProUGUI>();
            //missionText.GetComponent<TextMeshProUGUI>().text = mg[i].toreach.ToString();

            go = missionImage;
        }
        return go;
    }

}
