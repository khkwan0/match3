using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSceneController : MonoBehaviour {

    public List<GameObject> rats;

    private void Start()
    {
        for (int i = 0; i < rats.Count; i++)
        {
            InvokeRepeating("MoveRat0", Random.Range(1f, 4f), 10f);
            InvokeRepeating("MoveRat1", Random.Range(1f, 4f), 10f);
        }
    }

    private void MoveRat0()
    {
        StartCoroutine(DoMove(rats[0]));
    }

    private void MoveRat1()
    {
        StartCoroutine(DoMove(rats[1]));
    }

    IEnumerator DoMove(GameObject rat)
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));
        rat.GetComponent<RatController>().Move(Random.Range(0.5f, 1f));
    }


}
