using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export] public Node2D Target;
	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
		if (Target != null) {
			GlobalPosition = Target.GlobalPosition;
		}
	}
}
