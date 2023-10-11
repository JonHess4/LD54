using Godot;
using System;
using System.Collections.Generic;

// TODO: Each scene will have it's own SceneMgr script
// so this will need to be renamed to like 'TutorialSceneMgr'

public partial class SceneMgr : Node2D {

  public List<Unit> enemyList;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
	this.initEnemyList();
	Camera2D cam = this.GetNode<Camera2D>("../Camera2D");
	TileMap tilemap = this.GetNode<TileMap>("../TileMap");
  GameMgr.tilemap = tilemap;
	CompressedTexture2D mapData = GD.Load<CompressedTexture2D>("res://sprites/map_data.png");

	Dictionary<string, List<Vector2I>> posMap = MapFactory.initMap(mapData.GetImage(), tilemap);

	// TODO: retrieve the available ally units
	this.spawnUnits(tilemap, posMap["allySpawns"], GameMgr.allyUnits);

	this.spawnUnits(tilemap, posMap["enemySpawns"], this.enemyList);

	// TODO: retrieve the available neutral units
	// No Neutral units in this scene
	// this.spawnUnits(tilemap, posMap["neutralSpawns"], new List<Unit>());

	cam.Position = tilemap.MapToLocal(posMap["camSpawns"][0]);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  // Enemies are specific to the scene, so the SceneMgr can own them
  // Might want to pull from a json files still though
  public void initEnemyList() {
	this.enemyList = new List<Unit>();
	Swordsman swordsman = new Swordsman();
	swordsman.group = "enemy";
	this.enemyList.Add(swordsman);
  }

  public void spawnUnits(TileMap tilemap, List<Vector2I> spawnPoints, List<Unit> units) {
	int i = 0;
	while (i < spawnPoints.Count && i < units.Count) {
	  UnitScene unit = GameMgr.unitScene.Instantiate<UnitScene>();
	  unit.setUnit(units[i]);
	  unit.Position = tilemap.MapToLocal(spawnPoints[i]);
	  this.GetParent().CallDeferred("add_child", unit);

	  i++;
	}
  }
}
