using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class ButtonHelper : Godot.Button {
  // Called when the node enters the scene tree for the first time.
  Vector2 startPos;
  public String unitName;
  public TileMap tilemap;
  public PackedScene unit;
  public override void _Ready() {
    this.startPos = this.Position;
    this.tilemap = GetNode<TileMap>("../TileMap");
    StyleBoxFlat sbf = new StyleBoxFlat();
    this.AddThemeStyleboxOverride("bgColor", sbf);
    sbf.BgColor = new Color("345f7c");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.isBuyPhase && !this.Visible) {
      this.Visible = true;
      this.Disabled = false;
      foreach(Unit unit in Engine.units) {
        if (unit.Name == this.unitName) {
          this.Disabled = true;
          break;
        }
      }
    } else if (!Engine.isBuyPhase && this.Visible) {
      this.Visible = false;
    }
  }

  
  public void spawn() {
    Sprite2D unitSprite = (Sprite2D)this.unit.Instantiate();
    Array<Vector2I> usedCells = tilemap.GetUsedCells(0);
    Vector2I[] orderedCells = usedCells.OrderBy(cell => Math.Abs(cell.X) + Math.Abs(cell.Y)).ToArray();

    foreach (Vector2I cell in orderedCells) {
      if (AStar.isOccupied(this.tilemap, cell, null) == null) {
        unitSprite.Position = this.tilemap.MapToLocal(cell);
        GetParent().AddChild(unitSprite);
        Engine.endBuyPhase();
        break;
      }
    }
  }

  public override void _Pressed() {
    base._Pressed();

    // TODO: Implement Cost
    this.spawn();
  }
}
