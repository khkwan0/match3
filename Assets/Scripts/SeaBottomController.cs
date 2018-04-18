using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaBottomController : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "fish")
        {
            Destroy(other.gameObject);
        }
    }
}
