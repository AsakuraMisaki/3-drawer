// ThirdPersonCameraController.cs
using Godot;

public partial class ThirdPersonCameraController : Node3D
{
    [ExportGroup("Camera Refs")]
    [Export] public SpringArm3D SpringArm;
    [Export] public Camera3D Camera;

    [ExportGroup("Control Settings")]
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float ZoomSensitivity = 2.0f;
    [Export] public float PanSensitivity = 0.01f;

    [ExportGroup("Rotation Limits")]
    [Export] public float MinVerticalAngle = -80.0f; // 最小俯角（度）
    [Export] public float MaxVerticalAngle = 80.0f;  // 最大仰角（度）

    [ExportGroup("Zoom Limits")]
    [Export] public float MinZoomDistance = 1.0f;
    [Export] public float MaxZoomDistance = 10.0f;

    // 当前相机的水平、垂直旋转角度
    private float _yaw;
    private float _pitch;
    // 用于平移的向量
    private Vector3 _panVector;

    public override void _Input(InputEvent @event)
    {
        // 1. 鼠标右键/左键拖拽：旋转相机
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // 示例：响应鼠标右键拖拽进行旋转
            if (Input.IsMouseButtonPressed(MouseButton.Right))
            {
                _yaw -= mouseMotion.Relative.X * MouseSensitivity;
                _pitch -= mouseMotion.Relative.Y * MouseSensitivity;
                // 限制垂直旋转角度，避免相机翻转到物体下方
                _pitch = Mathf.Clamp(_pitch, Mathf.DegToRad(MinVerticalAngle), Mathf.DegToRad(MaxVerticalAngle));

                // 应用旋转：水平旋转由父节点Node3D控制，垂直旋转由SpringArm3D控制
                this.Rotation = new Vector3(0, _yaw, 0); // 父节点控制水平旋转 (Yaw)
                SpringArm.Rotation = new Vector3(_pitch, 0, 0); // SpringArm控制垂直旋转 (Pitch)
            }
            // 示例：响应鼠标中键拖拽进行平移
            else if (Input.IsMouseButtonPressed(MouseButton.Middle))
            {
                _panVector = -this.GlobalTransform.Basis.X * (mouseMotion.Relative.X * PanSensitivity) + 
                             -this.GlobalTransform.Basis.Y * (mouseMotion.Relative.Y * PanSensitivity);
                this.GlobalTranslate(_panVector);
            }
        }

        // 2. 鼠标滚轮：缩放相机（通过调整SpringArm3D的Length实现）
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                SpringArm.SpringLength = Mathf.Max(MinZoomDistance, SpringArm.SpringLength - ZoomSensitivity);
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                SpringArm.SpringLength = Mathf.Min(MaxZoomDistance, SpringArm.SpringLength + ZoomSensitivity);
            }
        }
    }
}