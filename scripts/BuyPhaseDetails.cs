using Godot;
using System;

public partial class BuyPhaseDetails : RichTextLabel {
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.gameOver) {
      return;
    }
    if (Engine.isBuyPhase && !this.Visible) {
      this.Visible = true;
    } else if (!Engine.isBuyPhase && this.Visible) {
      this.Visible = false;
    }
  }
}
