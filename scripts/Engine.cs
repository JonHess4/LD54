using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

public partial class Engine : Camera2D {
  public static HashSet<Unit> units = new HashSet<Unit>();
  public static Camera2D _cam;
  public static TileMap _tilemap;
  public static bool isBuyPhase = false;
  public static int round = 1;
  public static int prevRound = 1;

  public static PackedScene enemyDagger = GD.Load<PackedScene>("res://scenes/red_dagger.tscn");
  public static PackedScene enemySword = GD.Load<PackedScene>("res://scenes/red_sword.tscn");
  public static PackedScene enemyBomb = GD.Load<PackedScene>("res://scenes/red_bomb.tscn");

  public static List<PackedScene> enemieScenes = new List<PackedScene>() {
    enemyDagger,
    enemySword,
    enemyBomb,
    enemyBomb
  };

  public static Camera2D getCam() {
    if (_cam == null) {
      MainLoop mainLoop = Godot.Engine.GetMainLoop();
      SceneTree sceneTree = mainLoop as SceneTree;
      Array<Node> nodes = sceneTree.GetNodesInGroup("Cam");
      _cam = (Camera2D)nodes[0];
    }
    return _cam;
  }

  public static TileMap getTilemap() {
    if (_tilemap == null) {
      MainLoop mainLoop = Godot.Engine.GetMainLoop();
      SceneTree sceneTree = mainLoop as SceneTree;
      Array<Node> nodes = sceneTree.GetNodesInGroup("TileMap");
      _tilemap = (TileMap)nodes[0];
    }
    return _tilemap;
  }

  public static void addUnit(Unit unit) {
    units.Add(unit);
  }

  public static void removeUnit(Unit removedUnit) {
    units.Remove(removedUnit);
    bool endCombat = true;
    foreach (Unit unit in units) {
      if (unit.isEnemy == removedUnit.isEnemy) {
        endCombat = false;
        break;
      }
    }
    if (endCombat) {
      if (removedUnit.isEnemy) {
        endCombatPhase();
      } else {
        units.Clear();
        MainLoop mainLoop = Godot.Engine.GetMainLoop();
        SceneTree sceneTree = mainLoop as SceneTree;
        sceneTree.ReloadCurrentScene();
      }
    }
  }

  public static void endBuyPhase() {
    isBuyPhase = false;

    foreach (Unit unit in units) {
      if (unit.isEnemy) {
        unit.resetColor();
      } else {
        unit.enable();
      }
    }

    int prev = round;
    round += prevRound;
    prevRound = prev;
    Random rand = new Random();
    TileMap tilemap = getTilemap();
    Array<Vector2I> usedCells = tilemap.GetUsedCells(0);
    usedCells.Shuffle();
    for (int i = 0; i < round; i++) {
      int dex = rand.Next(0, enemieScenes.Count);
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

    foreach (Unit unit in units) {
      unit.disable();
    }
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
          unit.readyTurn();
          unit.isTeamTurn = true;
        } else {
          unit.isTeamTurn = false;
          unit.resetColor();
        }
      }
    }
  }
}