using Godot;

public partial class UnitScene : Sprite2D {
  public Unit unit;
  public RichTextLabel dmgText;
  public ProgressBar hpBar;

    // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    this.dmgText = this.GetNode<RichTextLabel>("./DmgText");
    this.hpBar = this.GetNode<ProgressBar>("./HpBar");
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  public void setUnit(Unit newUnit) {
    
  }
}