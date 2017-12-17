using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    private int level;

    public int Level
    {
        get { return level; }
        set { level = value; }
    }
}
