using Godot;
using System;

public partial class TileMap3 : TileMap {
  TileMap mainTilemap;
  Vector2I prevCell;
  bool hasPrevCell;
  bool endPhase = false;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.mainTilemap = this.GetNode<TileMap>("../TileMap");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.gameOver) {
      return;
    }
    if (Engine.isBuyPhase) {
      Vector2 mosPos = this.GetGlobalMousePosition();
      Vector2I mosCellPos = this.LocalToMap(mosPos);
      if (this.hasPrevCell && this.prevCell != mosCellPos) {
        this.SetCell(0, this.prevCell, 1, new Vector2I(2, 2));
        this.hasPrevCell = false;
      }
      if (this.prevCell != mosCellPos && this.GetCellTileData(0, mosCellPos) != null) {
        this.SetCell(0, mosCellPos, 1, new Vector2I(1, 2));
        this.prevCell = mosCellPos;
        this.hasPrevCell = true;
      }
      if (Input.IsActionJustPressed("left_click") && this.GetCellTileData(0, mosCellPos) != null) {
        if (this.mainTilemap.GetCellTileData(0, mosCellPos) == null) {
          this.mainTilemap.SetCell(0, mosCellPos, 0, new Vector2I(1, 1));
          this.endPhase = true;
        }

        foreach (BaseUnit unit in Engine.getUnits()) {
          if (this.LocalToMap(unit.Position) == mosCellPos) {
            unit.upgrade();
            this.endPhase = true;
          }
        }
        if (this.endPhase) {
          this.endPhase = false;
          Engine.endBuyPhase();
        }
      }
    }
  }
}
