using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookColliderController : MonoBehaviour {

    public GameObject hook;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "fish" && !other.gameObject.GetComponent<FishController>().OutOfWater)
        {
            other.gameObject.GetComponent<FishController>().enabled = false;
            other.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
            other.transform.SetParent(hook.transform);
            hook.GetComponent<Rigidbody2D>().velocity = new Vector2(-30f, 8f);
            //pole.GetComponent<Rigidbody2D>().AddTorque(10f)           
        }
    }
}
