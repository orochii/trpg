using Godot;
using System;

public partial class VictoryConditions : MarginContainer
{
	[Export] Label ConditionsLabel;
	[Export] Button OkButton;
    public override void _Ready()
    {
        OkButton.Pressed += OnOkPressed;
    }
    private void OnOkPressed() {
		// Go to next phase.
		Main.Instance.Map.GoToPhase(EBattlePhase.UNIT_SELECT);
	}
	public void Focus() {
		OkButton.GrabFocus();
	}
}
