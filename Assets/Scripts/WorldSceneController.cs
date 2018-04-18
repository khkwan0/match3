using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneController : MonoBehaviour {

    public List<GameObject> elephants;
    public List<GameObject> birds;
    public GameObject boat;
    public GameObject tuktuk;
    public GameObject _thaiWoman;
    private GameObject thaiWoman;

    public List<GameObject> levels;
    private List<float> birdFlightDistance = new List<float>();
    private List<Vector3> birdOriginalPosition = new List<Vector3>();
    public List<float> birdDistanceThreshhold;

    public bool allowMove = true;

    public float birdSpeed;

    public GameObject cameraPos1;
    public GameObject cameraPos2;

    private GameController gc;
    private Camera cam;

    public float cameraLerpTime;

    private Animator anim;
    private int walkHash = Animator.StringToHash("walk");

    delegate void PostCameraMoveDelegate();
    PostCameraMoveDelegate PostCameraMoveCallback;

    public GameObject birdCollider;

    // Use this for initialization
    void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        birdCollider.GetComponent<BirdColliderScriptForWorldScene>().Target = "bird";
        GameObject gc_o = GameObject.Find("GameController");
        if (gc_o)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
        }
        SetCamera();
        birds[0].GetComponent<BirdController>().PlayFlap();
        birdFlightDistance.Add(0f);
        birdOriginalPosition.Add(birds[0].transform.position);
        InvokeRepeating("ElephantHind", 5f, 15f);

        thaiWoman = null;
    }

    private void SetCamera()
    {
        cam.transform.position = new Vector3(5.7f, 38.6f, -22.2f);
        cam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        cam.orthographic = false;
        cam.fieldOfView = 60f;
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 300;
        cameraPos2.transform.LookAt(tuktuk.transform);
        if (gc && gc.PlayerLastLevel >= 19)
        {
            PostCameraMoveCallback = null;
            StartCoroutine(MoveCamera(cameraPos2, cameraLerpTime, 2f));
        }
    }

    public void AnimateThaiWoman()
    {
        anim = thaiWoman.GetComponent<Animator>();
        anim.Play(walkHash);
    }

    public void SpawnThaiWoman()
    {
        if (!thaiWoman)
        {
            thaiWoman = GameObject.Instantiate(_thaiWoman, new Vector3(12.2f, 1.0f, 134.6f), Quaternion.identity);
            thaiWoman.name = "thai_woman";


            GameObject followPos = thaiWoman.transform.Find("FollowCamera").gameObject;
            followPos.transform.LookAt(thaiWoman.transform);
            cam.transform.SetParent(thaiWoman.transform);
            StartCoroutine(MoveCamera(followPos, 1f, 0f));
            PostCameraMoveCallback = AnimateThaiWoman;
        }
    }

    public void DeSpawnThaiWoman()
    {
        cam.transform.SetParent(transform.root);
        PostCameraMoveCallback = null;
        StartCoroutine(MoveCamera(cameraPos2, cameraLerpTime, 0f));
        Destroy(thaiWoman);
        thaiWoman = null;
    }

    public void OnMouseDown()
    {
        Debug.Log("STart movement");
    }
    public void OnMouseDrag()
    {
        Debug.Log(Input.mousePosition);
    }

    public void OnMouseUp()
    {
        Debug.Log("Stop movement");
    }
    IEnumerator MoveCamera(GameObject dest, float lerpTime, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        float perc;
        float startTime = Time.time;
        Vector3 originalPos = cam.transform.position;
        Quaternion originalRot = cam.transform.rotation;
        Vector3 finalPos = dest.transform.position;
        Quaternion finalRot = dest.transform.rotation;        
        while ((Time.time - startTime) < lerpTime)
        {
            perc = (Time.time - startTime) / lerpTime;
            cam.transform.position = Vector3.Slerp(originalPos, finalPos, perc);
            cam.transform.rotation = Quaternion.Slerp(originalRot, finalRot, perc);
            yield return null;          
        }
        if (PostCameraMoveCallback != null)
        {
            PostCameraMoveCallback();
        }
    }

	// Update is called once per frame
	void Update () {
        if (Input.touchCount == 1)
        {
            Debug.Log(Input.touchCount);
        }
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Debug.Log(touchDeltaPosition);
        }
        if (thaiWoman && thaiWoman.activeSelf && anim && anim.GetCurrentAnimatorStateInfo(0).shortNameHash == walkHash) {
            thaiWoman.transform.LookAt(tuktuk.transform);
            thaiWoman.transform.rotation = Quaternion.Euler(0f, thaiWoman.transform.rotation.eulerAngles.y, 0f);
            thaiWoman.transform.position += thaiWoman.transform.forward * 0.0125f;
        }
        
        if (gc && gc.PlayerLastLevel >= 20)
        {
            cam.transform.LookAt(tuktuk.transform);
        }
            

        birds[0].transform.position += transform.forward * birdSpeed;       
        birdFlightDistance[0] += birdSpeed;
        if (birdFlightDistance[0] > birdDistanceThreshhold[0])
        {
            ResetBird(0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            allowMove = !allowMove;
        }
        
	}

    private void ResetBird(int birdIndex)
    {
        birds[birdIndex].transform.position = birdOriginalPosition[birdIndex];
        birdFlightDistance[birdIndex] = 0f;
    }

    private void ElephantHind()
    {
        elephants[0].GetComponent<ElephantController>().Hind();
    }

    public void RotateTowardsTarget(GameObject target)
    {
        Vector3 targetDir = target.transform.position - boat.transform.position;
        StartCoroutine(DoRotateTowardsTarget(targetDir));            
    }

    public void MoveForward()
    {
        StartCoroutine(DoMoveTowardsTarget());
    }

    IEnumerator DoMoveTowardsTarget()
    {
        while (allowMove)
        {
            boat.transform.position += transform.worldToLocalMatrix.MultiplyVector(boat.transform.forward) * .05f;
            yield return null;
        }
    }

    IEnumerator DoRotateTowardsTarget(Vector3 targetDir)
    {
        float startTime = Time.time;
        Quaternion originalRotation = boat.transform.rotation;
        Quaternion finalRotation = Quaternion.LookRotation(targetDir);
        float perc;
        while (Time.time - startTime < 2f)
        {
            perc = (Time.time - startTime) / 2f;

            boat.transform.localRotation = Quaternion.Lerp(originalRotation, finalRotation, perc);
            Debug.DrawRay(boat.transform.position, transform.forward * 50f, Color.red);
            yield return null;
        }
        MoveForward();
    }
}
