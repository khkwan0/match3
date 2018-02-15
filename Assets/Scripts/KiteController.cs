using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiteController : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private float xDisplacement;
    [SerializeField]
    private float yDisplacement;

    public float horizontalDisplacement;
    public float horizontalSpeed;

    public float verticalDisplacement;
    public float verticalSpeed;

    private Vector3 displacement = new Vector3();
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        xDisplacement = horizontalDisplacement * Mathf.Sin(Time.fixedTime * horizontalSpeed);
        yDisplacement = verticalDisplacement * Mathf.Sin(Time.fixedTime * verticalSpeed);
        displacement.x = xDisplacement;
        displacement.y = yDisplacement;
        displacement.z = 0f;
        transform.position += displacement;
	}
}
