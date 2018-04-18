using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSceneController : MonoBehaviour {

    public List<GameObject> birds;
    public List<GameObject> movingClouds;
    public GameObject bottomCloud1;
    public GameObject bottomCloud2;

    [Range (0.5f, 1f)]
    public float maxElevationChange;
    private float minElevationChange = 0.5f;

    [Range(3f, 5f)]
    public float maxRollTime;
    private float minRollTime = 3f;

    [Range(3f, 5f)]
    public float maxLoopTime;
    private float minLoopTime = 3f;

    [Range(10f, 30f)]
    public float maxPitchAngle;
    private float minPitchAngle;

    [Range(0.2f, 1f)]
    public float maxLateralSpeed;
    private float minLateralSpeed = 0.2f;

    [Range(1f, 2f)]
    public float maxLateralTime;
    private float minLateralTime = 1f;

    [Range(0.1f, 1f)]
    public float maxCloudSpeed;
    private float minCloudSpeed = 0.1f;

    public float maxCloudDistance;
    public float bottomCloudSpeed;
    Vector3 _bottomCloudSpeed = new Vector3();

    public float leftBounds;

    private Vector3 bottomCloud2OriginalPos;
    // Use this for initialization
    void Start () {
        InvokeRepeating("DoAction", 5f, 5f);
        InvokeRepeating("ShowClouds", Random.Range(0.5f, 1f), 0f);
        bottomCloud2OriginalPos = bottomCloud2.transform.position;
        _bottomCloudSpeed.y = 0f;
        _bottomCloudSpeed.z = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        _bottomCloudSpeed.x = -bottomCloudSpeed;

        bottomCloud1.transform.position += _bottomCloudSpeed;
        bottomCloud2.transform.position += _bottomCloudSpeed;
        if (bottomCloud1.transform.localPosition.x < leftBounds)
        {
            bottomCloud1.transform.position = bottomCloud2OriginalPos;
        }
        if (bottomCloud2.transform.localPosition.x < leftBounds)
        {
            bottomCloud2.transform.position = bottomCloud2OriginalPos;
        }

	}

    void DoAction()
    {
        BirdController bird = birds[Random.Range(0, birds.Count)].GetComponent<BirdController>();
        BirdController._birdMovement birdMovement = (BirdController._birdMovement)Random.Range(0, (int)BirdController._birdMovement.Count);
        switch (birdMovement)
        {
            case BirdController._birdMovement.Soar: bird.Soar(); break;
            case BirdController._birdMovement.Flapping: bird.PlayFlap(); break;
            case BirdController._birdMovement.Forward: bird.MoveForward(Random.Range(minLateralTime, maxLateralTime), Random.Range(minLateralSpeed, maxLateralSpeed)); break;
            case BirdController._birdMovement.Backward: bird.MoveBackward(Random.Range(minLateralTime, maxLateralTime), Random.Range(minLateralSpeed, maxLateralSpeed)); break;
            case BirdController._birdMovement.Ascend: bird.Ascend(Random.Range(minElevationChange, maxElevationChange), Random.Range(minPitchAngle, maxPitchAngle));break;
            case BirdController._birdMovement.Descend: bird.Descend(Random.Range(minElevationChange, maxElevationChange), Random.Range(minPitchAngle, maxPitchAngle));break;
            case BirdController._birdMovement.BarrelRollDown: bird.BarrelRollLeft(Random.Range(minRollTime, maxRollTime)); break;
            case BirdController._birdMovement.BarrelRollUp: bird.BarrelRollRight(Random.Range(minRollTime, maxRollTime)); break;
            case BirdController._birdMovement.LoopDown: bird.LoopDLoopDown(Random.Range(minLoopTime, maxLoopTime)); break;
            case BirdController._birdMovement.LoopUp: bird.LoopDLoopUp(Random.Range(minLoopTime, maxLoopTime)); break;
            default: bird.Soar(); break;
        }
    }

    void ShowClouds()
    {
        CancelInvoke("ShowClouds");
        StartCoroutine(RunCloud());
        InvokeRepeating("ShowClouds", Random.Range(0.5f, 5f), 0f);
    }

    IEnumerator RunCloud()
    {
        GameObject cloud = movingClouds[Random.Range(0, movingClouds.Count)];
        Vector3 originalPos = cloud.transform.position;
        float cloudSpeed = Random.Range(minCloudSpeed, maxCloudSpeed);
        Vector3 lateralSpeed = new Vector3(-cloudSpeed, 0f, 0f);
        float distance = 0;
        cloud.transform.position = new Vector3(cloud.transform.position.x, cloud.transform.position.y + Random.Range(-1f, 1f), cloud.transform.position.z);
        while (distance < maxCloudDistance)
        {
            cloud.transform.position += lateralSpeed;
            distance += cloudSpeed;
            yield return null;
        }
        cloud.transform.position = originalPos;
    }
}
