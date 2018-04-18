using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FishController : MonoBehaviour {

    // Use this for initialization
    private Animator anim;
    int slowHash = Animator.StringToHash("Slow");
    int mediumHash = Animator.StringToHash("Medium");
    int fastHash = Animator.StringToHash("Fast");
    
    public enum _direction { left, right };
    public _direction direction;

    public enum _speed { slow, medium, fast };
    public _speed speed;

    [SerializeField]
    private Vector3 directionVector;
    [SerializeField]
    private float velocityMagnitude;
    float randomOffset;
    [Range(0f, 0.001f)]
    public float maxAmplitude;
    [SerializeField]
    private float randomAmplitude;
    float depth;
    private bool canRise;
    [Range(0f, 0.2f)]
    public float riseAmount;

    private bool outOfWater;

    public bool OutOfWater
    {
        get { return outOfWater; }
        set { outOfWater = value; }
    }

    public bool CanRise
    {
        get { return canRise; }
        set { canRise = value; }
    }

    public float Amplitude
    {
        get { return randomAmplitude; }
        set { randomAmplitude = value; }
    }

    private void Awake()
    {
        canRise = false;
        outOfWater = false;
    }

    void Start()
    {
        StartFish();
    }

    public void Rise()
    {
        transform.position += new Vector3(0f, riseAmount, 0f);
    }

	public void StartFish ()
    {
        ChooseStartDirection();
        ChooseStartSpeed();
        depth = UnityEngine.Random.Range(-2.5f, 0.5f);
        anim = GetComponent<Animator>();
        SetDirection();
        SetAnimation();
        GetComponent<Rigidbody>().velocity = directionVector * velocityMagnitude;
        randomOffset = UnityEngine.Random.Range(0f, 1f);
        randomAmplitude = UnityEngine.Random.Range(0f, maxAmplitude);
        transform.position += new Vector3(UnityEngine.Random.Range(-2f, 2f), depth, 0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += new Vector3(0f,randomAmplitude * Mathf.Sin(Time.time + randomOffset), 0f);        
    }

    public void Turn()
    {
        direction = (direction == _direction.right) ? _direction.left : _direction.right;
        if (direction == _direction.right)
        {
            directionVector = Vector3.right;
        }
        else
        {
            directionVector = -Vector3.right;
        }
        GetComponent<Rigidbody>().velocity = directionVector * velocityMagnitude;

    }

    private void ChooseStartDirection()
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            direction = _direction.right;
        } else
        {
            direction = _direction.left;
            Turn();
        }
    }

    private void ChooseStartSpeed()
    {
        velocityMagnitude = UnityEngine.Random.Range(0.1f, 3f);
    }

    private void SetDirection()
    {
        if (direction == _direction.right)
        {
            directionVector = Vector3.right;
        } else
        {
            directionVector = Vector3.left;
        }
    }

    private void SetAnimation()
    {
        float speed = velocityMagnitude;
        if (speed < 1f)
        {
            anim.Play(slowHash);
        }
        else if (speed < 2f)
        {
            anim.Play(mediumHash);
        }
        else
        {
            anim.Play(fastHash);
        }
    }

}
