using System;
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
  }

  public void drag(Vector2 mosPos) {
      this.unitScene.Position = mosPos;
      this.targetCell = this.tilemap.LocalToMap(this.unitScene.Position);
  }

  public void drop() {
      this.isSelected = false;

      this.unitScene.Position = this.tilemap.MapToLocal(this.targetCell);
  }
}