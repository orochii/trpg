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
	public static bool Busy {get;private set;}
    public override void _Ready()
    {
        Animation.AnimationFinished += OnAnimationFinished;
    }
    private void OnAnimationFinished(StringName name) {
		//
	}
	public async void ShowPhase(EFaction faction) {
		Busy = true;
		PhaseLabel.Text = $"{faction} Phase";
		Animation.Play("phaseChange");
		await ToSignal(Animation, AnimationPlayer.SignalName.AnimationFinished);
		Busy = false;
	}
	public async void Open(EBattlePhase phase) {
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
		}
		return null;
	}
	private void HideAll() {
		VictoryConditions.Visible = false;
		UnitPlacement.Visible = false;
	}
}
