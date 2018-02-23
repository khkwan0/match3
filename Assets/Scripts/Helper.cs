using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Helper : MonoBehaviour {

    private TextMeshProUGUI textMesh;

    [SerializeField]
    private GameController._helperType helperType;
    [SerializeField]
    private int amount = 0;

    private void Awake()
    {
        textMesh = transform.Find("helperAmount").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public int Amount
    {
        get { return amount; }
        set { amount = value; UpdateAmount(); }
    }

    public GameController._helperType HelperType
    {
        get { return helperType; }
        set { helperType = value; }
    }

    private void UpdateAmount()
    {
        textMesh.text = amount.ToString();  
    }
}
