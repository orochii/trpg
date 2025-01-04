using System;
using System.Collections.Generic;

[Serializable]
public class GameMap {
    public string Name;
    public Dictionary<int,string> Deployed = new ();
    public List<GameUnit> Units = new ();
}