using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController2 : MonoBehaviour {

    private Vector3 moveVector = new Vector3(0f, 0f, 0f);
    public float forwardSpeed;
    public GameObject water;
    public float levelOutThreshhold;
    [SerializeField]
    private float groundAltitude;
    [SerializeField]
    private float ratio;
    private float xrot;

    private Animator anim;
    private int flapHash = Animator.StringToHash("Flapping");
    private float originalXrot;
    private Vector3 originalPos;
    private Quaternion originalRot;

    public float immelmanTime;

    [SerializeField]
    private float distanceTravelled;

    public float maxDistance;

    public GameObject roosterTail;

    private ParticleSystem.EmissionModule em;
    // Use this for initialization
    void Start () {
        originalPos = transform.position;
        originalRot = transform.rotation;
        originalXrot = transform.rotation.eulerAngles.x;
        InvokeRepeating("DoFlap", 1f, 2f);
        anim = GetComponent<Animator>();
        distanceTravelled = 0f;
        em = roosterTail.GetComponent<ParticleSystem>().emission;
        em.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition += transform.forward * forwardSpeed;
        distanceTravelled += forwardSpeed;
        groundAltitude = transform.position.y - water.transform.position.y - 1.5f;
        ratio = groundAltitude / levelOutThreshhold;

        if (groundAltitude < levelOutThreshhold)
        {
            xrot = originalXrot * ratio;
            float yrot = transform.rotation.eulerAngles.y;
            float zrot = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(new Vector3(xrot, yrot, zrot));

            if (groundAltitude < 1.0f && !em.enabled)
            {

                em.enabled = true;
            }
            if (groundAltitude > 1.0f && em.enabled)
            {
                em.enabled = false;
            }
                
        }
        if (distanceTravelled > maxDistance)
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
            distanceTravelled = 0f;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "immelmanCollider")
        {
            float dot = Vector3.Dot(transform.forward, other.transform.forward);
            if (dot > 0f)
            {
                if (Random.Range(0, 2) == 0)
                {
                    StartCoroutine(DoImmelman());
                }
                else
                {
                    StartCoroutine(RollRight());
                }
            }
        }
    }

    private void DoFlap()
    {
        anim.Play(flapHash);
    }

    IEnumerator DoImmelman()
    {
        float startTime = Time.time;
        float perc;
        Quaternion originalRot = transform.rotation;
        Quaternion finalRot = Quaternion.Euler(originalRot.eulerAngles.x - 179f, originalRot.eulerAngles.y, originalRot.eulerAngles.z);
        while ((Time.time - startTime) < immelmanTime)
        {
            perc = (Time.time - startTime) / immelmanTime;
            transform.rotation = Quaternion.Slerp(originalRot, finalRot, perc);
            yield return null;
        }
        startTime = Time.time;
        originalRot = transform.rotation;
        finalRot = Quaternion.Euler(originalRot.eulerAngles.x, originalRot.eulerAngles.y, originalRot.eulerAngles.z - 179f);
        float _immelmanTime = immelmanTime * 5;
        while ((Time.time - startTime) < _immelmanTime)
        {
            perc = (Time.time - startTime) / _immelmanTime;
            transform.rotation = Quaternion.Slerp(originalRot, finalRot, perc);
            yield return null;
        }

        transform.rotation = finalRot;
    }

    IEnumerator RollRight()
    {
        float rollTime = 3.0f;
        float startTime = Time.time;
        float perc;
        Quaternion originalRot = transform.rotation;
        Quaternion finalRot = Quaternion.Euler(transform.rotation.eulerAngles.x - 179f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 90f);
        while ((Time.time - startTime) < rollTime)
        {
            perc = (Time.time - startTime) / rollTime;
            transform.rotation = Quaternion.Slerp(originalRot, finalRot, perc);
            yield return null;
        }
        originalRot = transform.rotation;
        finalRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 90f);
        startTime = Time.time;
        while ((Time.time - startTime) < rollTime)
        {
            perc = (Time.time - startTime) / rollTime;
            transform.rotation = Quaternion.Slerp(originalRot, finalRot, perc);
            yield return null;
        }
    }
}
