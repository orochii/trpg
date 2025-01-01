using Godot;
using System;
using System.Collections.Generic;

public partial class Map : Node2D
{
	[Export] string CurrentMapName;
	[Export] Node2D ObjectParent;
	[Export] string[] TestMapUnits = new string[1]{"hikari"};
	List<StartPosition> startPositions;
	List<Unit> allUnits;
	int unitInitIdx = 0;
	public override void _Ready()
    {
		PrepareInstance();
		PrepareMapUnits();
		/* 
			Will probably have to split this here, 
			to do a "preparation phase" of sorts.
		*/
		PreparePartyUnits();
    }
	private void PrepareInstance() {
		allUnits = new();
		startPositions = new();
		foreach (var c in ObjectParent.GetChildren()) {
			if (c is StartPosition) startPositions.Add(c as StartPosition);
			else if (c is Unit) allUnits.Add(c as Unit);
		}
	}
	private void PrepareMapUnits() {
		if (IsMapValid()) {
			// When loading, etc, update current units.
			foreach (var unit in Main.State.Map.Units) {
				var obj = GetOrCreate(unitInitIdx);
				obj.Setup(unit);
				unitInitIdx++;
			}
		} else {
			// Otherwise restart fight.
            Main.State.Map = new GameMap();
			foreach (var id in TestMapUnits) {
				Main.State.Map.Deployed.Add(id);
			}
        }
		for (;unitInitIdx < allUnits.Count; unitInitIdx++) {
			var obj = allUnits[unitInitIdx];
			obj.Setup();
			Main.State.Map.Units.Add(obj.State);
		}
	}
	private void PreparePartyUnits() {
		var spIdx = 0;
		foreach (var id in Main.State.Map.Deployed) {
			var unit = Main.State.Party.GetOrCreate(id);
			if (!unit.InBattle) {
				var sp = startPositions[spIdx];
				var tpos = sp.GetGlobalTilePos();
				unit.PosX = tpos.X;
				unit.PosY = tpos.Y;
				unit.InBattle = true;
				sp.Visible = false;
				spIdx++;
			}
			var obj = GetOrCreate(unitInitIdx);
			obj.Setup(unit);
			unitInitIdx++;
		}
		for (;spIdx < startPositions.Count; spIdx++) {
			startPositions[spIdx].Visible = false;
		}
	}
	private Unit GetOrCreate(int idx) {
		if (idx < allUnits.Count) return allUnits[idx];
		// Create
		var newobj = Main.Instance.UnitTemplate.Instantiate<Unit>();
		ObjectParent.AddChild(newobj);
		allUnits.Add(newobj);
		return newobj;
	}
    private bool IsMapValid()
    {
        return Main.State.Map != null && Main.State.Map.Name == CurrentMapName;
    }
    public override void _Process(double delta)
	{
	}
}
