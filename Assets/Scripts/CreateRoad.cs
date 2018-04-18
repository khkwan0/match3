using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoad : MonoBehaviour {

    public float width = 50f;
    public float height = 200f;

	// Use this for initialization
	void Start() {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        // Vertices
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width/2, 0, 0),
            new Vector3(width/2, 0, 0),
            new Vector3(width/2, 0, height),
            new Vector3(-width/2, 0, height)
        };

        // Triangles
        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 3;
        tri[2] = 1;

        tri[3] = 3;
        tri[4] = 2;
        tri[5] = 1;

        // Normals
        Vector3[] normals = new Vector3[4];
        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;

        // UVs
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(0, 1);

        // assign arrays;
        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;

    }

}
