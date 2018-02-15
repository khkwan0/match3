﻿using System.Collections;
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
    float randomAmplitude;
    float depth;

	public void StartFish ()
    {
        ChooseStartDirection();
        ChooseStartSpeed();
        depth = UnityEngine.Random.Range(-4f, 1f);
        anim = GetComponent<Animator>();
        SetDirection();
        SetAnimation();
        GetComponent<Rigidbody>().velocity = directionVector * velocityMagnitude;
        randomOffset = UnityEngine.Random.Range(0f, 1f);
        randomAmplitude = UnityEngine.Random.Range(0f, 0.01f);
        transform.position += new Vector3(UnityEngine.Random.Range(-2f, 2f), depth, 0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += new Vector3(0f,Mathf.Sin(Time.time + randomOffset)*randomAmplitude, 0f);        
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