using Godot;
using System;
using System.Collections.Generic;

public partial class MapCursor : Node2D
{
	public static MapCursor Instance {get; private set;}
	public const float MOVE_SPEED = 2;
	[Export] private Sprite2D Preview;
	[Export] private Texture2D SquareTexture;
	[Export] private Texture2D PathTexture;
	Vector2 CurrentPosition;
	public override void _Ready()
	{
		Instance = this;
		var cam = GetViewport().GetCamera2D() as Camera;
		cam.Target = this;
		CurrentPosition = GlobalPosition;
	}
	public void Reposition(Vector2 pos) {
		pos = Unit.SnapPosition(pos);
		GlobalPosition = pos;
		CurrentPosition = pos;
	}
	public Unit LastHover = null;
	private Vector2I lastPosition;
	public override void _Process(double delta)
	{
		RefreshPreview();
		if (IsMoving()) {
			GlobalPosition = GlobalPosition.MoveToward(CurrentPosition, MOVE_SPEED);
			return;
		}
		if (!IsVisibleInTree()) return;
		if (GetViewport().GuiGetFocusOwner() != null || BattleUi.Busy) {
			return;
		}
		if (Map.Busy) return;
		if (Main.Instance.Map.SelectedUnit != null && Main.Instance.Map.SelectedUnit.IsMoving()) {
			return;
		}
		var hovered = Main.Instance.Map.GetUnitAt(CurrentPosition);
		RefreshHovered(hovered);
		RefreshSelected();
		if (Input.IsActionJustPressed("ui_accept")) {
			Main.Instance.Map.OnAction();
			return;
		}
		if (Input.IsActionJustPressed("ui_cancel")) {
			Main.Instance.Map.OnCancel();
			return;
		}
		var horz = (int)Math.Round(Input.GetAxis("ui_left", "ui_right"));
		var vert = (int)Math.Round(Input.GetAxis("ui_up", "ui_down"));
		if (horz != 0 || vert != 0) {
			var newX = GlobalPosition.X + horz*Unit.TILE_SIZE;
			var newY = GlobalPosition.Y + vert*Unit.TILE_SIZE;
			//
			var clampedPos = GetClampedPos(newX,newY);
			var unit = Main.Instance.Map.SelectedUnit;
			if (unit != null) {
				var tpos = Unit.GlobalToTilePos(clampedPos);
				if (!unit.GetWalkableArea().Contains(tpos)) return;
			}
			CurrentPosition = clampedPos;
		}
	}
	public void RefreshHovered(Unit hovered) {
		if (LastHover != hovered) {
			if (hovered != null) {
				Main.Instance.GUI.Battle.UnitStatusShort.Setup(hovered.State);
			}
			else Main.Instance.GUI.Battle.UnitStatusShort.Setup(null);
			LastHover = hovered;
			if (Main.Instance.Map.SelectedUnit == null) RefreshAreaHover();
		}
	}
	private void RefreshSelected() {
		var _selected = Main.Instance.Map.SelectedUnit;
		if (_selected != null) {
			var target = Unit.GlobalToTilePos(GlobalPosition);
			if (lastPosition != target) {
				var path = _selected.GetPathTo(target);
				SetPath(path);
				lastPosition = target;
			}
		} else {
			SetPath(null);
		}
	}
	List<Sprite2D> areaSquares = new List<Sprite2D>();
	List<Sprite2D> pathTiles = new List<Sprite2D>();
	private void RefreshAreaHover() {
		foreach (var s in areaSquares) s.Visible = false;
		if (LastHover == null) return;
		if (LastHover.Faction != Main.Instance.Map.Faction) return;
		var walkable = LastHover.GetWalkableArea();
		if (walkable == null) return;
		SetArea(walkable, Colors.BlueViolet);
	}
	private void SetArea(List<Vector2I> area, Color c, float a=.5f) {
		while (areaSquares.Count < area.Count) SpawnSquare();
		for (var i = 0; i < area.Count; i++) {
			var w = area[i];
			var s = areaSquares[i];
			s.GlobalPosition = new Vector2(w.X, w.Y) * Unit.TILE_SIZE;
			s.GlobalPosition += Vector2.One * (Unit.TILE_SIZE / 2);
			s.SelfModulate = c;
			s.Modulate = new Color(1,1,1,a);
			s.Visible = true;
		}
	}
	public void SetPath(List<Vector2I> path) {
		foreach (var s in pathTiles) s.Visible = false;
		if (path==null) return;
		while (pathTiles.Count < path.Count) SpawnPath();
		for (var i = 1; i < path.Count; i++) {
			var curr = path[i-1];
			var next = path[i];
			var prev = i>1 ? path[i-2] : curr;
			var dirStart = GetPathDirection(prev,curr);
			var dirEnd = GetPathDirection(curr,next);
			if (i == path.Count-1) { // End
				if (dirStart==dirEnd)
					SetPathTile(i,0,dirEnd,curr);
				else {
					var t = SetPathTile(i,4,dirStart,curr);
					var dS = dirStart < 0 ? 360-dirStart : dirStart;
					t.FlipH = (dS > dirEnd && !(dirStart==180&&dirEnd==-90));
				}
			}
			else if (prev==curr) { // Start
				SetPathTile(i,1,dirEnd,curr);
			}
			else if (dirStart==dirEnd) // Straight
				SetPathTile(i,2,dirStart,curr);
			else { // Curve
				var t = SetPathTile(i,3,dirStart,curr);
				var dS = dirStart < 0 ? 360-dirStart : dirStart;
				t.FlipH = (dS > dirEnd && !(dirStart==180&&dirEnd==-90));
			}
		}
	}
	private float GetPathDirection(Vector2I c, Vector2I t) {
		var d = t-c;
		if (d.X<0) return -90;
		if (d.X>0) return 90;
		if (d.Y>0) return 180;
		return 0;
	}
	private Sprite2D SetPathTile(int idx, int tileIdx, float angle, Vector2I pos) {
		var t = pathTiles[idx];
		var x = (tileIdx%2) * 16;
		var y = (tileIdx/2) * 16;
		t.RegionRect = new Rect2(x,y,16,16);
		t.RotationDegrees = angle;
		t.Visible = true;
		t.GlobalPosition = new Vector2(pos.X, pos.Y) * Unit.TILE_SIZE + new Vector2(Unit.TILE_SIZE/2,Unit.TILE_SIZE/2);
		return t;
	}
	private void SpawnSquare() {
		var newSquare = new Sprite2D();
		newSquare.Texture = SquareTexture;
		newSquare.Visible = false;
		Main.Instance.Map.SquareParent.AddChild(newSquare);
		areaSquares.Add(newSquare);
	}
	private void SpawnPath() {
		var tile = new Sprite2D();
		tile.Texture = PathTexture;
		tile.RegionEnabled = true;
		tile.RegionRect = new Rect2(0,0,16,16);
		tile.Visible = false;
		Main.Instance.Map.SquareParent.AddChild(tile);
		pathTiles.Add(tile);
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
	private void RefreshPreview() {
		if (Main.Instance.Map != null && Main.Instance.Map.CurrentUnit != null) {
			var u = Main.Instance.Map.CurrentUnit;
			var d = DataUnit.Get(u.Id);
			Preview.Visible = true;
			Preview.Texture = d.Graphic;
		} else {
			Preview.Visible = false;
		}
	}
}
