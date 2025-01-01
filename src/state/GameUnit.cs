[System.Serializable]
public class GameUnit {
    public string Id;
    public EFaction Faction;
    public int CurrentHP;
    public int CurrentLP;
    public bool InBattle;
    public int PosX;
    public int PosY;
    public GameUnit(string id, EFaction faction) {
        Id = id;
        Faction = faction;
        Initialize();
    }
    public void Initialize() {
        var data = DataUnit.Get(Id);
        CurrentHP = data.HP;
        CurrentLP = data.LP;
    }
}
