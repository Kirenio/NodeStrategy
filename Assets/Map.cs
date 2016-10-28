using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
public class Map : MonoBehaviour {
    public float tileSize;
    public int numberOfTiles;
    Mesh mesh;

	// Use this for initialization
	void Awake () {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Map";

        Vector3[] vertices = new Vector3[(numberOfTiles + 1) * (numberOfTiles + 1)];
        Vector2[] uv = new Vector2[(numberOfTiles + 1) * (numberOfTiles + 1)];

        for (int i = 0, z = 0; z <= numberOfTiles; z++)
        {
            for (int x = 0; x <= numberOfTiles; x++, i++)
            {
                vertices[i] = new Vector3(x * tileSize, 0, z * tileSize);
                uv[i] = new Vector2(x / tileSize, z / tileSize);
            }
        }

        int[] triangles = new int[numberOfTiles * numberOfTiles * 6];

        for (int ti = 0, vi = 0, y = 0; y < numberOfTiles; y++, vi++)
        {
            for (int x = 0; x < numberOfTiles; x++, ti += 6, vi++)
            {
                // Forward polygon
                triangles[ti] = vi;
                triangles[ti + 1] = vi + numberOfTiles + 1;
                triangles[ti + 2] = vi + 1;
                // Backward polygon
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + numberOfTiles + 1;
                triangles[ti + 5] = vi + numberOfTiles + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        transform.position = new Vector3(-mesh.bounds.extents.x, 0, - mesh.bounds.extents.z);
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
}
