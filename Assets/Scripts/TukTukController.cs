using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline;

public class TukTukController : MonoBehaviour {

    public float speed;
    public BansheeGz.BGSpline.Curve.BGCurve pathCurve;
    private BansheeGz.BGSpline.Curve.BGCurvePointI[] points;
    private int currentPoint;

	void Start () {
        points = pathCurve.Points;
        currentPoint = 0;
        transform.LookAt(points[currentPoint].PositionWorld);
        Destroy(gameObject, 10f);

	}

	void Update () {        
        transform.position += transform.forward * speed;
	}

    private void OnTriggerEnter(Collider other)
    {
        currentPoint++;
        if (currentPoint < points.Length)
        {
            transform.LookAt(points[currentPoint].PositionWorld);
        }
    }
}
