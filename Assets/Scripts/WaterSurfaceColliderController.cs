using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurfaceColliderController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "fish")
        {
            GameObject fish = GameObject.Instantiate(other.gameObject, other.gameObject.transform.position, other.gameObject.transform.rotation);
            fish.GetComponent<FishController>().OutOfWater = true;
            fish.GetComponent<Rigidbody>().AddTorque(0f, 0f, -2000f);
            fish.GetComponent<Rigidbody>().velocity = new Vector3(3f, 8f, 0f);
            fish.GetComponent<Rigidbody>().useGravity = true;
            fish.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            fish.transform.Find("Bubbles").gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}
