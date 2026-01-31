using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCity : MonoBehaviour
{
	[SerializeField] private float xSizeCity = 20f;
	[SerializeField] private float zSizeCity = 20f;
	[Space]
	[SerializeField] private Material groundMaterial = null;
	[Space]
	[SerializeField] private TensorGrid tensorGrid = null;
	[SerializeField] private RoadGenerator roadGenerator = null;

	private IEnumerator Start()
	{
        yield return new WaitForSeconds(5f);
		CreateCity();
	}

	private void CreateCity()
	{
		//GenerateGround();
		GenerateRoad();
	}

	private void GenerateGround()
	{
		GameObject groundObj = Instantiate(new GameObject(), transform);
		groundObj.name = "Ground";
		
		Mesh mesh = new Mesh();
		mesh.Clear();

		Vector3[] vertices = 
		{
			new Vector3(-xSizeCity, 0, zSizeCity),
			new Vector3(xSizeCity, 0, zSizeCity),
			new Vector3(-xSizeCity, 0, -zSizeCity),
			new Vector3(xSizeCity, 0, -zSizeCity)
		};

		int[] triangles = 
		{
			0, 1, 2,
			3, 2, 1
		};

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		MeshFilter mf = groundObj.AddComponent(typeof(MeshFilter)) as MeshFilter;
		mf.mesh = mesh;

		MeshRenderer mr = groundObj.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		mr.material = groundMaterial;
	}

	private void GenerateRoad()
	{
        for (int i = 0; i < tensorGrid.TrackingMain.Count; i++)
        {
            roadGenerator.GenerateMesh(tensorGrid.TrackingMain[i].ToArray());
        }
	}

	private Transform[] TrackingRoad()
	{
		return null;
	}
}
