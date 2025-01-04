using Godot;
using System;

[Tool]
public partial class CameraLimit : Node2D
{
	int left,right,top,bottom;
    public override void _Process(double delta)
    {
		if(!Engine.IsEditorHint()) return;
		var p = GetParent<Camera2D>();
		if (p==null) return;
        GlobalPosition = Vector2.Zero;
		if (left==p.LimitLeft ||
			right==p.LimitTop ||
			top==p.LimitRight ||
			bottom==p.LimitBottom)
			//Notification((int)NotificationDraw);
			QueueRedraw();
    }
    public override void _Draw()
    {
		if(!Engine.IsEditorHint()) return;
		var p = GetParent<Camera2D>();
		if (p==null) return;
		var x = p.LimitLeft;
		var y = p.LimitTop;
		var w = p.LimitRight - x;
		var h = p.LimitBottom - y;
		Rect2 r = new Rect2(x,y,w,h);
        DrawRect(r, Colors.Red, false, 2);
		// Update cached variables.
		left = p.LimitLeft;
		right = p.LimitTop;
		top = p.LimitRight;
		bottom = p.LimitBottom;
    }
}
