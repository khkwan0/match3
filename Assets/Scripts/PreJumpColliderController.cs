using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreJumpColliderController : MonoBehaviour {

    public GameObject obj;

    private void OnTriggerEnter(Collider other)
    {
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 2f, 0f);
    }
}
