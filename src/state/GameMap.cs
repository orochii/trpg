using System;
using System.Collections.Generic;

[Serializable]
public class GameMap {
    public string Name;
    public Dictionary<int,string> Deployed = new ();
    public List<GameUnit> Units = new ();
    public int IsDeployed(string id) {
        foreach (var kvp in Deployed) {
            if (kvp.Value == id) return kvp.Key;
        }
        return -1;
    }
    public void Deploy(string id, int index)
    {
        Undeploy(id);
        Deployed.Remove(index);
        Deployed.Add(index, id);
    }
    public void Undeploy(string id) {
        int i = IsDeployed(id);
        if (i >= 0) Deployed.Remove(i);
    }
}