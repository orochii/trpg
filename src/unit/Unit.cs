using Godot;
using Godot.NativeInterop;
using System;

public enum EFaction {
	ENEMY, NEUTRAL, ALLY
}

public partial class Unit : Node2D
{
	public const int TILE_SIZE = 16;
	public const float MOVE_SPEED = 1;
	[Export] private DataUnit Data;
	[Export] private EFaction Faction;
	private CharGraphic Graphic;
	public GameUnit State;
	public Vector2I CurrentPosition;
	public override void _Ready()
	{
		foreach(var c in GetChildren()) {
			if (c is CharGraphic) Graphic = c as CharGraphic;
		}
	}
	public override void _Process(double delta)
	{
		if (IsMoving()) {
			var t = TileToGlobalPos(CurrentPosition);
			GlobalPosition = GlobalPosition.MoveToward(t, MOVE_SPEED);
			return;
		}
		/* 	// Might repurpose this for uh non-battle maps
			// Idea is like Another Bible's towns.
		var horz = (int)Math.Round(Input.GetAxis("ui_left", "ui_right"));
		var vert = (int)Math.Round(Input.GetAxis("ui_up", "ui_down"));
		if (horz != 0 || vert != 0) {
			var newpos = CurrentPosition + new Vector2I(horz,vert);
			if (CanMove(newpos,V2ToDir(horz,vert))) {
				CurrentPosition = newpos;
			}
		}
		*/
	}
	public void Setup() {
		State = new GameUnit(Data.Id, Faction);
		CurrentPosition = GetGlobalTilePos();
		GlobalPosition = TileToGlobalPos(CurrentPosition);
		State.PosX = CurrentPosition.X;
		State.PosY = CurrentPosition.Y;
		Refresh();
	}
	public void Setup(GameUnit unit)
    {
        State = unit;
		Data = DataUnit.Get(unit.Id);
		Faction = unit.Faction;
		CurrentPosition = new Vector2I(unit.PosX, unit.PosY);
		GlobalPosition = TileToGlobalPos(CurrentPosition);
		Refresh();
    }
	public void Refresh() {
		if (Data == null) {
			Graphic.Texture = null;
			return;
		}
		Graphic.Texture = Data.Graphic;
	}
	public bool IsMoving() {
		var t = TileToGlobalPos(CurrentPosition);
		return GlobalPosition != t;
	}
	public Vector2I GetGlobalTilePos() {
		int tx = (int)(GlobalPosition.X / TILE_SIZE);
		int ty = (int)(GlobalPosition.Y / TILE_SIZE);
		return new Vector2I(tx,ty);
	}
	public bool CanMove(Vector2I tpos, uint dir=0) {
		if (Data == null) return false;
		if (Data.CollisionLayers==0) return true;
		//
		var space = GetWorld2D().DirectSpaceState;
		var to = TileToGlobalPos(tpos);
		var from = to + DirToVector2(dir);
		if(dir==0) from -= new Vector2(TILE_SIZE/2+1,0);
		var ray = PhysicsRayQueryParameters2D.Create(from,to,Data.CollisionLayers);
		var result = space.IntersectRay(ray);
		if (result.Count > 0) {
			return false;
		}
		return true;
	}
	public static Vector2 TileToGlobalPos(Vector2I tpos) {
		float px = tpos.X * TILE_SIZE + TILE_SIZE/2;
		float py = tpos.Y * TILE_SIZE + TILE_SIZE/2;
		return new Vector2(px, py);
	}
	public static uint V2ToDir(int x, int y) {
		if (x > 0) return 6;
		if (x < 0) return 4;
		if (y > 0) return 2;
		if (y < 0) return 8;
		return 0;
	}
	public static Vector2 DirToVector2(uint dir) {
		switch (dir) {
			case 2:
				return Vector2.Down * TILE_SIZE;
			case 4:
				return Vector2.Left * TILE_SIZE;
			case 6:
				return Vector2.Right * TILE_SIZE;
			case 8:
				return Vector2.Up * TILE_SIZE;
			default:
				return Vector2.Zero;
		}
	}
}
