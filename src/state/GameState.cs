[System.Serializable]
public class GameState {
    public int Money;
    public GameMap Map = null;
    public GameParty Party = new GameParty();
}
