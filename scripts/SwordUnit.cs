using Godot;

public partial class SwordUnit : AllyUnit {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

  this.unitName = "Blue Sword";

    this.movement = 3;
    this.maxHp = 8;
    this.currentHp = this.maxHp;
    this.atk = 6;
    this.minAtk = 3;
    this.atkRange = 1;

    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
  }
}