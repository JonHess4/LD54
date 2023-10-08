using Godot;
using System;

public partial class EndBuyPhaseBtn : Button {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.Visible = false;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.gameOver && !this.Visible) {
      this.Visible = true;
    } else if (!Engine.gameOver && this.Visible) {
      this.Visible = false;
    }
  }

  public override void _Pressed() {
    base._Pressed();
    Engine.onReset();
  }
}
