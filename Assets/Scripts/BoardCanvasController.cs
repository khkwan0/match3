using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardCanvasController : MonoBehaviour
{

    public GameObject missionGoalsCanvasPrefab;
    private TextMeshProUGUI scoreArea;
    private GameObject progressBar;
    public GameObject dropCountPanel;
    public GameObject tileCountPanel;
    public GameObject pauseMenu;
    public GameObject confirmPanel;
    public GameObject helperPanel;
    public GameObject pauseButton;
    public TextMeshProUGUI timerPanel;

    public Button resumeButton;
    public Button leaveButton;
    public Button confirmNoButton;    

    public float showPauseMenuLerpTime;

    private bool pauseMenuShown;
    private bool confirmMenuShown;

    public float helperLeftOffsetX;

    Vector3 pauseMenuOriginalPos;
    Vector3 confirmMenuOriginalPos;

    private bool chosenHelper;

    private float screenScaleWidth;
    public bool ChosenHepler
    {
        get { return chosenHelper; }
        set { chosenHelper = value; }
    }

    private void Awake()
    {
        scoreArea = GameObject.FindGameObjectWithTag("BoardScore").GetComponent<TextMeshProUGUI>();
        progressBar = GameObject.FindGameObjectWithTag("ProgressBar").gameObject;
        //GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GetComponent<Canvas>().planeDistance = 3f;
        pauseMenuShown = false;
        confirmMenuShown = false;
    }

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
        leaveButton.GetComponent<Button>().onClick.AddListener(ShowConfirmMenu);
        confirmNoButton.GetComponent<Button>().onClick.AddListener(HideConfirmMenu);
        pauseMenuOriginalPos = pauseMenu.transform.position;
        confirmMenuOriginalPos = confirmPanel.transform.position;
        chosenHelper = false;

        if (Screen.currentResolution.width < 1536)
        {
            screenScaleWidth = 1080f / 1536f;
        }
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

    public void TogglePauseMenu()
    {
        if (confirmMenuShown)
        {
            HideConfirmMenu();
        }
        else
        {
            if (pauseMenuShown)
            {
                GameController.GetGameController().PlaySwishDown();
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }

    public void ResumeGame()
    {
        GameController.GetGameController().PlaySwishDown();
        HidePauseMenu();
    }
    public void ShowPauseMenu()
    {
        GameController.GetGameController().PlaySwishUp();
        StartCoroutine(DoMoveMenu(pauseMenu, pauseMenuOriginalPos, new Vector3(0f, 0f, 0f), showPauseMenuLerpTime));
        pauseMenuShown = true;
    }

    public void HidePauseMenu()
    {

        StartCoroutine(DoMoveMenu(pauseMenu, pauseMenu.transform.position, new Vector3(0, -20f, 0f), showPauseMenuLerpTime));
        pauseMenuShown = false;
    }

    IEnumerator DoMoveMenu(GameObject go, Vector3 src, Vector3 dst, float lerpTIme)
    {
        float startTime = Time.time;
        float perc;
        Vector3 originalPos = src;
        go.transform.position = src;
        Vector3 newPos = dst;
        while ((Time.time - startTime) <= showPauseMenuLerpTime)
        {
            perc = (Time.time - startTime) / showPauseMenuLerpTime;
            go.GetComponent<RectTransform>().position = Vector3.Lerp(originalPos, newPos, perc);
            yield return null;
        }
        go.GetComponent<RectTransform>().position = dst;
    }

    public void ShowConfirmMenu()
    {
        HidePauseMenu();
        GameController.GetGameController().PlaySwishUp();
        StartCoroutine(DoMoveMenu(confirmPanel, confirmMenuOriginalPos, new Vector3(0f, 0f, 0f), showPauseMenuLerpTime));
        confirmMenuShown = true;
    }

    public void HideConfirmMenu()
    {
        GameController.GetGameController().PlaySwishDown();
        StartCoroutine(DoMoveMenu(confirmPanel, confirmPanel.transform.position, new Vector3(0, -20f, 0f), showPauseMenuLerpTime));
        confirmMenuShown = false;
    }

    public void ShowHelpers(LevelData levelData, PlayerData playerData)
    {
        if (levelData.helpers != null && levelData.helpers.Count > 0)
        {
            for (int i = 0; i < levelData.helpers.Count; i++)
            {
                AddHelper(levelData.helpers[i].helpertype, levelData.helpers[i].amount, i, playerData);
            }
        }
    }

    private void AddHelper(string helpertype, int amount, int offset, PlayerData pd)
    {       
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/helpers/" + helpertype));

        Debug.Log(Screen.currentResolution.width);
        if (Screen.currentResolution.width < 1536)
        {

            go.transform.localScale = new Vector3(screenScaleWidth, screenScaleWidth, 1f);
            go.transform.position += new Vector3(offset * go.GetComponent<RectTransform>().sizeDelta.x * screenScaleWidth, 0f, 0f);
        } else
        {
            go.transform.position += new Vector3(offset * go.GetComponent<RectTransform>().sizeDelta.x - helperLeftOffsetX, 0f, 0f);
        }
        go.GetComponent<RectTransform>().SetParent(helperPanel.transform, false);
        go.transform.Find("helperAmount").gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        Helper h = go.GetComponent<Helper>();
        switch (helpertype)
        {
            case "bomb": h.HelperType = GameController._helperType.Bomb; h.Amount = pd.bombHelper; break;
            case "rainbow": h.HelperType = GameController._helperType.Rainbow; h.Amount = pd.rainbowHelper; break;
            case "hammer": h.HelperType = GameController._helperType.Hammer; h.Amount = pd.hammerHelper; break;
            case "vertical": h.HelperType = GameController._helperType.Vertical; h.Amount = pd.verticalHelper; break;
            case "horizontal": h.HelperType = GameController._helperType.Horizontal; h.Amount = pd.horizontalHelper; break;
            default: break;
        }
        h.Amount += amount;
        go.GetComponent<Button>().onClick.AddListener(() => { HandleHelperClick(go.GetComponent<Button>()); });
    }

    private void HandleHelperClick(Button target)
    {
        if (target.GetComponent<Helper>().Amount > 0)
        {   
            for (int i = 0; i < helperPanel.transform.childCount; i++) {
                if (helperPanel.transform.GetChild(i).gameObject != target.gameObject)
                {
                    helperPanel.transform.GetChild(i).gameObject.SetActive(chosenHelper);
                }
            }
            if (chosenHelper)
            {
                GameController.GetGameController().Undarken();
                chosenHelper = false;
            }
            else
            {
                GameController.GetGameController().Darken();
                GameController.GetGameController().SetHelper(target.gameObject);
                chosenHelper = true;
            }
            
        }
    }

    public void HandlePostHelper()
    {
        GameController.GetGameController().Undarken();
        chosenHelper = false;
        for (int i = 0; i < helperPanel.transform.childCount; i++)
        {
            helperPanel.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UpdateTimer(int timeLeft)
    {
        int minutes = timeLeft / 60;
        int seconds = timeLeft % 60;

        string minText;
        string secText;
        if (minutes < 10)
        {
            minText = "0" + minutes.ToString();
        }
        else
        {
            minText = minutes.ToString();
        }
        if (seconds < 10)
        {
            secText = "0" + seconds.ToString();
        }
        else
        {
            secText = seconds.ToString();
        }
        timerPanel.text = minText + ":" + secText;
    }
}
