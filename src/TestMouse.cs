using Godot;
using System;

public partial class TestMouse : Node2D
{
	[Export] Camera2D Camera;
	[Export] Sprite2D Sprite;
	[Export] Sprite2D Cursor;
	[Export] Label Label;
	public override void _Process(double delta)
	{
		var move = Input.GetVector("ui_left","ui_right","ui_up","ui_down");
		Camera.Position += move;
		Cursor.GlobalPosition = GetViewport().GetMousePosition();
		//
		var playerPos = Sprite.GlobalPosition;
		var camera = GetViewport().GetCamera2D();
		var mouse = camera.GetLocalMousePosition();
		var playerOnScreen = playerPos - camera.GetScreenCenterPosition();
		var playerMouseOffset = mouse - playerOnScreen;
		//
		Label.Text = string.Format("mouse:{0} \n player:{1}",mouse,playerOnScreen);
	}
}
