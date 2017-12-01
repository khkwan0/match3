using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBarFillController : MonoBehaviour {

    public float incrementAmt = 0.001f;
    private Image fillContent;
	// Use this for initialization
	void Start () {
        fillContent = transform.GetComponent<Image>();
        fillContent.fillAmount = 0.0f;
	}

    public void SetFill(float amt)
    {
        StartCoroutine(DoSetFill(amt));
    }

    IEnumerator DoSetFill(float amt)
    {
        while (fillContent.fillAmount < amt)
        {
            fillContent.fillAmount += incrementAmt;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
