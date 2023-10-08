using Godot;
using System;

public partial class EndGameDetails : RichTextLabel {
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.Text = Engine.endGameText;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Engine.gameOver && !this.Visible) {
      this.Text = Engine.endGameText;
      this.Visible = true;
    } else if (!Engine.gameOver && this.Visible) {
      this.Visible = false;
    }
  }
}
