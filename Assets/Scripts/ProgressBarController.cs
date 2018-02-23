using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour {
    // Use this for initialization
    private GameObject star1, star2, star3;
    public float radius = 100f;
    private int prevScore;
    public GameObject effectPrefab;
    private GameController gc;

    void Start() {
        prevScore = 0;
        gc = GameController.GetGameController();
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlaceStars(int tier1, int tier2, int tier3, int maxFill)
    {
        star1 = transform.Find("Tier1StarOuter").gameObject;
        star2 = transform.Find("Tier2StarOuter").gameObject;
        star3 = transform.Find("Tier3StarOuter").gameObject;
        float perc;

        perc = GetPerc(tier1, maxFill);
        star1.transform.localPosition = new Vector3(Mathf.Cos(perc) * radius, Mathf.Sin(perc) * radius, 0f);
        perc = GetPerc(tier2, maxFill);
        star2.transform.localPosition = new Vector3(Mathf.Cos(perc) * radius, Mathf.Sin(perc) * radius, 0f);
        perc = GetPerc(tier3, maxFill);
        star3.transform.localPosition = new Vector3(Mathf.Cos(perc) * radius, Mathf.Sin(perc) * radius, 0f);


    }

    private float GetPerc(int val, int maxVal)
    {
        float perc;

        perc = ((float)val / (float)maxVal * 2f * 3.14f + 3.14f * .5f) * -1f;
        return perc;
    }

    public void CheckStarFill(int tier1, int tier2, int tier3, int numScore)
    {
        if (numScore >= tier1 && prevScore < tier1)
        {
            ShowStar(1);
        }
        if (numScore >= tier2 && prevScore < tier2)
        {
            ShowStar(2);
        }
        if (numScore >= tier3 && prevScore < tier3)
        {
            ShowStar(3);
        }
        prevScore = numScore;
    }

    private void ShowStar(int starIdx)
    {
        switch(starIdx)
        {
            case 1: star1.transform.Find("StarInner").gameObject.SetActive(true); Destroy(GameObject.Instantiate(effectPrefab, star1.transform), 5f); gc.PlayStarSound(); break;
            case 2: star2.transform.Find("StarInner").gameObject.SetActive(true); Destroy(GameObject.Instantiate(effectPrefab, star2.transform), 5f); gc.PlayStarSound();  break;
            case 3: star3.transform.Find("StarInner").gameObject.SetActive(true); Destroy(GameObject.Instantiate(effectPrefab, star3.transform), 5f); gc.PlayStarSound();  break;
            default:break;

        }
    }
}
