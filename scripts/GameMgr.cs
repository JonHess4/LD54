using System.Collections.Generic;
using Godot;

public class GameMgr {
  public static PackedScene unitScene = GD.Load<PackedScene>("res://prefabs/unit.tscn");

  private static List<Unit> _allyUnits;
  public static List<Unit> allyUnits {
    get {
      if (_allyUnits == null) {
        _allyUnits = new List<Unit>();
        _allyUnits.Add(new Unit());
      }
      return _allyUnits;
    }
    set { _allyUnits = value; }
  }
}