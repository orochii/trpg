using Godot;
using System;

public partial class UnitButton : Button
{
	public string Id {get; private set;}
	private GameUnit State;
	public DataUnit Data {get; private set;}
	public bool Enabled {get; private set;}
    public override void _Ready()
    {
        Pressed += OnPressed;
    }
    private void OnPressed() {
		Main.Instance.Map.GoToPhase(EBattlePhase.UNIT_PLACE);
		Main.Instance.Map.CurrentUnit = State;
	}
	public void Setup(string id) {
		Id = id;
		State = Main.State.Party.GetOrCreate(Id);
		Data = DataUnit.Get(Id);
		Icon = Data==null ? null : Data.Graphic;
		Enabled = Main.State.Map.IsDeployed(Id) < 0;
		Modulate = Enabled ? Colors.White : new Color(.1f,.1f,.1f,1f);
	}

}
