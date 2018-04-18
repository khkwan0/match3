using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorbikeController : MonoBehaviour {

    private Animator anim;
    private int goHash = Animator.StringToHash("Go");
    private int stopHash = Animator.StringToHash("stop");
    public float maxSpeed;
    public float minSpeed;
    [SerializeField]
    private float desiredSpeed;

    [SerializeField]
    private float currentSpeed;
    private Vector3 displacement;
    public float accelerate;
    public float decelerate;
    private bool turning;
    public float upperBounds;
    public float lowerBounds;

    private Vector3 startingPosition;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        currentSpeed = 0f;
        desiredSpeed = Random.Range(minSpeed, maxSpeed);
	}

    private void Start()
    {
        startingPosition = transform.position;
        Go(Random.Range(2f, 4f));
    }

    // Update is called once per frame
    void Update () {
        displacement = transform.forward * currentSpeed;
        transform.position += displacement;
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    TurnAround(true);
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    TurnAround(false);
        //}
	}

    public void ChangeSpeed()
    {
        desiredSpeed = Random.Range(minSpeed, maxSpeed);
        if (currentSpeed > desiredSpeed)
        {
            StartCoroutine(Accelerate(decelerate, "change"));
        } else
        {
            StartCoroutine(Accelerate(accelerate, "change"));
        }
    }

    public void Stop()
    {
        desiredSpeed = 0f;
        StartCoroutine(Accelerate(decelerate, "go"));
        
    }

    public void Go(float wait)
    {
        StartCoroutine(DoGo(wait));
    }

    IEnumerator DoGo(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        if (desiredSpeed == 0f)
        {
            desiredSpeed = Random.Range(minSpeed, maxSpeed);
        }
        StartCoroutine(Accelerate(accelerate, "stop"));
    }

    IEnumerator Accelerate(float dir, string previousState)
    {
        if (previousState == "stop")
        {
            anim.Play(goHash);
        }
        if (previousState == "go")
        {
            anim.Play(stopHash);
        }
        if (currentSpeed < desiredSpeed)
        {
            while (currentSpeed < desiredSpeed)
            {
                currentSpeed += dir;
                yield return null;
            }
        }
        if (currentSpeed > desiredSpeed)
        { 
            while (currentSpeed > desiredSpeed)
            {
                currentSpeed -= dir;
                yield return null;
            }
        }
        currentSpeed = desiredSpeed;
    }

    public void TurnAround(bool left)
    {
        StartCoroutine(DoTurn(left));
    }

    IEnumerator DoTurn(bool left)
    {
        while (turning)
        {
            yield return null;
        }
        turning = true;
        Vector3 oldRotation = transform.localEulerAngles;
        float dir = 1f;
        if (left)
        {
            dir = -1f;
        }

        float newY = transform.localEulerAngles.y + dir * 180f;
        //Quaternion newRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y + dir * 180f, transform.localRotation.eulerAngles.z);
        Vector3 newRotation = new Vector3(transform.localEulerAngles.x, newY, transform.localEulerAngles.z);
        int steps = 0;
        float perc = 0f;
        while (steps < 180)
        {
            perc = (float)steps / 180f;
            transform.localEulerAngles = Vector3.Lerp(oldRotation, newRotation, Mathf.SmoothStep(0f, 1f, perc));

            steps++;
            yield return null;
        }
        turning = false;
        //transform.position = new Vector3(transform.position.x, transform.position.y, startingPosition.z);
        transform.localEulerAngles = newRotation;
        ChangeSpeed();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.name == "left_wall")
        {
            if (transform.localPosition.z > upperBounds)
            {
                TurnAround(true);
            }
            else if (transform.localPosition.z < lowerBounds)
            {
                TurnAround(false);
            }
            else
            {
                TurnAround(Random.Range(0, 2) == 1 ? true : false);
            }
        }
        if (other.name == "right_wall")            
        {
            if (transform.localPosition.z > upperBounds)
            {
                TurnAround(false);
            }
            else if (transform.localPosition.z < lowerBounds)
            {
                TurnAround(true);
            }
            else
            {
                TurnAround(Random.Range(0, 2) == 1 ? true : false);
            }
        }
        if (other.name == "middle_trigger")
        {
            switch (Random.Range(0, 3))
            {
                case 0: break;
                case 1:
                    Stop();
                    Go(Random.Range(2f, 6f));
                    break;
                case 2:
                    {
                        //Debug.Log(Mathf.Round(transform.localEulerAngles.y));
                        if (Mathf.Round(transform.localEulerAngles.y) == 90f) // enter collider from th left
                        {
                            if (transform.localPosition.z > upperBounds)
                            {
                                TurnAround(false);
                            }
                            if (transform.localPosition.z < lowerBounds)
                            {
                                // turn left
                                TurnAround(true);
                            }
                        }
                        else
                        {
                            if (transform.localPosition.z > upperBounds)
                            {
                                // turn left
                                TurnAround(true);
                            }
                            if (transform.localPosition.z < lowerBounds)
                            {
                                // turn right
                                TurnAround(false);
                            }
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
