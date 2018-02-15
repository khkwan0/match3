using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;

public class CurveController : MonoBehaviour {

    // Use this for initialization
    private BGCurve curve;

    public Transform anchor;
    public Transform midPoint;
	void Start () {
        BGCurve curve = GetComponent<BGCurve>();
        curve.Clear();
        
        curve.AddPoint(new BGCurvePoint(curve, Vector3.zero) { ControlType = BGCurvePoint.ControlTypeEnum.BezierSymmetrical });
        curve.AddPoint(new BGCurvePoint(curve, midPoint.position) { ControlType = BGCurvePoint.ControlTypeEnum.BezierSymmetrical });
        curve.AddPoint(new BGCurvePoint(curve, transform.parent.transform.position) { ControlType = BGCurvePoint.ControlTypeEnum.BezierSymmetrical });
        curve[0].ControlFirstLocal = -Vector3.right;
        curve[0].ControlSecondLocal = Vector3.right;
        curve[1].ControlFirstLocal = new Vector3(-1f, -1f, 0f);
        curve[1].ControlSecondLocal = new Vector3(1f, 1f, 0f);
        curve.transform.position = new Vector3(curve.transform.position.x, curve.transform.position.y, transform.parent.transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        BGCurve curve = GetComponent<BGCurve>();
        curve[0].PositionWorld = anchor.position;


        curve[2].PositionWorld = transform.parent.transform.position;
	}
}
