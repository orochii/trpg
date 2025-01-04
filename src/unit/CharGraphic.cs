using Godot;
using System;

[Tool]
public partial class CharGraphic : Sprite2D
{
	public override void _Ready()
	{
		GD.Print("Char Graphic run.");
	}
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) {
			Texture = EditorGetGraphic();
			return;
		}
	}
	private Texture2D EditorGetGraphic() {
		var p = GetParent();
		if (p==null) return null;
		var d = p.Get("Data");
		if (d.Obj==null) return null;
		var data = d.As<Resource>();
		if (data==null) return null;
		return data.Get("Graphic").As<Texture2D>();
	}
}
