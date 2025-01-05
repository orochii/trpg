using Godot;

public partial class UnitStatus : MarginContainer
{
	[ExportCategory("Basic")]
	[Export] private Label NameLabel;
	[Export] private Label HPLabel;
	[Export] private IconBar LP;
	[Export] private Label MoveLabel;
	[ExportCategory("Extended")]
	[Export] private TextureRect Graphic;
	[Export] private Label OffenseLabel;
	[Export] private Label DexterityLabel;
	GameUnit _unit = null;
	public void Setup(GameUnit unit) {
		if (unit == null) {
			_unit = unit;
			Visible = false;
			return;
		}
		if (unit==_unit) return;
		_unit = unit;
		Visible = true;
		var data = DataUnit.Get(unit.Id);
		// Basics.
		NameLabel.Text = data.DisplayName;
		HPLabel.Text = string.Format("{0}/{1}", unit.CurrentHP, data.HP);
		LP.Value = (uint)unit.CurrentLP;
		LP.MaxValue = (uint)data.LP;
		MoveLabel.Text = data.Move.ToString();
		// Extended data.
		if (Graphic != null) Graphic.Texture = data.Graphic;
		if (OffenseLabel != null) OffenseLabel.Text = data.Offense.ToString();
		if (DexterityLabel != null) DexterityLabel.Text = data.Dexterity.ToString();
	}
}
