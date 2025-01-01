using System.Data.Common;
using Godot;

public enum EElement {
    PHYSICAL,FIRE,WATER,WIND,EARTH
}

[GlobalClass]
public partial class DataUnit : Resource {
    private const string PATH="res://dat/unit/";
    private const string EXT=".tres";
    //
    [ExportCategory("Display")]
    [Export] public string DisplayName;
    [Export] public Texture2D Graphic;
    [ExportCategory("Stats")]
    [Export] public int HP;
    [Export] public int LP;
    [Export] public int Offense;
    [Export] public int Dexterity;
    [Export] public int Move;
    [ExportCategory("Properties")]
    [Export] public EElement Element;
    [Export(PropertyHint.Layers2DPhysics)] public uint CollisionLayers;
    // Skills
    // Passives
    public string Id {
        get {
            var len = ResourcePath.Length - PATH.Length - EXT.Length;
            return ResourcePath.Substr(PATH.Length, len);
        }
    }
    public static DataUnit Get(string id) {
        return OZResourceLoader.Load<DataUnit>(PATH+id+EXT);
    }
}