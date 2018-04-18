using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour {

    public float cloudSpeed;  
    public float maxDistance;

    [SerializeField]
    private float distance;
    private Vector3 displacement;
    private Vector3 originalPosition;
    private Vector3 moveVelocity;
	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
        distance = 0f;
        moveVelocity = cloudSpeed * Vector3.right;

		
	}
	
	// Update is called once per frame
	void Update () {
        displacement = moveVelocity;
        distance += Mathf.Abs(cloudSpeed);
        transform.position += displacement;
        if (distance > maxDistance)
        {
            transform.position = originalPosition;
            distance = 0f;
        }
	}
}
