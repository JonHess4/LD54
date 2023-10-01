using Godot;

public partial class ESwordUnit : EnemyUnit {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    base._Ready();

    this.movement = 4;
    this.maxHp = 8;
    this.currentHp = this.maxHp;
    this.atk = 5;
    this.minAtk = 2;
    this.atkRange = 1;

    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
  }
}