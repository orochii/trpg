using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class FrameRect : NinePatchRect
{
	[Export] Control Follow;
	[Export] int MarginTop = 4;
	[Export] int MarginBottom = 4;
	[Export] int MarginLeft = 4;
	[Export] int MarginRight = 4;
	public override void _Ready()
	{
		Refresh();
	}
	public override void _Process(double delta)
	{
		Refresh();
	}
	private void Refresh() {
		if (Follow == null || Follow.IsVisibleInTree() == false) {
			Visible = false;
			return;
		}
		Visible = true;
		var marginTL = new Vector2(MarginLeft, MarginTop);
		var marginBR = new Vector2(MarginRight, MarginBottom);
		GlobalPosition = Follow.GlobalPosition - marginTL;
		Size = Follow.Size + marginTL + marginBR;
	}
}
