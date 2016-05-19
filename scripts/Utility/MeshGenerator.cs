using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshGenerator {

	public static void AddMesh(Mesh target, Mesh add, Vector3 position, Quaternion rotation)
	{
		List<Vector3> vertices = new List<Vector3>(target.vertices);
		List<Vector2> uvs = new List<Vector2>(target.uv);
		List<int> triangles = new List<int>(target.triangles);

		int offset = vertices.Count;

		List<Vector3> nVertices = new List<Vector3>(add.vertices);
		List<Vector2> nUvs = new List<Vector2>(add.uv);
		List<int> nTriangles = new List<int>(add.triangles);

		for(int i = 0; i < nVertices.Count; i++)
		{
			Vector3 vertex = nVertices[i];
			// Rotate
			vertex = rotation*vertex;

			// Offset by transform
			vertex = vertex + position;

			nVertices[i] = vertex;
		}

		// Offset the new triangles
		for(int i = 0; i < nTriangles.Count; i++)
		{
			nTriangles[i] = nTriangles[i] + offset;
		}

		vertices.AddRange (nVertices);
		uvs.AddRange(nUvs);
		triangles.AddRange (nTriangles);

		// Update the original mesh

		target.vertices = vertices.ToArray();
		target.triangles = triangles.ToArray();
		target.uv = uvs.ToArray();

		target.Optimize();
		target.RecalculateNormals();
	}

	public static void AddBox(Mesh target, Vector3 origin, Vector3 width, Vector3 length, Vector3 height)
	{
		AddMesh (target, MeshGenerator.GenerateBox(origin, width, length, height), Vector3.zero, Quaternion.identity);
	}

	public static Mesh GenerateBox(Vector3 origin, Vector3 width, Vector3 length, Vector3 height)
	{
		Mesh box = new Mesh();

		List<Vector3> vertices = new List<Vector3>();

		List<Vector2> uvCoords = new List<Vector2>();
		
		vertices.Add (origin); // 0
		uvCoords.Add (new Vector2(0, 0));
		vertices.Add (origin + length); // 1
		uvCoords.Add (new Vector2(1,0));
		vertices.Add (origin + height); // 2
		uvCoords.Add (new Vector2(0,1));
		vertices.Add (origin + length + height); // 3
		uvCoords.Add (new Vector2(1,1));
		vertices.Add (origin + width); // 4
		uvCoords.Add (new Vector2(1,0));
		vertices.Add (origin + width + height); // 5
		uvCoords.Add (new Vector2(1,1));
		vertices.Add (origin + length + width); // 6
		uvCoords.Add (new Vector2(1,0));
		vertices.Add (origin + length + width + height); // 7
		uvCoords.Add (new Vector2(1,1));
		
		List<int> triangles = new List<int>();
		
		triangles.Add (0);
		triangles.Add (1);
		triangles.Add (2);
		
		triangles.Add (1);
		triangles.Add (3);
		triangles.Add (2);
		
		triangles.Add (2);
		triangles.Add (3);
		triangles.Add (5);
		
		triangles.Add (3);
		triangles.Add (7);
		triangles.Add (5);
		
		triangles.Add (1);
		triangles.Add (6);
		triangles.Add (3);
		
		triangles.Add (6);
		triangles.Add (7);
		triangles.Add (3);
		
		triangles.Add (5);
		triangles.Add (6);
		triangles.Add (4);
		
		triangles.Add (6);
		triangles.Add (5);
		triangles.Add (7);
		
		triangles.Add (4);
		triangles.Add (1);
		triangles.Add (0);
		
		triangles.Add (1);
		triangles.Add (4);
		triangles.Add (6);
		
		triangles.Add (2);
		triangles.Add (4);
		triangles.Add (0);
		
		triangles.Add (4);
		triangles.Add (2);
		triangles.Add (5);
		
		box.vertices = vertices.ToArray();
		box.uv = uvCoords.ToArray(); 
		box.triangles = triangles.ToArray();

		box.Optimize();
		box.RecalculateNormals();

		return box;

	}
}
