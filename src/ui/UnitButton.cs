using Godot;
using System;

public partial class UnitButton : Button
{
	public string Id {get; private set;}
	private GameUnit State;
	public DataUnit Data {get; private set;}
    public override void _Ready()
    {
        Pressed += OnPressed;
    }
    private void OnPressed() {
		//
	}
	public void Setup(string id) {
		Id = id;
		State = Main.State.Party.GetOrCreate(Id);
		Data = DataUnit.Get(Id);
		Icon = Data==null ? null : Data.Graphic;
	}
}
