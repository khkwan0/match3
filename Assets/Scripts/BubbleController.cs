using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hit");
    }
}
