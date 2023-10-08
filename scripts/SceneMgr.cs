using Godot;
using System;
using System.Collections.Generic;

public partial class SceneMgr : Node2D {

  List<Vector2I> allySpawnPoints;
  List<Vector2I> enemySpawnPoints;
  List<Vector2I> neutralSpawnPoints;

  Dictionary<int, Vector2I> hueAtlasMap;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    TileMap tilemap = this.GetNode<TileMap>("../TileMap");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }
}
