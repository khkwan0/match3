using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _787Controller : MonoBehaviour {

    // Use this for initialization

    private Vector3 originalPosition;   
    public float resetDistance;
    public float speed;
    private Vector3 moveVelocity;
    private float distanceTravelled;
    public float startDelay;
    public float repeatDelay;

	void Start () {
        originalPosition = transform.position;
        moveVelocity = speed * Vector3.right;
        InvokeRepeating("Fly", startDelay, repeatDelay);
	}	

    private void Fly()
    {
        StartCoroutine(DoFly());
    }

    IEnumerator DoFly()
    {
        distanceTravelled = 0f;
        while (distanceTravelled < resetDistance)
        {
            distanceTravelled += speed;
            transform.position += moveVelocity;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
