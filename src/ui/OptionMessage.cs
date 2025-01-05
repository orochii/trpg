using Godot;

public partial class OptionMessage : MarginContainer {
    [Export] Button[] Options;
    public int SelectedIndex;
    public override void _Ready()
    {
        for (int i = 0; i < Options.Length; i++) {
            int index = i;
            Options[i].Pressed += () => {
                // I don't like lambdas but I'm also lazy :)
                GD.Print("Selected idx:",index);
                SelectedIndex = index;
                Main.Instance.Map.OnAction();
            };
        }
    }
    public void Focus() {
        Options[0].GrabFocus();
    }
}