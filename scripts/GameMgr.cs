using System.Collections.Generic;
using Godot;

public class GameMgr {
  public static PackedScene unitScene = GD.Load<PackedScene>("res://prefabs/unit.tscn");
  public static TileMap tilemap;

  private static List<Unit> _allyUnits;
  public static List<Unit> allyUnits {
    get {
      if (_allyUnits == null) {
        _allyUnits = new List<Unit>();
        Swordsman swordsman = new Swordsman();
        swordsman.group = "ally";
        _allyUnits.Add(swordsman);
      }
      return _allyUnits;
    }
    set { _allyUnits = value; }
  }
}