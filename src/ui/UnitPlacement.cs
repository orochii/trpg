using Godot;
using System;
using System.Collections.Generic;

public partial class UnitPlacement : MarginContainer
{
	[Export] private Control UnitContainer;
	[Export] private UnitButton UnitTemplate;
	private List<UnitButton> entries = new();
    public override void _Ready()
    {
        UnitTemplate.Visible = false;
    }
	public void Refresh() {
		foreach (var e in entries) e.QueueFree();
		foreach (var unit in Main.State.Party.Members) {
			var i = UnitTemplate.Duplicate() as UnitButton;
			i.Visible = true;
			i.Setup(unit.Key);
			UnitContainer.AddChild(i);
			entries.Add(i);
		}
	}
	public void Focus() {
		entries[0].GrabFocus();
	}
}
