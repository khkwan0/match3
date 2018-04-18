using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour {

    private Animator anim;
    int soarHash = Animator.StringToHash("Soar");
    int flappingHash = Animator.StringToHash("Flapping");
    // Use this for initialization

    public float elevationDeltaChangeTime;
    public bool checkBounds = true;
    public bool rotate_90 = true;
    public enum _birdMovement { Soar, Flapping, Forward, Backward, Ascend, Descend, BarrelRollUp, BarrelRollDown, LoopUp, LoopDown, Count };

    void Start () {
        anim = gameObject.GetComponent<Animator>();
        InvokeRepeating("PlayFlap", Random.Range(2f, 5f), Random.Range(2f, 5f));
    }

    public void Soar()
    {
        anim.Play(soarHash);
    }

    public void PlayFlap()
    {
        anim.Play(flappingHash);
    }

    public void LoopDLoopUp(float loopTime)
    {
        StartCoroutine(DoLoop(true, loopTime));
    }

    public void LoopDLoopDown(float loopTime)
    {
        StartCoroutine(DoLoop(false, loopTime));
    }

    public void MoveForward(float lateralTime, float lateralSpeed)
    {
        StartCoroutine(DoLateral(lateralTime, lateralSpeed, true));
        PlayFlap();
    }

    public void MoveBackward(float lateralTime, float lateralSpeed)
    {
        StartCoroutine(DoLateral(lateralTime, lateralSpeed, false));
        PlayFlap();
    }

    IEnumerator DoLateral(float lateralTime, float lateralSpeed, bool forward)
    {
        float perc = 0f;
        float startTime = Time.time;
        float dir = forward ? 1f : -1f;
        Vector3 originalPos = transform.position;
        Vector3 newPos = new Vector3(transform.position.x + dir * (lateralSpeed * lateralTime), transform.position.y, transform.position.z);
        while ((Time.time - startTime) < lateralTime)
        {
            perc = (Time.time - startTime) / lateralTime;
            transform.position = Vector3.Lerp(originalPos, newPos, Mathf.SmoothStep(0f, 1f, perc));
            yield return null;
        }
        PlayFlap();
        if (transform.position.x > 2.9f)
        {
            MoveBackward(2f, 1f);
        }
        if (transform.position.x < -2.9f)
        {
            MoveForward(2f, 1f);
        }
    }
    IEnumerator DoLoop(bool up, float loopTime)
    {
        float perc = 0f;
        float startTime = Time.time;
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;
        float dir = up ? 1f : -1f;
        while ((Time.time - startTime) < loopTime)
        {
            perc = (Time.time - startTime) / loopTime;
            transform.rotation = Quaternion.Euler(dir * -360f * perc, 90f, 0f);
            transform.position = new Vector3(originalPos.x + Mathf.Sin(perc * 6.28f), originalPos.y + dir * 2 * Mathf.Sin(perc * 3.14f), originalPos.z);

            yield return null;
        }
        transform.position = originalPos;
        transform.rotation = originalRot;
        PlayFlap();
    }

    public void BarrelRollLeft(float rollTime)
    {
        StartCoroutine(DoBarrelRoll(true, rollTime));
    }

    public void BarrelRollRight(float rollTime)
    {
        StartCoroutine(DoBarrelRoll(false, rollTime));
    }

    IEnumerator DoBarrelRoll(bool left, float rollTime)
    {
        float perc = 0f;
        float startTime = Time.time;
        float dir = 1f;
        if (!left)
        {
            dir = -1f;
        }
        while ((Time.time - startTime) < rollTime)
        {
            perc = (Time.time - startTime) / rollTime;
            transform.rotation = Quaternion.Euler(0f, 90f, dir * 360f * perc);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        PlayFlap();
    }

    public void Ascend(float elevationChange, float pitchAngle)
    {
        StartCoroutine(DoAscend(true, elevationChange, pitchAngle));

    }

    public void Descend(float elevationChange, float pitchAngle) 
    {
        StartCoroutine(DoAscend(false, elevationChange, pitchAngle));
    }

    public void Descend(float elevationChange, float pitchAngle, float forwardVel)
    {
        StartCoroutine(DoAscend(false, elevationChange, pitchAngle, forwardVel));
    }

    IEnumerator DoAscend(bool ascend, float elevationChange, float pitchAngle)
    {
        float perc;
        float startTime = Time.time;
        float dir = ascend ? 1f : -1f;
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + dir * elevationChange, transform.position.z);
        Vector3 originalPosition = transform.position;

        while ((Time.time - startTime) < elevationDeltaChangeTime)
        {
            perc = (Time.time - startTime) / elevationDeltaChangeTime;
            if (rotate_90)
            {
                transform.rotation = Quaternion.Euler(-dir * pitchAngle * Mathf.Sin(3.14f * perc), 90f, 0f);
            } else
            {
                transform.rotation = Quaternion.Euler(-dir * pitchAngle * Mathf.Sin(3.14f * perc), 0f, 0f);
            }
            transform.position = Vector3.Lerp(originalPosition, newPosition, Mathf.SmoothStep(0f, 1f, perc));
            yield return null;
        }
        if (rotate_90)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        } else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        PlayFlap();
        if (checkBounds)
        {
            if (transform.position.y > 4f)
            {
                Descend(1f, 30f);
            }
            if (transform.position.y < 2f)
            {
                Ascend(1f, 30f);
            }
        }
    }

    IEnumerator DoAscend(bool ascend, float elevationChange, float pitchAngle, float forwardVel)
    {
        float perc;
        float startTime = Time.time;
        float dir = ascend ? 1f : -1f;
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + dir * elevationChange, transform.position.z + (100f * forwardVel * elevationDeltaChangeTime));
        Vector3 originalPosition = transform.position;
            
        while ((Time.time - startTime) < elevationDeltaChangeTime)
        {
            perc = (Time.time - startTime) / elevationDeltaChangeTime;
            if (rotate_90)
            {
                transform.rotation = Quaternion.Euler(-dir * pitchAngle * Mathf.Sin(3.14f * perc), 90f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(-dir * pitchAngle * Mathf.Sin(3.14f * perc), 0f, 0f);
            }
            transform.position = Vector3.Lerp(originalPosition, newPosition, Mathf.SmoothStep(0f, 1f, perc));
            yield return null;
        }
        if (rotate_90)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        PlayFlap();
        if (checkBounds)
        {
            if (transform.position.y > 4f)
            {
                Descend(1f, 30f);
            }
            if (transform.position.y < 2f)
            {
                Ascend(1f, 30f);
            }
        }
    }
}
