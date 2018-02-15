using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiController : MonoBehaviour {

    private float speed;
    public float resetDistance;
    [SerializeField]
    private float distanceTravelled;
    public Vector2 delayRange = new Vector2();
    public Vector2 speedRange = new Vector2();
    private Vector3 moveVelocity;
    private Vector3 originalPos;

	void Start () {

        originalPos = transform.position;
        StartCoroutine("DoTaxi");
           
	}

    IEnumerator DoTaxi()
    {

        distanceTravelled = 0f;
        speed = Random.Range(speedRange.x, speedRange.y);
        moveVelocity = speed * Vector3.right;
        yield return new WaitForSeconds(Random.Range(delayRange.x, delayRange.y));

        while (distanceTravelled < resetDistance)
        {
            distanceTravelled += Mathf.Abs(speed);
            transform.position += moveVelocity;
            yield return null;
        }
        transform.position = originalPos;
        StartCoroutine("DoTaxi");
    }

}
