using Godot;
using System;

public partial class Main : Node
{
	public static Main Instance {get;private set;}
	public static GameState State {get;private set;}
	[Export] private Node2D MapParent;
	[Export] private Node2D Map;
	[ExportCategory("Templates")]
	[Export] public PackedScene UnitTemplate;
	public Main() {
		Instance = this;
		if (State==null) State = new GameState();
	}
	public override void _Ready()
	{
		//
	}
	public override void _Process(double delta)
	{
	}
}
