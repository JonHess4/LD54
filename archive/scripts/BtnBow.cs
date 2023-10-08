using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class BtnBow : ButtonHelper {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();
    this.unit = GD.Load<PackedScene>("res://scenes/blue_bow.tscn");
    this.unitName = "Blue-bow";
  }
}
