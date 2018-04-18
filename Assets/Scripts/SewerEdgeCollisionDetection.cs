using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerEdgeCollisionDetection : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Col");
        other.GetComponent<RatController>().TurnAround();
    }
}
