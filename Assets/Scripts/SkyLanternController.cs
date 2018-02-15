using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyLanternController : MonoBehaviour {

    // Use this for initialization
    private float verticalSpeed;
    private float horizontalSpeed;
    public Vector2 hor = new Vector2();
    public Vector2 ver = new Vector2();
    public Vector2 swaySpeed = new Vector2();
    public Vector2 swayAmt = new Vector2();
    private float sway;
    private float swayScale;
    public Vector2 size = new Vector2();

	void Start () {
        horizontalSpeed = Random.Range(hor.x, hor.y);
        verticalSpeed = Random.Range(ver.x, ver.y);
        sway = Random.Range(swaySpeed.x, swaySpeed.y);
        swayScale = Random.Range(swayAmt.x, swayAmt.y);
        transform.localScale = Random.Range(size.x, size.y) * Vector3.one;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += verticalSpeed * Vector3.up + Mathf.Sin(Time.time + sway) * swayScale * Vector3.right + horizontalSpeed * Vector3.right;
	}
}
