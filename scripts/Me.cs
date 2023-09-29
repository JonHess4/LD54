using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Me : Sprite2D {
  bool isSelected;
  TileMap tilemap;
  TileMap tilemap2;
  Vector2I posMod = new Vector2I(0, -1);
  Vector2I oldCellPos;
  Vector2I targetCellPos;
  List<Vector2I> path;
  int movement = 6;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.isSelected = false;
    this.tilemap = GetNode<TileMap>("../TileMap");
    this.tilemap2 = GetNode<TileMap>("../TileMap2");
    this.oldCellPos = this.tilemap.LocalToMap(this.Position);
    this.targetCellPos = this.tilemap.LocalToMap(this.Position);
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    Vector2 mosPos = GetGlobalMousePosition();
    if (Input.IsActionJustPressed("left_click")) {
      if (Math.Abs(mosPos.X - this.Position.X) < 8 && Math.Abs(mosPos.Y - this.Position.Y) < 8) {
        this.isSelected = true;
        this.path = new List<Vector2I>();
      }
    } else if (isSelected && Input.IsActionJustReleased("left_click")) {
      this.isSelected = false;

      // drop character back onto map
      this.Position = this.tilemap.MapToLocal(this.path.Last()) + this.posMod;
      this.oldCellPos = this.path.Last();

      this.clearPath(this.path, this.tilemap2);
    }

    if (this.isSelected) {
      this.Position = mosPos;
      Vector2I cellPos = this.tilemap2.LocalToMap(this.Position);
      // TODO: collect the cells the unit is dragged over and prioritize them for pathing
      Vector2I newTargetCellPos = this.tilemap.LocalToMap(this.Position);
      if (newTargetCellPos != this.targetCellPos || this.path.Count <= 0) {
        GD.Print("New Target Cell Pos");
        this.targetCellPos = newTargetCellPos;
        this.clearPath(this.path, this.tilemap2);
        this.path = AStar.findPath(oldCellPos, targetCellPos, this.tilemap);
        this.drawPath(this.path, this.tilemap2);
      }
    }
  }

  void drawPath(List<Vector2I> cellCords, TileMap tilemap) {
    foreach (Vector2I cellPos in cellCords) {
      GD.Print(cellPos);
      tilemap.SetCell(0, cellPos, 0, new Vector2I(2, 3));
    }
  }

  void clearPath(List<Vector2I> path, TileMap tileMap) {
    foreach (Vector2I cellPos in path) {
      tilemap.SetCell(0, cellPos);
    }
  }
}
