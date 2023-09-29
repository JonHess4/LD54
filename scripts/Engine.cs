using System.Collections.Generic;
using Godot;

public class Engine {
  public static HashSet<Unit> units = new HashSet<Unit>();

  public static void addUnit(Unit unit) {
    units.Add(unit);
  }

  public static void removeUnit(Unit unit) {
    units.Remove(unit);
  }

  public static void checkEndTurn(bool isEnemy) {
    bool turnFlip = true;
    foreach (Unit unit in units) {
      if (unit.isEnemy == isEnemy) {
        if (unit.isTurn) {
          turnFlip = false;
          break;
        }
      }
    }
    
    if (turnFlip) {
      foreach (Unit unit in units) {
      if (unit.isEnemy != isEnemy) {
        unit.isTurn = true;
      }
    }
    }
  }
}