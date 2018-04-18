using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneHeadButt : MonoBehaviour {

    public float xVel, yVel;
    public GameObject head;
    public GameObject body;
    private Quaternion originalRot;
    public bool jump;

    private void Start()
    {
        if (!jump)
        {
            originalRot = head.transform.rotation;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(xVel, yVel, 0f);
        if (!jump)
        {
            StartCoroutine(HeadButt());
        }
    }

    IEnumerator HeadButt()
    {
        head.transform.Rotate(0f, 0f, 10f);
        yield return new WaitForSeconds(1);
        head.transform.rotation = originalRot;
    }
}
