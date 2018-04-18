using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachViewController : MonoBehaviour {


    public Material skybox;
    public GameObject lighting;

    void Start () {
        RenderSettings.skybox = skybox;
        lighting.GetComponent<Light>().color = Color.white;
    }

}
