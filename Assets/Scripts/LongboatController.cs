using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongboatController : MonoBehaviour {

    public GameObject splashPrefab;
    private float rotateAmt;
    private bool shown;

	// Use this for initialization
	void Start () {
        shown = false;
	}
	
	// Update is called once per frame
	void Update () {
        //rotateAmt = Mathf.Sin(Time.time) * 0.1f;
        //transform.Rotate(Vector3.forward * rotateAmt);
        //ShowSplash();
	}

    private void ShowSplash()
    {
        if (!shown && transform.rotation.z < 0f)
        {
            GameObject splash = GameObject.Instantiate(splashPrefab, transform);
            Destroy(splash, 2f);
            shown = true;
        }
        if (transform.rotation.z > 0f)
        {
            shown = false;
        }
    }
}
