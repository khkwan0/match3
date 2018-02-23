using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelperCanvasController : MonoBehaviour
{

    private List<GameObject> helpers = new List<GameObject>();

    public GameObject helperButtonPrefab;

    private TextMeshProUGUI textMesh;

    public void CreateHelpers(LevelData levelData, PlayerData playerData, Transform _parent)
    {
        for (int i = 0; i < levelData.helpers.Count; i++)
        {
            //GameObject hpb = GameObject.Instantiate(helperButtonPrefab, new Vector3(i * helperButtonPrefab.GetComponent<RectTransform>().sizeDelta.x, 0f, 0f), Quaternion.identity, transform);
            GameObject hpb = GameObject.Instantiate(helperButtonPrefab, _parent);
            hpb.transform.position += new Vector3((float)i * hpb.GetComponent<SpriteRenderer>().bounds.size.x, 0f, 0f);
            hpb.GetComponent<HelperController>().CreateHelper(levelData.helpers[i].helpertype, levelData.helpers[i].amount, playerData, hpb.transform);
            helpers.Add(hpb);
        }
    }

    public void MaskOtherHelpers(GameObject go)
    {
        for (int i = 0; i < helpers.Count; i++)
        {
            if (helpers[i] != go)
            {
                helpers[i].SetActive(false);
            }
        }
    }

    public void ShowAllHelpers()
    {
        for (int i = 0; i< helpers.Count; i++)
        {
            helpers[i].SetActive(true);
        }
    }



}
