using Godot;
using Godot.Collections;
using System;
using System.IO;

[Tool]
public partial class IconBar : MeshInstance2D
{
	[Export] uint Value;
	[Export] uint MaxValue;
	[Export] uint Pieces = 1;
	private uint _value;
	private uint _maxValue;
	private uint _pieces;
	public override void _Ready()
	{
		RegenerateMesh();
	}
	public override void _Process(double delta)
	{
		if (Changed()) {
			RegenerateMesh();
		}
	}
	private void RegenerateMesh() {
		if (Pieces==0 || MaxValue==0 || Texture==null) {
			this.Mesh = null;
			Changes();
			return;
		}
		//
		float tW = Texture.GetWidth();
		float tH = Texture.GetHeight();
		float pW = tW / (Pieces+1);
		float tWW = pW / tW;
		uint totalIcons = MaxValue / Pieces;
		GD.Print("W:",tW,"H:",tH," PW:",pW," tWW:",tWW," TotalIcons:",totalIcons);
		var st = new SurfaceTool();
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetMaterial(Material);
		for (var i = 0; i < totalIcons; i++) {
			var remainder = Value - (i * Pieces);
			var idx = Math.Clamp(remainder, 0, Pieces);
			//
			var x = i * pW;
			var y = 0;
			var u = idx * tWW;
			var v = 0;
			// First triangle
			st.SetUV(new Vector2(u,v));
			st.AddVertex(new Vector3(x,y,0));
			st.SetUV(new Vector2(u+tWW,v));
			st.AddVertex(new Vector3(x+pW,y,0));
			st.SetUV(new Vector2(u,v+1));
			st.AddVertex(new Vector3(x,y+tH,0));
			// Second triangle
			st.SetUV(new Vector2(u+tWW,v));
			st.AddVertex(new Vector3(x+pW,y,0));
			st.SetUV(new Vector2(u+tWW,v+1));
			st.AddVertex(new Vector3(x+pW,y+tH,0));
			st.SetUV(new Vector2(u,v+1));
			st.AddVertex(new Vector3(x,y+tH,0));
		}
		this.Mesh = st.Commit();
		// Mark as updated.
		Changes();
	}
	private void Changes() {
		_value = Value;
		_maxValue = MaxValue;
		_pieces = Pieces;
	}
	private bool Changed() {
		return (
			_value != Value ||
			_maxValue != MaxValue ||
			_pieces != Pieces
		);
	}
}
