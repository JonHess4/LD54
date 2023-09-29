using System.Collections.Generic;

public class Engine {
  public static HashSet<Unit> units = new HashSet<Unit>();

  public static void addUnit(Unit unit) {
    units.Add(unit);
  }

  public static void removeUnit(Unit unit) {
    units.Remove(unit);
  }
}