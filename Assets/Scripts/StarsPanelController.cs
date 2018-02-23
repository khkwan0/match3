using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsPanelController : MonoBehaviour {

    public GameObject starInnerPrefab;
    public GameObject starOuterPrefab;

    private float left;

    public void SetStars(int stars)
    {
        GameObject go = null;
        for (int i = 0; i < stars; i++)
        {
            go = GameObject.Instantiate(starInnerPrefab, transform);
            left = go.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            go.transform.position = new Vector3(i * go.GetComponent<SpriteRenderer>().bounds.size.x/2f + go.transform.position.x - left, go.transform.position.y, -2f);
        }
        for (int i = stars; i < 3; i++)
        {
            go = GameObject.Instantiate(starOuterPrefab, transform);
            go.transform.position = new Vector3(i * go.GetComponent<SpriteRenderer>().bounds.size.x/2f + go.transform.position.x - left, go.transform.position.y, -2f);
            Color tmp = go.GetComponent<SpriteRenderer>().color;
            tmp.a = 0.5f;
            go.GetComponent<SpriteRenderer>().color = tmp;
        }
    }
}
