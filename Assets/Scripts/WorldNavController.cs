using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNavController : MonoBehaviour {

    private Camera cam;
    private Vector2 currPos = new Vector2();
    private Vector2 prevPos = new Vector2();
    private Vector2 dirVector = new Vector2();
    private Vector3 newCameraPos = new Vector3();
    private Vector3 cameraPos;
    private float magnitude;
    public float scrollSpeedLimiter;

    public float orthoZoomSpeed = 0.05f;
    public float accelerator = 4.0f;

	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cameraPos = cam.transform.position;
        scrollSpeedLimiter = 50.0f;
	}
	
	// Update is called once per frame
	void Update () {
        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d != 0f)
        {
            cam.orthographicSize -= d * orthoZoomSpeed * accelerator;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 6, 16f);
        }
		if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 6, 16f);
        }
	}

    public void OnMouseDown()
    {
        currPos = prevPos = Input.mousePosition;
    }
    public void OnMouseDrag()
    {
        currPos = Input.mousePosition;
        magnitude = (currPos - prevPos).magnitude;
        dirVector = (currPos - prevPos);
        newCameraPos.x = Mathf.Clamp(cam.transform.position.x - dirVector.x / scrollSpeedLimiter, -11f, 13f);
        newCameraPos.y = Mathf.Clamp(cam.transform.position.y - dirVector.y / scrollSpeedLimiter, 0f, 50f);
        newCameraPos.z = cam.transform.position.z;
        cam.transform.position = newCameraPos;
        prevPos = currPos;                        
    }

    public void OnMouseUp()
    {
        StartCoroutine(EaseCamera());
    }

    IEnumerator EaseCamera()
    {
        float maxMagnitude = 10.0f;
        float currentMagnitude;
        dirVector = dirVector.normalized;
        currentMagnitude = magnitude > maxMagnitude ? maxMagnitude : magnitude;
        while (currentMagnitude > 0)
        {
            newCameraPos.x = Mathf.Clamp(cam.transform.position.x - dirVector.x  * currentMagnitude / scrollSpeedLimiter, -11f, 13f);
            newCameraPos.y = Mathf.Clamp(cam.transform.position.y - dirVector.y  * currentMagnitude / scrollSpeedLimiter, 0f, 50f);
            newCameraPos.z = cam.transform.position.z;
            cam.transform.position = newCameraPos;
            currentMagnitude -= 0.1f;
            yield return null;
        }
    }
}
