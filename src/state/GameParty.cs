using System.Collections.Generic;

public class GameParty {
    public Dictionary<string,GameUnit> Members = new Dictionary<string,GameUnit>();
    public GameUnit GetOrCreate(string id) {
        if (!Members.TryGetValue(id, out var unit)) {
            unit = new GameUnit(id,EFaction.ALLY);
            Members.Add(id, unit);
        }
        return unit;
    }
}