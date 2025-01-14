using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public enum EFaction {
	ENEMY, NEUTRAL, ALLY
}

public partial class Unit : Node2D
{
	public const int TILE_SIZE = 16;
	public const float MOVE_SPEED = 2;
	static Vector2I[] ps = new Vector2I[]{Vector2I.Up,Vector2I.Down,Vector2I.Left,Vector2I.Right};
	static uint[] ds = new uint[]{8,2,4,6};
	[Export] public DataUnit Data;
	[Export] public EFaction Faction;
	private CharGraphic Graphic;
	public GameUnit State;
	public Vector2I CurrentPosition;
	public Action OnPathFinish;
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
			if (IsMoving()) return;
		}
		if (currentPath != null && currentPath.Count > 0) {
			CurrentPosition = currentPath[0];
			currentPath.RemoveAt(0);
			if (currentPath.Count==0) {
				if(OnPathFinish != null) OnPathFinish.Invoke();
			}
		}
	}
	public void Reposition(Vector2 pos) {
		GlobalPosition = pos;
		CurrentPosition = GetGlobalTilePos();
		GlobalPosition = TileToGlobalPos(CurrentPosition);
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
		return GlobalToTilePos(GlobalPosition);
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
	public class PathNode {
		public uint Depth;
		public Vector2I Position;
		public PathNode Prev;
		public PathNode(uint depth, Vector2I pos, PathNode prev) {
			Depth = depth;
			Position = pos;
			Prev = prev;
		}
		public void Set(uint depth, PathNode prev) {
			Depth = depth;
			Prev = prev;
		}
	}
	private Dictionary<Vector2I,PathNode> pathGraph;
	private List<Vector2I> walkable;
	private List<Vector2I> currentPath;
	public List<Vector2I> GetWalkableArea() {
		if (walkable==null) GenerateArea();
		return walkable;
	}
	public List<Vector2I> GetPathTo(Vector2I target) {
		if (pathGraph==null) return null;
		if (!pathGraph.TryGetValue(target, out var n)) return null;
		var path = new List<Vector2I>();
		while (n != null) {
			path.Add(n.Position);
			n = n.Prev;
		}
		path.Reverse();
		return path;
	}
	public bool GoTo(Vector2I pos) {
		var path = GetPathTo(pos);
		if (path == null) return false;
		currentPath = path;
		return true;
	}
	public void GenerateArea() {
		walkable = new(){CurrentPosition};
		PathNode rootNode = new(0,CurrentPosition,null);
		pathGraph = new(){{CurrentPosition,rootNode}};
		PlotArea(rootNode, Data.Move);
	}
	private void PlotArea(PathNode node, int move) {
		if (move <= 0) return;
		for (int i=0;i<ps.Length;i++) {
			var p = node.Position + ps[i];
			var d = ds[i];
			if (CanMove(p, d)) {
				if (!walkable.Contains(p)) walkable.Add(p);
				var depth = node.Depth+1;
				if (!pathGraph.TryGetValue(p, out var n)) {
					n = new PathNode(depth, p, node);
					pathGraph.Add(p,n);
				} else {
					if (n.Depth > depth) {
						n.Set(depth, node);
					}
				}
				PlotArea(n,move-1);
			}
		}
	}
    public static Vector2 TileToGlobalPos(Vector2I tpos) {
		float px = tpos.X * TILE_SIZE + TILE_SIZE/2;
		float py = tpos.Y * TILE_SIZE + TILE_SIZE/2;
		return new Vector2(px, py);
	}
	public static Vector2I GlobalToTilePos(Vector2 pos) {
		int tx = (int)(pos.X / TILE_SIZE);
		int ty = (int)(pos.Y / TILE_SIZE);
		return new Vector2I(tx,ty);
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
	public static Vector2 SnapPosition(Vector2 pos) {
		var x = (int)(pos.X / Unit.TILE_SIZE) * Unit.TILE_SIZE;
		var y = (int)(pos.Y / Unit.TILE_SIZE) * Unit.TILE_SIZE;
		if (pos.X < 0) x -= 1;
		if (pos.Y < 0) y -= 1;
		return new Vector2(x,y);
	}
}
