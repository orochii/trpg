using Godot;
using System;

public partial class SkillSelect : MarginContainer
{
	[Export] Button[] Buttons;
	[Export] RichTextLabel Description;
	public override void _Process(double delta)
	{
		if (!IsVisibleInTree()) return;
		if (Input.IsActionJustPressed("ui_cancel")) {
			Main.Instance.Map.OnCancel();
		}
	}
	public void Refresh() {
		//
	}
	public void Focus() {
		Buttons[0].GrabFocus();
	}
}
