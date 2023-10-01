using Godot;

public partial class BowUnit : AllyUnit {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
	base._Ready();

  this.unitName = "Blue Bow";
	this.movement = 4;
	this.maxHp = 6;
	this.currentHp = this.maxHp;
	this.atk = 4;
	this.minAtk = 2;
	this.atkRange = 2;

	this.healthbar.MaxValue = this.maxHp;
	this.healthbar.Value = this.currentHp;
  }
}
