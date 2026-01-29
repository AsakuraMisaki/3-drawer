using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class MainScene : Node3D
{
	[Export] public Camera3D MainCamera { get; set; }
	[Export] public MeshInstance3D MeshPreview { get; set; }
	
	private LineDrawer _lineDrawer;
	private PatchGenerator _patchGenerator;
	private List<Vector3> _currentPoints = new List<Vector3>();

	public override void _Ready()
	{
		_lineDrawer = new LineDrawer(MainCamera);
		_patchGenerator = new PatchGenerator();
		
		// 初始化一个空Mesh用于预览
		MeshPreview.Mesh = new BoxMesh();
	}

	public override void _Input(InputEvent @event)
	{
		Debug.WriteLine("aa");
		// 鼠标左键按下开始画线
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				var point = _lineDrawer.GetPointInWorld(mouseEvent.Position);
				if (point != null)
				{
					_currentPoints.Add(point.Value);
					UpdateMeshPreview();
				}
			}
		}
	}

	private void UpdateMeshPreview()
	{
		if (_currentPoints.Count >= 2)
		{
			var mesh = _patchGenerator.CreateTubeMesh(_currentPoints, radius: 0.1f, segments: 8);
			MeshPreview.Mesh = mesh;
		}
	}
}
