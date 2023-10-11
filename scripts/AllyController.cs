using System;
using System.Collections.Generic;
using Godot;

public partial class AllyController : Node2D {

  public UnitScene unitScene;
  public bool isSelected;
  public Vector2I targetCell;
  public Vector2I oldCell;
  public TileMap tilemap;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.unitScene = GetNode<UnitScene>("../../Unit");
    this.tilemap = GameMgr.tilemap;

    this.isSelected = false;
    this.oldCell = this.tilemap.LocalToMap(this.unitScene.Position);
    this.targetCell = this.oldCell;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    Vector2 mosPos = GetGlobalMousePosition();

    if (!this.isSelected
      && Input.IsActionJustPressed("left_click")
      && Math.Abs(mosPos.X - this.unitScene.Position.X) < 16
      && Math.Abs(mosPos.Y - this.unitScene.Position.Y) < 16
    ) {
      this.pickup();
    } else if (this.isSelected && Input.IsActionJustReleased("left_click")) {
      this.drop();
    }

    if (this.isSelected) {
      this.drag(mosPos);
    }
  }

  public void pickup() {
    this.isSelected = true;

    // display moveRange and threatZone UI for picked up unit
    List<Vector2I> moveCells = this.getMoveCells(this.oldCell, this.unitScene.unit.move);
    List<Vector2I> threatCells = this.getThreatCells(moveCells, this.unitScene.unit.range);
    this.setCells(1, moveCells, 0, new Vector2I(3, 1));
    this.setCells(1, threatCells, 0, new Vector2I(3, 2));
  }

  public void drag(Vector2 mosPos) {
    this.unitScene.Position = mosPos;
    this.targetCell = this.tilemap.LocalToMap(this.unitScene.Position);
  }

  public void drop() {
    this.isSelected = false;
    this.tilemap.ClearLayer(1);

    this.unitScene.Position = this.tilemap.MapToLocal(this.targetCell);
    this.oldCell = this.targetCell;
  }

  public List<Vector2I> getMoveCells(Vector2I startPos, int move) {
    List<Vector2I> moveCells = new List<Vector2I>();

    int x;
    int y;

    x = move;
    for (int i = x * -1; i <= x; i++) {
      y = Math.Abs(i) - move;
      for (int j = y; j <= Math.Abs(y); j++) {
        // TODO: verify if the unit can reach/path to the cell
        if (this.tilemap.GetCellTileData(0, startPos + new Vector2I(i, j)) != null) {
          moveCells.Add(startPos + new Vector2I(i, j));
        }
      }
    }

    return moveCells;
  }

  public List<Vector2I> getThreatCells(List<Vector2I> moveCells, int range) {
    List<Vector2I> threatCells = new List<Vector2I>();

    int x;
    int y;

    x = range;
    foreach (Vector2I cell in moveCells) {
      for (int i = x * -1; i <= x; i++) {
        y = Math.Abs(i) - range;
        for (int j = y; j <= Math.Abs(y); j++) {
          if (!moveCells.Contains(cell + new Vector2I(i, j))) {
            threatCells.Add(cell + new Vector2I(i, j));
          }
        }
      }
    }

    return threatCells;
  }

  public void setCells(int layer, List<Vector2I> tiles, int sourceId, Vector2I atlasCoords) {
    foreach (Vector2I tile in tiles) {
      this.tilemap.SetCell(layer, tile, 0, atlasCoords);
    }
  }

}