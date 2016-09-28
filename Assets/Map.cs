using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Map : MonoBehaviour {
    public int numberofProvences;

	// Use this for initialization
	void Start () {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        mesh.vertices = new Vector3[]
        {
            new Vector3(0,0,3),
            new Vector3(2,0,4),
            new Vector3(1,0,-1),
            new Vector3(-2,0,0)
        };
        mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
	}
	
    int[] GetZonePoints(int amount)
    {
        int[] result = new int[amount];
        for (int i = 0; i < amount; i++)
        {
            result[i] = Random.Range(1, 10);
        }
        return result;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
