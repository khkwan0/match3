using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyArmController : MonoBehaviour {

    public float windTime, releaseTime;
    private bool throwing;
    private bool thrown;


    public Vector2 x = new Vector2();
    public Vector2 y = new Vector2();
    public float torque;
    public float lifeTimeSeconds;

    public GameObject bananaPrefab;
    private GameObject banana;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Throw()
    {
        StartCoroutine(DoThrow());
        transform.rotation = Quaternion.Euler(0f, 0f, -8f);
    }

    IEnumerator DoThrow()
    {
        while (throwing)
        {
            yield return null;
        }
        float startTime = Time.time;
        float perc = 0f;
        throwing = true;
        thrown = false;
        banana = GameObject.Instantiate(bananaPrefab, transform);
        while ((Time.time - startTime) < windTime)
        {
            yield return null;
            perc = (Time.time - startTime) / windTime;
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, -8f), Quaternion.Euler(0f, 0f, 38f), perc);
        }
        startTime = Time.time;
        while ((Time.time - startTime) < releaseTime)
        {
            yield return null;
            perc = (Time.time - startTime) / releaseTime;
            if (perc > 0.01f && !thrown)
            {
                thrown = true;
                banana.transform.parent = null;
                banana.GetComponent<Rigidbody>().useGravity = true;
                banana.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(x.x, x.y), Random.Range(y.x, y.y), 0f);
                banana.GetComponent<Rigidbody>().AddTorque(0f, 0f, torque);
                Destroy(banana, lifeTimeSeconds);
            }
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, 38f), Quaternion.Euler(0f, 0f, -8f), perc);
        }
        thrown = false;
        throwing = false;
    }
}
