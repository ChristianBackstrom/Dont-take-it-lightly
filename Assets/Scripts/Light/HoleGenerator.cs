using System;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using static UnityEngine.Mesh;

[RequireComponent(typeof(MeshFilter))]
public class HoleGenerator : MonoBehaviour
{
	[SerializeField]
	private Vector3 size;

	[SerializeField]
	private float radius;

	private List<Vector3> vertices = new List<Vector3>();

	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private int radialPoints = 8;

	public Vector3 Point { get; set; }
	public Vector3 HitNormal { get; set; }

	private void OnValidate()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();
	}

	private void Start()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();
		Generate();
	}


	[ContextMenu("Generate")]
	public void Generate()
	{
		vertices = new List<Vector3>();

		for (int z = 0; z < 2; z++)
		{
			for (int y = 0; y < 2; y++)
			{
				if (y == 0)
				{
					for (int x = 0; x < 2; x++)
					{
						vertices.Add(new Vector3(x * size.x, y * size.y, z * size.z));
					}
				}
				else
				{
					for (int x = 1; x >= 0; x--)
					{
						vertices.Add(new Vector3(x * size.x, y * size.y, z * size.z));
					}
				}
			}
		}

		MakeCube();
	}

	private void MakeCube()
	{
		MeshData meshData = new MeshData(vertices.ToArray());
		// Top
		meshData.AddTriangle(3, 7, 2);
		meshData.AddTriangle(7, 6, 2);

		// Bot
		meshData.AddTriangle(5, 4, 1);
		meshData.AddTriangle(4, 0, 1);

		// Side 1 
		meshData.AddTriangle(2, 5, 1);
		meshData.AddTriangle(6, 5, 2);

		// Side 2
		meshData.AddTriangle(7, 3, 0);
		meshData.AddTriangle(7, 0, 4);

		// Side 2
		meshData.AddTriangle(0, 2, 1);
		meshData.AddTriangle(0, 3, 2);

		// Side 2
		meshData.AddTriangle(4, 5, 6);
		meshData.AddTriangle(4, 6, 7);

		Mesh mesh = meshData.CreateMesh(false);
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}

	public void MakeHole(GameObject gameObject)
	{
		Generate();

		Point = transform.transform.transform.transform.transform.transform.InverseTransformPoint(Point);
		print("Making hole");
		if (HitNormal == Vector3.forward)
		{
			HitNormal = HitNormal + new Vector3(.001f, 0, 0);
			HitNormal = HitNormal.normalized;
		}

		Vector3 left = (Quaternion.AngleAxis(90, Vector3.forward) * HitNormal).normalized;
		Vector3 up = Vector3.Cross(HitNormal, left).normalized;
		left = Vector3.Cross(up, HitNormal).normalized;

		for (int i = 0; i < 2; i++)
		{
			Vector3 midPoint = Point + Vector3.forward * i * size.z;

			for (int g = 0; g < radialPoints; g++)
			{
				float radialPercent = (float)g / radialPoints * 2 * Mathf.PI;
				Vector3 localPos = new Vector3(Mathf.Cos(radialPercent), Mathf.Sin(radialPercent), 0) * radius;

				Vector3 pos = midPoint + up * localPos.y + left * localPos.x;
				vertices.Add(pos);
			}
		}

		MeshData meshData = new MeshData(vertices.ToArray());

		// Top
		meshData.AddTriangle(3, 7, 2);
		meshData.AddTriangle(7, 6, 2);

		// Bot
		meshData.AddTriangle(5, 4, 1);
		meshData.AddTriangle(4, 0, 1);

		// Side 1 
		meshData.AddTriangle(2, 5, 1);
		meshData.AddTriangle(6, 5, 2);

		// Side 2
		meshData.AddTriangle(7, 3, 0);
		meshData.AddTriangle(7, 0, 4);

		for (int side = 0; side < 2; side++)
		{
			for (int i = 0; i < 4; i++)
			{
				int index = i + side * 4;
				if ((index + 1) % 4 == 0)
				{
					if (side > 0)
					{
						meshData.AddTriangle(index, side * radialPoints + 8, index * 2 + radialPoints + 1);
						meshData.AddTriangle(index, side * radialPoints + 8 + 1, side * radialPoints + 8);
						meshData.AddTriangle(index, index - 3, side * radialPoints + 8 + 1);
					}
					else
					{
						meshData.AddTriangle(index, index * 2 + radialPoints + 1, side * radialPoints + 8);
						meshData.AddTriangle(index, side * radialPoints + 8, side * radialPoints + 8 + 1);
						meshData.AddTriangle(index, side * radialPoints + 8 + 1, index - 3);
					}

					continue;
				}

				if (side > 0)
				{
					meshData.AddTriangle(index, index * 2 + radialPoints + 2, index * 2 + radialPoints + 1);
					meshData.AddTriangle(index, index * 2 + radialPoints + 3, index * 2 + radialPoints + 2);
					meshData.AddTriangle(index, index + 1, index * 2 + radialPoints + 3);
				}
				else
				{
					meshData.AddTriangle(index, index * 2 + radialPoints + 1, index * 2 + radialPoints + 2);
					meshData.AddTriangle(index, index * 2 + radialPoints + 2, index * 2 + radialPoints + 3);
					meshData.AddTriangle(index, index * 2 + radialPoints + 3, index + 1);
				}
			}
		}

		for (int i = 8; i < 8 + radialPoints; i++)
		{
			if (i == 7 + radialPoints)
			{
				meshData.AddTriangle(i, i + radialPoints, i + 1);
				meshData.AddTriangle(i, i + 1, i - radialPoints + 1);
				continue;
			}

			meshData.AddTriangle(i, i + radialPoints + 1, i + 1);
			meshData.AddTriangle(i, i + radialPoints, i + radialPoints + 1);
		}

		Mesh mesh = meshData.CreateMesh(false);
		meshCollider.sharedMesh = mesh;
		meshFilter.sharedMesh = mesh;
	}
#if (UNITY_EDITOR)
	private void OnDrawGizmos()
	{
		Handles.matrix = transform.localToWorldMatrix;

		for (int i = 0; i < vertices.Count; i++)
		{
			Handles.Label(vertices[i], i.ToString());
		}
	}
#endif
}
