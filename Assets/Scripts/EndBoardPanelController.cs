using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBoardPanelController : MonoBehaviour {

    public GameObject postPanel;
    public GameObject starsPanel;

    public void SetText(string textString)
    {
        if (postPanel)
        {
            postPanel.GetComponent<PostPanelController>().SetText(textString);
        }
    }

    public void SetStars(int stars)
    {
        if (starsPanel)
        {
            starsPanel.GetComponent<StarsPanelController>().SetStars(stars);
        }
    }
}
