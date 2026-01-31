using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
	[SerializeField] private float widhtRoad = 4f;

	[SerializeField] private Transform[] path = null;

	private MeshFilter meshFilter = null;

	public void GenerateMesh(Transform[] path)
	{
		Vector3[] verts = new Vector3[path.Length * 2];
		int[] tris = new int[((path.Length - 1) * 3) * 2];
		Vector3[] normals = new Vector3[path.Length * 2];
		
		int vertIndex = 0;
		int trisIndex = 0;
		for(int i = 0; i < path.Length; i++)
		{
			Vector3 posPointLeft = new Vector3(path[i].position.x + (widhtRoad / 2), 0f, path[i].position.z);
			Vector3 posPointRight = new Vector3(path[i].position.x - (widhtRoad / 2), 0f, path[i].position.z);

			verts[vertIndex] = RotatePointAroundPivot(posPointLeft, path[i].position, path[i].rotation.eulerAngles);
			verts[vertIndex + 1] = RotatePointAroundPivot(posPointRight, path[i].position, path[i].rotation.eulerAngles);
			
			if(i < path.Length -1)
			{
				tris[trisIndex] = vertIndex;
				++trisIndex;
				tris[trisIndex] = vertIndex + 2;
				++trisIndex;
				tris[trisIndex] = vertIndex + 1;
				
				++trisIndex;
				tris[trisIndex] = vertIndex + 2;
				++trisIndex;
				tris[trisIndex] = vertIndex + 3;
				++trisIndex;
				tris[trisIndex] = vertIndex + 1;
			}

			vertIndex += 2;
			trisIndex++;
		}

		for(int i = 0; i < normals.Length; i++)
		{
			normals[i] = -Vector3.forward;
		}

		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.normals = normals;
		mesh.RecalculateNormals();

        GameObject obj = new GameObject();

		meshFilter = obj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshFilter.mesh = mesh;

        obj.AddComponent(typeof(MeshRenderer));
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
    		return Quaternion.Euler(angles) * (point - pivot) + pivot;
  	}
}
