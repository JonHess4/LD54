using Godot;
using System;

public partial class TileMapHelper : Godot.TileMap {
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    // if (Input.IsActionJustPressed("right_click")) {
    //   if (Engine.isBuyPhase) {
    //     Engine.endBuyPhase();
    //   } else {
    //     Engine.endCombatPhase();
    //   }
    // }
    // if (Engine.isBuyPhase) {
    //   if (Input.IsActionJustPressed("left_click")) {
    //     Vector2 mosPos = GetGlobalMousePosition();
    //     Vector2I mosCellPos = this.LocalToMap(mosPos);
    //     if (Math.Abs(mosCellPos.X) <= 4 && Math.Abs(mosCellPos.Y) <= 4 && this.GetCellTileData(0, mosCellPos) == null) {
    //       // TODO: implement cost
    //       this.SetCell(0, mosCellPos, 0, new Vector2I(1, 1));
    //     }
    //   }
    // }
  }
}
