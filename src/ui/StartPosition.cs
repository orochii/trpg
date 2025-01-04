using Godot;
using System;

[Tool]
public partial class StartPosition : Sprite2D
{
	private const float OPACITY = 0.7f;
	private const float FULL = 1.0f;
	private const float HALF = 0.5f;
	private static Color F_NONE = new(FULL,FULL,FULL,OPACITY);
	private static Color F_ALLY = new(HALF,HALF,FULL,OPACITY);
	private static Color F_ENEMY = new(FULL,HALF,HALF,OPACITY);
	//
	[Export] public EFaction Faction = EFaction.ALLY;
	public override void _Process(double delta)
	{
		if (!IsVisibleInTree()) return;
		Modulate = Colors.White;
		SelfModulate = GetFactionColor();
		if (Engine.IsEditorHint()) {
			foreach (var c in GetChildren()) {
				if (c is Label) {
					var l = c as Label;
					l.Text = GetFactionLabel();
					return;
				}
			}
		}
	}
	public void SetLabel(int val) {
		foreach (var c in GetChildren()) {
			if (c is Label) {
				var l = c as Label;
				l.Text = val.ToString();
				return;
			}
		}
	}
	private Color GetFactionColor() {
		switch(Faction) {
			case EFaction.ALLY:
				return F_ALLY;
			case EFaction.ENEMY:
				return F_ENEMY;
			default:
				return F_NONE;
		}
	}
	private string GetFactionLabel() {
		switch(Faction) {
			case EFaction.ALLY:
				return "P";
			case EFaction.ENEMY:
				return "E";
			default:
				return "N";
		}
	}
	public Vector2I GetGlobalTilePos() {
		int tx = (int)(GlobalPosition.X / Unit.TILE_SIZE);
		int ty = (int)(GlobalPosition.Y / Unit.TILE_SIZE);
		return new Vector2I(tx,ty);
	}
	
}
