using Godot;
using System.Collections.Generic;

public class PatchGenerator
{
	public ArrayMesh CreateTubeMesh(List<Vector3> points, float radius, int segments)
	{
		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
		
		// 生成管状网格的顶点数据
		var vertices = GenerateTubeVertices(points, radius, segments);
		var indices = GenerateTubeIndices(points.Count, segments);
		
		// 添加顶点到SurfaceTool
		for (int i = 0; i < vertices.Count; i++)
		{
			surfaceTool.SetNormal(CalculateNormal(vertices, i));
			surfaceTool.AddVertex(vertices[i]);
		}
		
		// 添加三角形索引
		for (int i = 0; i < indices.Count; i += 3)
		{
			surfaceTool.AddIndex(indices[i]);
			surfaceTool.AddIndex(indices[i + 1]);
			surfaceTool.AddIndex(indices[i + 2]);
		}
		
		return surfaceTool.Commit(new ArrayMesh());
	}
	
	private List<Vector3> GenerateTubeVertices(List<Vector3> points, float radius, int segments)
	{
		var vertices = new List<Vector3>();
		
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 direction = (i < points.Count - 1) ? 
				(points[i + 1] - points[i]).Normalized() : 
				(points[i] - points[i - 1]).Normalized();
			
			// 计算每个点的截面圆环
			var circle = CalculateCirclePoints(points[i], direction, radius, segments);
			vertices.AddRange(circle);
		}
		
		return vertices;
	}
	
	private List<int> GenerateTubeIndices(int pointCount, int segments)
	{
		var indices = new List<int>();
		
		for (int i = 0; i < pointCount - 1; i++)
		{
			for (int j = 0; j < segments; j++)
			{
				int current = i * segments + j;
				int next = i * segments + (j + 1) % segments;
				int nextPoint = (i + 1) * segments + j;
				int nextPointNext = (i + 1) * segments + (j + 1) % segments;
				
				// 添加两个三角形构成四边形面片
				indices.AddRange(new[] { current, nextPoint, nextPointNext });
				indices.AddRange(new[] { current, nextPointNext, next });
			}
		}
		
		return indices;
	}
	
	private Vector3[] CalculateCirclePoints(Vector3 center, Vector3 direction, float radius, int segments)
	{
		var points = new Vector3[segments];
		Vector3 up = Vector3.Up;
		
		// 确保up和direction不平行
		if (Mathf.Abs(direction.Dot(up)) > 0.9f)
			up = Vector3.Right;
		
		Vector3 right = direction.Cross(up).Normalized();
		up = right.Cross(direction).Normalized();
		
		for (int i = 0; i < segments; i++)
		{
			float angle = 2 * Mathf.Pi * i / segments;
			points[i] = center + (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
		}
		
		return points;
	}
	
	private Vector3 CalculateNormal(List<Vector3> vertices, int index)
	{
		// 简化法线计算 - 实际使用时需要更精确的实现
		return Vector3.Up;
	}
}
