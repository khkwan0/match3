using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : MonoBehaviour {

    private Animator anim;
    private int upHash = Animator.StringToHash("Up");
    private int idleHash = Animator.StringToHash("Rest");
    private int hopHash = Animator.StringToHash("Hop");
    public float moveSpeed;
    public float turnTime;
    private bool turning;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        turning = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            Move(1f); 
        }
	}

    public void Up()
    {
        anim.Play(upHash);
    }

    public void Rest()
    {
        anim.Play(idleHash);
    }

    public void Hop()
    {
        anim.Play(hopHash);
    }
    public void Move(float distance)
    {        
        StartCoroutine(DoMove(distance * 2));
    }

    IEnumerator DoMove(float distance)
    {
        while (turning)
        {
            yield return null;
        }
        Vector3 originalPos = transform.position;
        Vector3 newPos = Vector3.right * Mathf.Cos(transform.rotation.eulerAngles.y) * distance + transform.position;
        float startTime = Time.time;
        float perc = 0f;
        while ((Time.time - startTime) * 2f < distance & !turning)  
        {
            Hop();
            perc = ((Time.time - startTime) * 2f / distance);
            transform.position = Vector3.Lerp(originalPos, newPos, Mathf.SmoothStep(0f, 1f, perc));
            yield return null;
        }
        Up();
    }

    public void TurnAround()
    {
        if (!turning)
        {
            StartCoroutine(DoTurn(180f, turnTime));
        }
    }

    IEnumerator DoTurn(float angle, float time)
    {
        turning = true;
        float startTime = Time.time;
        float perc = 0;
        Quaternion originalRot = transform.rotation;
        Vector3 angles = transform.rotation.eulerAngles;
        Quaternion newRot = Quaternion.Euler(angles.x, angles.y + angle, angles.z);
        while ((Time.time - startTime) < time)
        {
            Hop();
            perc = (Time.time - startTime) / time;
            transform.rotation = Quaternion.Lerp(originalRot, newRot, Mathf.SmoothStep(0f, 1f, perc));
            yield return null;
        }
        turning = false;
    }

}
