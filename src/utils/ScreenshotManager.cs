using Godot;

public partial class ScreenshotManager : Node {
    public override void _Input(InputEvent evt)
    {
        if (!evt.IsPressed()) return;
        if (evt is InputEventKey) {
            var key = evt as InputEventKey;
            if (key.Keycode == Key.F9) {
                TakeScreenshot();
            }
        }
    }
    private void TakeScreenshot() {
        var unix = Time.GetUnixTimeFromSystem();
        var path = "user://" + unix.ToString() + ".png";
        var image = GetViewport().GetTexture().GetImage();
        image.SavePng(path);
    }
}
