using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowTileController : MonoBehaviour {

    public List<Sprite> sprites = new List<Sprite>();
    private List<GameObject> tiles = new List<GameObject>();
    bool render = true;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < sprites.Count; i++)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = sprites[i];
            go.GetComponent<SpriteRenderer>().enabled = false;
            go.transform.localPosition = new Vector3(0f, 0f, 0f);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            tiles.Add(go);
            
            //renderedTiles.Add(GameObject.Instantiate(tiles[i], transform));
        }
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        if (render)
        {
            tiles[(int)Mathf.Floor(Time.timeSinceLevelLoad) % tiles.Count].GetComponent<SpriteRenderer>().enabled = true;
        }
	}

    public void DisableAllRendering()
    {
        render = false;
    }
}
