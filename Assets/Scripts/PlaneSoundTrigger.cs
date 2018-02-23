using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSoundTrigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        GameController.GetGameController().PlayBoing();
    }
}
