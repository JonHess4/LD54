using Godot;

public partial class DaggerUnit : AllyUnit {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

    this.movement = 6;
    this.maxHp = 2;
    this.currentHp = this.maxHp;
    this.atk = 2;
    this.minAtk = 2;
    this.atkRange = 1;

    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
  }
}