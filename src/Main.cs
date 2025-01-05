using Godot;
using System;

public partial class Main : Node
{
	public static Main Instance {get;private set;}
	public static GameState State => m_State;
	private static GameState m_State = new();
	[Export] private Node2D MapParent;
	[Export] public Map Map;
	[Export] public GuiParent GUI;
	[ExportCategory("Templates")]
	[Export] public PackedScene UnitTemplate;
	public Main() {
		Instance = this;
		AudioManager.Init();
	}
	public override void _Ready()
	{
		//
	}
	public override void _Process(double delta)
	{
	}
}
