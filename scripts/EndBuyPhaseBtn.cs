using Godot;
using System;

public partial class EndBuyPhaseBtn : Button {
  Vector2 startPos;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.startPos = this.Position;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.isBuyPhase && !this.Visible) {
      this.Visible = true;
    } else if (!Engine.isBuyPhase && this.Visible) {
      this.Visible = false;
    }
  }

  public override void _Pressed() {
    base._Pressed();
    Engine.endBuyPhase();
  }
}
