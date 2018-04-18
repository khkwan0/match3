using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour {

    private NavMeshAgent agent;

	void Start () {
        agent = GetComponent<NavMeshAgent>();
        //InvokeRepeating("Move", 0f, 2f);
	}

	void Move () {
        //agent.Warp(transform.position);

	}

    private void Update()
    {
        agent.destination = transform.position + (transform.forward * .01f);
    }
}
