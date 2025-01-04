using Godot;
using System;

public partial class MapCursor : Node2D
{
	public const float MOVE_SPEED = 2;
	Vector2 CurrentPosition;
	public override void _Ready()
	{
		var cam = GetViewport().GetCamera2D() as Camera;
		cam.Target = this;
		CurrentPosition = GlobalPosition;
	}
	public override void _Process(double delta)
	{
		if (IsMoving()) {
			GlobalPosition = GlobalPosition.MoveToward(CurrentPosition, MOVE_SPEED);
			return;
		}
		if (!IsVisibleInTree()) return;
		if (GetViewport().GuiGetFocusOwner() != null) {
			return;
		}
		var horz = (int)Math.Round(Input.GetAxis("ui_left", "ui_right"));
		var vert = (int)Math.Round(Input.GetAxis("ui_up", "ui_down"));
		if (horz != 0 || vert != 0) {
			var newX = GlobalPosition.X + horz*Unit.TILE_SIZE;
			var newY = GlobalPosition.Y + vert*Unit.TILE_SIZE;
			//
			CurrentPosition = GetClampedPos(newX,newY);
		}
	}
	public Vector2 GetClampedPos(float x, float y) {
		var cam = GetViewport().GetCamera2D();
		var l = cam.LimitLeft / Unit.TILE_SIZE * Unit.TILE_SIZE;
		var r = (cam.LimitRight-Unit.TILE_SIZE) / Unit.TILE_SIZE * Unit.TILE_SIZE;
		var t = cam.LimitTop / Unit.TILE_SIZE * Unit.TILE_SIZE;
		var b = (cam.LimitBottom-Unit.TILE_SIZE) / Unit.TILE_SIZE * Unit.TILE_SIZE;
		return new Vector2(Math.Clamp(x, l, r), Math.Clamp(y, t, b));
	}
	public bool IsMoving() {
		return GlobalPosition != CurrentPosition;
	}
}
