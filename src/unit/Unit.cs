using Godot;
using Godot.NativeInterop;
using System;

public partial class Unit : Node2D
{
	public const int TILE_SIZE = 16;
	[Export] float MoveSpeed = 1;
	[Export(PropertyHint.Layers2DPhysics)] uint CollisionLayers;
	private Vector2I targetPosition;
	public override void _Ready()
	{
		targetPosition = GetTilePos();
		GlobalPosition = TileToGlobalPos(targetPosition);
	}
	public override void _Process(double delta)
	{
		//var currPos = GetTilePos();
		var t = TileToGlobalPos(targetPosition);
		if (GlobalPosition != t) {
			GlobalPosition = GlobalPosition.MoveToward(t, MoveSpeed);
			return;
		}
		var horz = (int)Math.Round(Input.GetAxis("ui_left", "ui_right"));
		var vert = (int)Math.Round(Input.GetAxis("ui_up", "ui_down"));
		if (horz != 0 || vert != 0) {
			var newpos = targetPosition + new Vector2I(horz,vert);
			if (CanMove(newpos)) {
				targetPosition = newpos;
			}
		}
	}
	private Vector2 TileToGlobalPos(Vector2I tpos) {
		float px = tpos.X * TILE_SIZE + TILE_SIZE/2;
		float py = tpos.Y * TILE_SIZE + TILE_SIZE/2;
		return new Vector2(px, py);
	}
	private Vector2I GetTilePos() {
		int tx = (int)(GlobalPosition.X / TILE_SIZE);
		int ty = (int)(GlobalPosition.Y / TILE_SIZE);
		return new Vector2I(tx,ty);
	}
	public bool CanMove(Vector2I tpos, bool ignoreWall=false) {
		if (CollisionLayers==0) return true;
		//
		var space = GetWorld2D().DirectSpaceState;
		var to = TileToGlobalPos(tpos);
		var from = to - new Vector2(8,0);
		if (!ignoreWall) from = TileToGlobalPos(GetTilePos());
		var ray = PhysicsRayQueryParameters2D.Create(from,to,CollisionLayers);
		var result = space.IntersectRay(ray);
		if (result.Count > 0) {
			return false;
		}
		return true;
	}
}
