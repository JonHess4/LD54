using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class Engine : Camera2D {
  public static String endGameText = "";
  private static HashSet<Unit> _units;
  public static TileMap _tilemap;
  public static bool isBuyPhase = false;
  public static int round = 1;
  public static int prevRound = 1;
  public static bool gameOver = false;

  public static PackedScene enemyDagger = GD.Load<PackedScene>("res://scenes/red_dagger.tscn");
  public static PackedScene enemySword = GD.Load<PackedScene>("res://scenes/red_sword.tscn");
  public static PackedScene enemyBomb = GD.Load<PackedScene>("res://scenes/red_bomb.tscn");

  public static List<PackedScene> enemieScenes = new List<PackedScene>() {
    enemyDagger,
    enemySword,
    enemyBomb,
    enemyBomb
  };

  public static TileMap getTilemap() {
    if (_tilemap == null) {
      MainLoop mainLoop = Godot.Engine.GetMainLoop();
      SceneTree sceneTree = mainLoop as SceneTree;
      Array<Node> nodes = sceneTree.GetNodesInGroup("TileMap");
      _tilemap = (TileMap)nodes[0];
    }
    return _tilemap;
  }

  public static HashSet<Unit> getUnits() {
    if (_units == null) {
      _units = new HashSet<Unit>();
    }
    return _units;
  }

  public static void addUnit(Unit unit) {
    getUnits().Add(unit);
  }

  public static void removeUnit(Unit removedUnit) {
    getUnits().Remove(removedUnit);
    bool endCombat = true;
    foreach (Unit unit in getUnits()) {
      if (unit.isEnemy == removedUnit.isEnemy) {
        endCombat = false;
        break;
      }
    }
    if (endCombat) {
      if (removedUnit.isEnemy) {
        endCombatPhase();
      } else {
        endGameText = "[center]Game Over: On round " + round + ", Party was wiped out[/center]";
        onGameOver();
      }
    }
  }

  public static void endBuyPhase() {
    isBuyPhase = false;

    foreach (Unit unit in getUnits()) {
      if (unit.isEnemy) {
        unit.resetColor();
      } else {
        unit.enable();
      }
    }

    int prev = round;
    round += 1 + Mathf.RoundToInt(round / 4);
    prevRound = prev;
    Random rand = new Random();
    TileMap tilemap = getTilemap();
    Array<Vector2I> usedCells = tilemap.GetUsedCells(0);
    usedCells.Shuffle();
    for (int i = 0; i < round; i++) {
      int dex = round == 2 ? 3 : rand.Next(0, enemieScenes.Count);
      Sprite2D enemy = (Sprite2D)enemieScenes[dex].Instantiate();
      foreach (Vector2I cell in usedCells) {
        if (AStar.isOccupied(tilemap, cell, null) == null) {
          enemy.Position = tilemap.MapToLocal(cell);
          tilemap.GetParent().AddChild(enemy);
          break;
        }
      }
    }
  }

  public static void endCombatPhase() {
    isBuyPhase = true;

    foreach (Unit unit in getUnits()) {
      unit.disable();
    }
  }

  public static void checkEndTurn(bool isEnemy) {
    bool turnFlip = true;
    foreach (Unit unit in getUnits()) {
      if (unit.isEnemy == isEnemy) {
        if (unit.isTurn) {
          turnFlip = false;
          break;
        }
      }
    }

    if (turnFlip) {
      TileMap tilemap = getTilemap();
      foreach (Unit unit in getUnits()) {
        foreach (Unit targetUnit in getUnits()) {
          if (unit.isEnemy && !targetUnit.isEnemy) {
            if (tilemap.GetCellTileData(0, tilemap.LocalToMap(targetUnit.Position)) == null) {
              continue;
            } else {
              List<Vector2I> path = AStar.findPath(tilemap.LocalToMap(unit.Position), tilemap.LocalToMap(targetUnit.Position), tilemap, unit, null);
              if (path.Count <= 0) {
                endGameText = "[center]Game Over: On round " + round + ", unreachable enemy detected[/center]";
                onGameOver();
                return;
              }
            }
          }
        }

        if (unit.isEnemy != isEnemy) {
          unit.readyTurn();
          unit.isTeamTurn = true;
        } else {
          unit.isTeamTurn = false;
          unit.resetColor();
        }
      }
    }
  }

  public static void onGameOver() {
    gameOver = true;
  }

  public static void onReset() {
    gameOver = false;
    _units = null;
    _tilemap = null;
    round = 1;
    prevRound = 1;
    MainLoop mainLoop = Godot.Engine.GetMainLoop();
    SceneTree sceneTree = mainLoop as SceneTree;
    sceneTree.ReloadCurrentScene();
  }
}