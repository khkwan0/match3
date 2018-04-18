using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThaiWomanSoccerBallSceneController : MonoBehaviour {

    public GameObject ball;
    public float minPower;
    public float maxPower;
    public GameObject humanoid;

	// Use this for initialization
	void Start () {
        SetBall();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoKick()
    {
        if (ball)
        {
            ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            ball.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * Random.Range(minPower, maxPower) + Vector3.up * Random.Range(minPower, maxPower));
            //ball.GetComponent<Rigidbody>().AddForce(-1f * 100f, 100f, 0f);
            GameController.GetGameController().PlayKickSound();
        }
        StartCoroutine(ResetBall());
    }

    public void SetBall()
    {
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        ball.transform.position = humanoid.transform.position + humanoid.transform.forward * 1.8f + transform.up * 1f;
        ball.transform.rotation = humanoid.transform.rotation;
    }

    IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(4f);
        SetBall();
    }
}
