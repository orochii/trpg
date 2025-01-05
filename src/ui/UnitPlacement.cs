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
    public override void _Process(double delta)
    {
        if (!IsVisibleInTree()) return;
		if (Input.IsActionJustPressed("ui_cancel")) {
			Main.Instance.Map.OnCancel();
		}
    }
    
	public void Refresh() {
		foreach (var e in entries) e.QueueFree();
		entries.Clear();
		foreach (var unit in Main.State.Party.Members) {
			var i = UnitTemplate.Duplicate() as UnitButton;
			i.Visible = true;
			i.Setup(unit.Key);
			UnitContainer.AddChild(i);
			entries.Add(i);
			GD.Print(unit.Key);
		}
	}
	public void Focus() {
		int idx = 0;
		for (int i = 0; i < entries.Count; i++) {
			if (entries[i].Enabled) {
				idx = i;
				break;
			}
		}
		entries[idx].GrabFocus();
	}
}
