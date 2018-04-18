using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollisionDetection : MonoBehaviour {
  
    public void OnTriggerEnter(Collider other)
    {
        other.transform.Rotate(new Vector3(0f, 180f, 0f));
        other.GetComponent<FishController>().Turn();
        if (other.GetComponent<FishController>().CanRise)
        {
            other.GetComponent<FishController>().Rise();
        }
    }
}
