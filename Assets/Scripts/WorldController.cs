using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorldController : MonoBehaviour {

    private GameObject panel;
    private GameObject gameControllerGO;
    private GameController game;
    private int numLevels;
    private int currentLevel;

    public GameObject levelButton;

    public void DoRender()
    {
        gameControllerGO = GameObject.Find("GameController");
        game = gameControllerGO.GetComponent<GameController>();
        numLevels = game.NumLevels;
        if (game.LastLevel < 0)
        {
            currentLevel = 0;
        } else
        {
            currentLevel = game.LastLevel + 1;
        }
        panel = GameObject.Find("Panel");
        Object.DontDestroyOnLoad(transform);
        DrawLevels();
    }

    private void DrawLevels()
    {
        GameObject button;
        for (int i=0; i < numLevels; i++)
        {
            button = GameObject.Instantiate(levelButton, panel.transform);
            button.GetComponent<LevelButton>().Level = i;
            button.transform.Find("Text").GetComponent<Text>().text = (i + 1).ToString();
        }
    }

    public void LevelOnClick()
    {
        GameObject clicked;

        clicked = EventSystem.current.currentSelectedGameObject;
        if (clicked != null)
        {
            //Debug.Log(clicked.GetComponent<LevelButton>().Level);
            GameObject.Find("GameController").GetComponent<GameController>().LoadLevel(clicked.GetComponent<LevelButton>().Level);
        }
    }
}