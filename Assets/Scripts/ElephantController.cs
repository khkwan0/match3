using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantController : MonoBehaviour {

    private Animator anim;
    private int standingHash;
    private int hindUpHash;
    private int hindDownHash;
    private int scaredUpHash;
    private int scaredDownHash;
    private int walkingHash;
    private int lookAtCameraHash;
    private int lookBackToNormalHash;
    
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        standingHash = Animator.StringToHash("Standing");
        hindUpHash = Animator.StringToHash("HindUp");
        hindDownHash = Animator.StringToHash("HindDown");
        scaredUpHash = Animator.StringToHash("ScaredUp");
        scaredDownHash = Animator.StringToHash("ScaredDown");
        walkingHash = Animator.StringToHash("Walking");
        lookAtCameraHash = Animator.StringToHash("LookAtCamera");
        lookBackToNormalHash = Animator.StringToHash("LookBackToNormal");

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.W))
        {
            Walk();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Scare();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Hind();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Stop();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LookAtCamera();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            LookBackToNormal();
        }
	}

    void Walk()
    {
        anim.Play(walkingHash);
    }

    void Scare()
    {
        anim.Play(scaredUpHash);
    }

    public void Hind()
    {
        anim.Play(hindUpHash);
    }

    void Stop()
    {
        anim.Play(standingHash);
    }

    void LookAtCamera()
    {
        anim.Play(lookAtCameraHash);
    }

    void LookBackToNormal()
    {
        anim.Play(lookBackToNormalHash);
    }
}
