using Godot;
using System;

public enum EBattlePhase {
	INTRO, UNIT_SELECT, UNIT_PLACE, READY,
	START, SELECT, MOVE, ACTION, TARGET, PREDICTION, EXECUTE, RESULT, END,
	BATTLE_END
}

public partial class BattleUi : Control
{
	[Export] public AnimationPlayer Animation;
	[Export] public Label PhaseLabel;
	[Export] public VictoryConditions VictoryConditions;
	[Export] public UnitPlacement UnitPlacement;
	[Export] public OptionMessage ReadyMessage;
	[Export] public UnitStatus UnitStatusShort;
	public static bool Busy {get;private set;}
	private void ShowPhase(EFaction faction) {
		PhaseLabel.Text = $"{faction} Phase";
		Animation.Play("phaseChange");
	}
	public async void Open(EBattlePhase phase) {
		GD.Print("Set to phase ", phase);
		Busy = true;
		Animation.Play("show");
		var toFocus = ShowByPhase(phase);
		await ToSignal(Animation, AnimationPlayer.SignalName.AnimationFinished);
		Busy = false;
		if (toFocus != null) {
			var type = toFocus.GetType();
			var m = type.GetMethod("Focus");
            m?.Invoke(toFocus, null);
        }
	}
	public async void Close() {
		Busy = true;
		GetViewport().GuiReleaseFocus();
		Animation.Play("hide");
		await ToSignal(Animation, AnimationPlayer.SignalName.AnimationFinished);
		Busy = false;
		HideAll();
	}
	private Control ShowByPhase(EBattlePhase phase) {
		HideAll();
		switch(phase) {
			case EBattlePhase.INTRO:
				VictoryConditions.Visible = true;
				return VictoryConditions;
			case EBattlePhase.UNIT_SELECT:
				UnitPlacement.Refresh();
				UnitPlacement.Visible = true;
				return UnitPlacement;
			case EBattlePhase.READY:
				ReadyMessage.Visible = true;
				return ReadyMessage;
			case EBattlePhase.START:
				ShowPhase(Main.Instance.Map.Faction);
				return this;
		}
		return null;
	}
	private void HideAll() {
		VictoryConditions.Visible = false;
		UnitPlacement.Visible = false;
		ReadyMessage.Visible = false;
		UnitStatusShort.Setup(null);
	}
	public void Focus() {
		Main.Instance.Map.GoToPhase(EBattlePhase.SELECT);
	}
}
