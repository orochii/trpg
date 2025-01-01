using System;
using System.Collections.Generic;

[Serializable]
public class GameMap {
    public string Name;
    public List<string> Deployed = new List<string>();
    public List<GameUnit> Units = new List<GameUnit>();
}