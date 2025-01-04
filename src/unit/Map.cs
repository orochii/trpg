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
	bool test = false;
	public Map() {

		test = !IsMapValid();
	}
	public override void _Ready()
    {
		Main.Instance.Map = this;
		if (test) {
			foreach (var unit in TestMapUnits) {
				Main.State.Party.GetOrCreate(unit);
			}
		}
		PrepareInstance();
		PrepareMapUnits();
		// PreparePartyUnits();
		GoToPhase(EBattlePhase.INTRO);
    }
	private void PrepareInstance() {
		allUnits = new();
		startPositions = new();
		foreach (var c in ObjectParent.GetChildren()) {
			if (c is StartPosition) {
				var sp = c as StartPosition;
				startPositions.Add(sp);
				sp.SetLabel(startPositions.Count);
			}
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
        }
		for (;unitInitIdx < allUnits.Count; unitInitIdx++) {
			var obj = allUnits[unitInitIdx];
			obj.Setup();
			Main.State.Map.Units.Add(obj.State);
		}
	}
	private void PreparePartyUnits() {
		foreach (var kvp in Main.State.Map.Deployed) {
			if (kvp.Key < 0 || startPositions.Count <= kvp.Key) return;
			var unit = Main.State.Party.GetOrCreate(kvp.Value);
			if (!unit.InBattle) {
				var sp = startPositions[kvp.Key];
				var tpos = sp.GetGlobalTilePos();
				unit.PosX = tpos.X;
				unit.PosY = tpos.Y;
				unit.InBattle = true;
			}
			var obj = GetOrCreate(unitInitIdx);
			obj.Setup(unit);
			unitInitIdx++;
		}
		for (var idx = 0; idx < startPositions.Count; idx++) {
			startPositions[idx].Visible = false;
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
    EBattlePhase CurrentPhase;
	public void GoToPhase(EBattlePhase phase) {
		CurrentPhase = phase;
		Main.Instance.GUI.Battle.Open(phase);
	}
	public override void _Process(double delta)
	{
		if (!BattleUi.Busy) {
			var battle = Main.Instance.GUI.Battle;
			switch (CurrentPhase) {
				case EBattlePhase.INTRO:
					break;
				case EBattlePhase.UNIT_SELECT:
					break;
				case EBattlePhase.UNIT_PLACE:
					break;
				case EBattlePhase.READY:
					break;
				case EBattlePhase.START:
					break;
				case EBattlePhase.SELECT:
					break;
				case EBattlePhase.MOVE:
					break;
				case EBattlePhase.ACTION:
					break;
				case EBattlePhase.TARGET:
					break;
				case EBattlePhase.PREDICTION:
					break;
				case EBattlePhase.EXECUTE:
					break;
				case EBattlePhase.RESULT:
					break;
			}
		}
	}
}
