using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiteTailScript : MonoBehaviour {

    public GameObject linkPrefab;
    public int maxLinks;
    public Vector2 windForce = new Vector2();

    private GameObject last;
	// Use this for initialization
	void Start () {
        float yDisplacement = linkPrefab.GetComponent<CapsuleCollider>().height * linkPrefab.transform.localScale.y;

        GameObject previous = null;
        GameObject go = null;
        for (int i = 0; i < maxLinks; i++)
        {
            go = GameObject.Instantiate(linkPrefab, new Vector3(transform.position.x, -yDisplacement * i + transform.position.y, 0f), Quaternion.identity, transform);          
            //go = GameObject.Instantiate(linkPrefab, Vector3.zero, transform.rotation, transform);
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = false;
            go.GetComponent<Rigidbody>().mass = 0.1f;
            go.AddComponent<HingeJoint>();

            go.GetComponent<HingeJoint>().axis = new Vector3(0f, 0f, 1f);
            if (!previous)
            {
                go.GetComponent<HingeJoint>().connectedBody = transform.gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                go.GetComponent<HingeJoint>().connectedBody = previous.GetComponent<Rigidbody>();
            }



            previous = go;
        }
        last = go;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        last.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(windForce.x, windForce.y), 0f, 0f));
	}
}
