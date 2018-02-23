using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsPanelController : MonoBehaviour {

    public GameObject bombPrefab;
    public GameObject rainbowPrefab;
    public GameObject hammerPrefab;
    public GameObject horizontalPrefab;
    public GameObject verticalPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnReward(List<Rewards> rewards)
    {
        GameObject toSpawn = null;
        for (int i = 0; i < rewards.Count; i++)
        {
            switch(rewards[i].reward)
            {
                case "bomb": toSpawn = bombPrefab; break;
                case "rainbow": toSpawn = rainbowPrefab; break;
                case "hammer": toSpawn = hammerPrefab; break;
                case "vertical": toSpawn = verticalPrefab; break;
                case "horizontal": toSpawn = horizontalPrefab; break;
                default:break;
            }
            if (toSpawn)
            {
                GameObject go = GameObject.Instantiate(toSpawn, transform);
                go.transform.position += new Vector3(1f, 0f, -3f);
                go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            }
        }
    }
}
