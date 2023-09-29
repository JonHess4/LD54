using Godot;

public partial class TomeUnit : AllyUnit {

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    Engine.addUnit(this);
    this.isEnemy = false;
    this.movement = 6;
    this.maxHp = 4;
    this.currentHp = this.maxHp;
    this.atk = 2;
    this.atkRange = 2;
    this.canHeal = true;
    this.isTurn = true;

    this.isSelected = false;
    this.tilemap = GetNode<TileMap>("../TileMap");
    this.tilemap2 = GetNode<TileMap>("../TileMap2");
    this.healthbar = GetNode<ProgressBar>("./HealthBar");
    StyleBoxFlat sbf = new StyleBoxFlat();
    healthbar.AddThemeStyleboxOverride("fill", sbf);
    sbf.BgColor = new Color("C0483D");
    this.healthbar.MaxValue = this.maxHp;
    this.healthbar.Value = this.currentHp;
    this.oldCellPos = this.tilemap.LocalToMap(this.Position);
    this.targetCellPos = this.tilemap.LocalToMap(this.Position);
    this.pass = false;
  }
}