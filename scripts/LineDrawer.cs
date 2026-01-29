using Godot;

public class LineDrawer
{
	private Camera3D _camera;
	
	public LineDrawer(Camera3D camera)
	{
		_camera = camera;
	}
	
	public Vector3? GetPointInWorld(Vector2 screenPosition)
	{
		// 从摄像机发射射线检测碰撞点
		var from = _camera.ProjectRayOrigin(screenPosition);
		var to = from + _camera.ProjectRayNormal(screenPosition) * 100;
		
		// 使用PhysicsRayQueryParameters3D进行3D射线检测
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = _camera.GetWorld3D().DirectSpaceState.IntersectRay(query);
		
		if (result.Count > 0 && result.ContainsKey("position"))
		{
			return (Vector3)result["position"];
		}
		
		return null;
	}
}
