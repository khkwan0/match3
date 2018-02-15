using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyStarController : MonoBehaviour {

    // Use this for initialization
    private SpriteRenderer sr;
    private Color color;
    private float alpha;
    public float frequencyOffset = 0f;
    public float offset = 0f;

    public bool sin = true;
    public float speed = 1;
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;       
	}
	
	// Update is called once per frame
	void Update () {
        if (sin)
        {
            alpha = Mathf.Sin(Time.time * speed + frequencyOffset);
        }
        else
        {
            alpha = Mathf.Cos(Time.time * speed + frequencyOffset) + offset ;
        }
        color.a = alpha;
        sr.color = color;
	}
}
